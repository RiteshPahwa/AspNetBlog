using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace ASPNetBlog.App_Common.Mapper
{
    // Ritesh Pahwa @ RiteshPahwa.com
    // Small Mapper for minimal dependencies as ASP.Net 5 MVC 6 is still in beta
    // Can be replaced with AutoMapper or similar later on
    public class Mapper
    {
        // Copy all the properties of From to To
        public static To Map<From, To>(From from, To to, bool iterateOverTo = false, Func<To, object> exclude = null)
        {
            if (from == null || to == null) return to;

            if (!iterateOverTo) {
                foreach (PropertyInfo prop in from.GetType().GetProperties().Where(p => !(exclude == null ? false : exclude(to).GetType().GetProperties().Any(x => x.Name == p.Name)))) {
                    try { to.GetType().GetProperty(prop.Name).SetValue(to, prop.GetValue(from)); }
                    catch (Exception e) { throw new Exception($"Property mapping  error for: {prop.Name} | {e.Message}", e); }
                }
            } else {
                // Copy all the properties of From to To but loop using To properties
                foreach (PropertyInfo prop in to.GetType().GetProperties().Where(p=> !(exclude == null ? false : exclude(to).GetType().GetProperties().Any(x => x.Name == p.Name)))) {
                    try { prop.SetValue(to, from.GetType().GetProperty(prop.Name).GetValue(from)); }
                    catch (Exception e) { throw new Exception($"Property mapping  error for: {prop.Name} | {e.Message}", e); }
                }
            }

            return to;
        }

        // Copy the properties of From to To based on provided list
        public static To Map<From, To>(From from, To to, Func<From, object> include)
        {
            if (from == null || to == null) return to;

            foreach (PropertyInfo prop in include(from).GetType().GetProperties()) {
                try { to.GetType().GetProperty(prop.Name).SetValue(to, from.GetType().GetProperty(prop.Name).GetValue(from)); }
                catch (Exception e) { throw new Exception($"Property mapping  error for: {prop.Name} | {e.Message}", e); }
            }

            return to;
        }

        // Copy the properties of From to To based on provided list
        public static List<To> Map<From,To>(List<From> from, Func<From, object> include) where To : new()
        {
            if (from == null) return null;

            var list = new List<To>();
            foreach (var frm in from)
            {
                list.Add(Map(frm, new To(), include));
            }

            return list;
        }

    }
}
