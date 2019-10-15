using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace PFG.Inventory.Web.Library.Extensions
{
    /// <summary>
    /// http://weblogs.asp.net/srkirkland/archive/2009/09/17/a-urlhelper-extension-for-creating-absolute-action-paths-in-asp-net-mvc.aspx
    /// Url.AbsoluteAction("Edit", "Users", new {id="username"});
    /// </summary>
    public static class UrlExtensions
    {
        public static string AbsoluteAction(this UrlHelper url, string action, string controller, object routeValues)
        {
            Uri requestUrl = url.RequestContext.HttpContext.Request.Url;

            string absoluteAction = string.Format("{0}{1}",
                                                  requestUrl.GetLeftPart(UriPartial.Authority),
                                                  url.Action(action, controller, routeValues));

            return absoluteAction;
        }
    }
}
