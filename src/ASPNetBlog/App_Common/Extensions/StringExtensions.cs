using Microsoft.AspNet.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
/* MVC 6 Coding Example -- Ritesh Pahwa 9/21/15*/

namespace ASPNetBlog.App_Common.Extensions
{
    public static class StringExtensions
    {
        public static string ToSlug(this string text)
        {
            string str = text.RemoveAccent().ToLower();
            str = Regex.Replace(str, @"[^a-z0-9\s-]", "");
            str = Regex.Replace(str, @"\s+", " ").Trim();
            str = str.Substring(0, str.Length <= 50 ? str.Length : 50).Trim();
            str = Regex.Replace(str, @"\s", "-"); 
            return str;
        }

        public static string RemoveAccent(this string text)
        {
            byte[] bytes = System.Text.Encoding.GetEncoding("Cyrillic").GetBytes(text);
            return System.Text.Encoding.ASCII.GetString(bytes);
        }

        public static HtmlString ToHtmlString(this string text)
        {
            return new HtmlString(text);
        }
    }
}
