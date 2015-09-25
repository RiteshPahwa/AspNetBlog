using System;
using Microsoft.AspNet.Hosting;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Http.Features;
/* MVC 6 Coding Example -- Ritesh Pahwa 9/12/15*/

namespace ASPNetBlog.App_Common.Session
{
    public static class ContextAccessor
    {
        private static IHttpContextAccessor _httpContextAccessor;
        public static HttpContext Context => _httpContextAccessor.HttpContext;
        public static ISession Session => _httpContextAccessor.HttpContext.Session;

        public static void Configure(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

    }

    public class AppSession
    {
        public static ISession Session => ContextAccessor.Session;        

        public static int? AppUserId
        {
            get { return Session?.GetInt32(nameof(AppUserId)); } //C#6.0
            set {
                if (value == null) return;
                Session.SetInt32(nameof(AppUserId), value ?? default(int));
            }
        }
        public static string FullName
        {
            get { return Session?.GetString(nameof(FullName)); } //C#6.0
            set
            {
                if (value == null) return;
                Session.SetString(nameof(FullName), value);
            }
        }
        public static string Email
        {
            get { return Session?.GetString(nameof(Email)); } //C#6.0
            set
            {
                if (value == null) return;
                Session.SetString(nameof(Email), value);
            }
        }
        public static string PhoneNumber
        {
            get { return Session?.GetString(nameof(PhoneNumber)); } //C#6.0
            set {
                if (value == null) return;
                Session.SetString(nameof(PhoneNumber), value);
            }
        }

        public static void Clear()
        {
            Session.Clear();
        }
    }
}

