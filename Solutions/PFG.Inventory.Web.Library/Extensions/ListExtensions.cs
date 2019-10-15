using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace PFG.Inventory.Web.Library.Extensions
{
    /// <summary>
    /// 
    /// </summary>
    public static class ListExtensions
    {
        public static IEnumerable<T> ForEach<T>(this IEnumerable<T> collection, Action<T> action)
        {
            foreach (var item in collection) action(item);
            return collection;
        }

        public static SelectList ToSelectList<T>(this IEnumerable<T> collection)
        {
            return new SelectList(collection, "Value", "Text");
        }

        /// <summary>
        /// 將  IEnumerable<SelectListItem> 把打中的選項selected
        /// 單選
        /// Useage: SelectListUtils.GetBaseDateSelect().ToSelectList(item.BaseDate);
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection"></param>
        /// <param name="selectedValue"></param>
        /// <returns></returns>
        public static SelectList ToSelectList<T>(this IEnumerable<T> collection, string selectedValue)
        {
            return new SelectList(collection, "Value", "Text", selectedValue);
        }

        public static SelectList ToSelectList<T>(this IEnumerable<T> collection,
                             string dataValueField, string dataTextField)
        {
            return new SelectList(collection, dataValueField, dataTextField);
        }

        public static SelectList ToSelectList<T>(this IEnumerable<T> collection,
                             string dataValueField, string dataTextField, string selectedValue)
        {
            return new SelectList(collection, dataValueField, dataTextField, selectedValue);
        }

        /// <summary>
        /// 多選
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection"></param>
        /// <param name="selectedValue"></param>
        /// <returns></returns>
        public static MultiSelectList ToSelectList<T>(this IEnumerable<T> collection, List<string> selectedValue)
        {
            return new MultiSelectList(collection, "Value", "Text", selectedValue);
        }

        /// <summary>
        /// 多選
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection"></param>
        /// <param name="selectedValue"></param>
        /// <returns></returns>
        public static MultiSelectList ToSelectList<T>(this IEnumerable<T> collection, List<int> selectedValue)
        {
            return new MultiSelectList(collection, "Value", "Text", selectedValue);
        }
    }
}
