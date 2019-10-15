using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PFG.Inventory.Web.Library.Extensions
{
    public static class HashSetExtensions
    {
        /// <summary>
        /// useage : HashSet => HashSet.AddRange()
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="hashSet"></param>
        /// <param name="collection"></param>
        public static void AddRange<T>(this HashSet<T> hashSet, IEnumerable<T> collection)
        {
            foreach (T item in collection)
            {
                hashSet.Add(item);
            }
        }
    }
}
