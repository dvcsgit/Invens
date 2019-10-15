using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PFG.Inventory.Web.Library.Models
{
    /// <summary>
    /// 錯誤信息
    /// </summary>
    public class Error
    {
        /// <summary>
        ///驗證錯誤的Key值
        /// </summary>
        public string Key { get; set; }
        /// <summary>
        /// 驗證錯誤的Value值
        /// </summary>
        public string Value { get; set; }
    }
}