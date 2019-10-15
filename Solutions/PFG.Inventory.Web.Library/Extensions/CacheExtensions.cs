using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Caching;

namespace PFG.Inventory.Web.Library.Extensions
{
    /// <summary>
    /// usage:object o = HttpRuntime.Cache.GetOrInsert<object>("key1",GenObj);
    /// Reference:http://minalabib.wordpress.com/2009/12/14/extending-cache-to-get-or-insert-data/
    /// </summary>
    public static class CasheExtensions
    {
        public static T GetOrInsert<T>(this Cache Cache, string key, Func<T> generator)
        {
            var result = Cache[key];
            if (result != null)
                return (T)result;
            result = (generator != null) ? generator() : default(T);
            Cache.Insert(key, result);
            return (T)result;
        }
    }
}
