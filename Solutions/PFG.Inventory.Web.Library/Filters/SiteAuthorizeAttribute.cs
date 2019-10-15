using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace PFG.Inventory.Web.Library.Filters
{
    /// <summary>
    /// 權限處理 處理有無登入系統
    /// Note: 所有需要登入才能使用的Action都需掛此filter
    /// </summary>
    public class SiteAuthorizeAttribute : AuthorizeAttribute
    {
    }
}
