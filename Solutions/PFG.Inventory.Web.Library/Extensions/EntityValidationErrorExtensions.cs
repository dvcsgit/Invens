using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using PFG.Inventory.Web.Library.Common;

namespace PFG.Inventory.Web.Library.Extensions
{
    /// <summary>
    /// EntityValidation
    /// </summary>
    public static class EntityValidationErrorExtensions
    {
        /// <summary>
        /// 將Entity Validation 底層集合的錯誤 轉成 ModelState的Error
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static IEnumerable<ValidationResult> GetErrors(this DbEntityValidationException source)
        {
            List<ValidationResult> result = new List<ValidationResult>();
            string errorMsgFromat = "欄位:{0},錯誤:{1}";
            foreach (var validationErrors in source.EntityValidationErrors)
            {
                foreach (var validationError in validationErrors.ValidationErrors)
                {
                    yield return new ValidationResult("EntityValidation", string.Format(errorMsgFromat, validationError.PropertyName, validationError.ErrorMessage));
                }
            }
        }

        /// <summary>
        /// 將錯誤訊息倒出來
        /// 給Nlog使用
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string ToLogString(this IEnumerable<ValidationResult> source)
        {
            var result = "";
            var tempQuery = source.Select(x => x.Message).ToList();
            result = string.Join(",", tempQuery);
            return result;
        }
    }
}
