using Microsoft.AspNet.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Framework.Caching.Memory;
/* MVC 6 Coding Example -- Ritesh Pahwa 9/15/15*/

namespace ASPNetBlog.Helpers
{
    public static class IMemoryCacheExtensions
    {
        public static T GetOrSet<T>(this IMemoryCache cache, string key, Func<T> generator)
        {
            var result = cache.Get(key);
            if (result == null)
            {
                result = generator();
                cache.Set(key, result);
            }
            return (T)result;
        }
    }

}
