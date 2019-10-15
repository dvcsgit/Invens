using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PFG.Inventory.Web.Areas.WebApi.Models
{
    public class UserLoginInfo
    {
        /// <summary>
        /// 是否成功登入
        /// </summary>
        public bool IsLoginValid { get; set; }

        /// <summary>
        /// 無法登入錯誤訊息
        /// </summary>
        public string ErrorMessage { get; set; }

        /// <summary>
        /// 成功登入後-帳號
        /// </summary>
        public string Account { get; set; }

        /// <summary>
        /// 成功登入後-姓名
        /// </summary>
        public string Name { get; set; }
    }
}