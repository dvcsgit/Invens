using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PFG.Inventory.Web.Library
{
    public class WebSiteConst
    {
        /// <summary>
        /// Remember cookieName
        /// </summary>
        public static readonly string RememberCookie = "RememberPFG.Inventory";
    }

    public class StatusCodeConst
    {
        /// <summary>
        /// 資料庫未更動
        /// </summary>
        public const string Y = "Y";

        /// <summary>
        /// 新增
        /// </summary>
        public const string I = "I";

        /// <summary>
        /// 更新資料
        /// </summary>
        public const string O = "O";

        /// <summary>
        /// 換排位
        /// </summary>
        public const string C = "C";
    }
}