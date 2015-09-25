using Microsoft.AspNet.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
/* MVC 6 Coding Example -- Ritesh Pahwa 9/12/15*/

namespace ASPNetBlog.App_Common.Extensions
{
    public static class EnumExtensions
    {
        public static SelectList ToSelectList<T>(this T enumList)
            where T : struct, IComparable, IFormattable, IConvertible
        {
            var values = from T e in Enum.GetValues(typeof(T))
                         select new { Id = e, Name = e.ToString() };
            return new SelectList(values, "Id", "Name", enumList);
        }

        public static IEnumerable<SelectListItem> SelectListItems<T>(this T enumList)
            where T : struct, IComparable, IFormattable, IConvertible
        {
            //var values = from T e in Enum.GetValues(typeof(T))
            //             select new SelectListItem() { Value = (int)e = e.ToString() };

            //KeyValuePair<int, string> kp = new KeyValuePair<int, string>();
            //foreach (Enum value in Enum.GetValues(typeof(T)))
            //             kp = new KeyValuePair<int, string>((int)value, value.ToString());

            return (Enum.GetValues(typeof(T)).Cast<int>()
                .Select(e => new SelectListItem() { Text = Enum.GetName(typeof(T), e), Value = e.ToString() })).ToList();
            //return values;
        }
    }
}
