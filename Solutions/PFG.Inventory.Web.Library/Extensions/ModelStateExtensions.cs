using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using PFG.Inventory.Web.Library.Models;

namespace PFG.Inventory.Web.Library.Extensions
{
    public static class ModelStateExtensions
    {
        /// <summary>
        /// 將 ModelState的Error轉成 IEnumerable<string>
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static IEnumerable<string> GetErrors(this ModelStateDictionary source)
        {
            return source.SelectMany(x => x.Value.Errors.Select(error => error.ErrorMessage));
        }
        /// <summary>
        /// 返回錯誤集合
        /// </summary>
        /// <param name="source">錯誤狀態</param>
        /// <returns>錯誤集合</returns>
        public static IEnumerable<Error> GetErrorsByPair(this ModelStateDictionary source)
        {
            IEnumerable<string> key = source.Select(x => x.Key);
            IEnumerable<string> value = source.SelectMany(x => x.Value.Errors.Select(err=>err.ErrorMessage));
            List<Error> errors = new List<Error>();
            for(int i=0;i<key.Count();i++)
            {
                Error err = new Error();
                err.Key = key.ElementAt(i);
                err.Value = value.ElementAt(i);
                errors.Add(err);
            }
            return errors.AsEnumerable();
        }

        /// <summary>
        /// 將 ModelState的Error轉成 string
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string GetErrorsString(this ModelStateDictionary source)
        {
            return string.Join(";", source.SelectMany(x => x.Value.Errors.Select(error => error.ErrorMessage)));
        }

    }
}
