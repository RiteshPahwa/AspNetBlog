using Microsoft.AspNet.Http;
using Microsoft.AspNet.Http.Features;
using Newtonsoft.Json;
using System;
/* MVC 6 Coding Example -- Ritesh Pahwa 9/12/15*/

namespace ASPNetBlog.App_Common.Session
{
    public static class SessionExtensions
    {
        public static void SetAppSession(this ISession session)
        {
            //ContextAccessor.
            //AppSession.Session = session;
        }

        public static bool? GetBoolean(this ISession session, string key)
        {
            var data = session.Get(key);
            if (data == null)
            {
                return null;
            }
            return BitConverter.ToBoolean(data, 0);
        }

        public static void SetBoolean(this ISession session, string key, bool value)
        {
            session.Set(key, BitConverter.GetBytes(value));
        }

        public static DateTime? GetDateTime(this ISession session, string key)
        {
            var data = session.Get(key);
            if (data == null)
            {
                return null;
            }

            long dateInt = BitConverter.ToInt64(data, 0);
            return new DateTime(dateInt);
        }

        public static void SetDateTime(this ISession session, string key, DateTime value)
        {
            session.Set(key, BitConverter.GetBytes(value.Ticks));
        }

        public static T GetObjectFromJson<T>(this ISession session, string key)
        {
            var value = session.GetString(key);

            return value == null ? default(T) : JsonConvert.DeserializeObject<T>(value);
        }

        public static void SetObjectAsJson(this ISession session, string key, object value)
        {
            session.SetString(key, JsonConvert.SerializeObject(value));
        }

    }
}
