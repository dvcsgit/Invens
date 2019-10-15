using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using PFG.Inventory.Web.Library.Enums;
using PFG.Inventory.Web.Library.Models;
using PFG.Inventory.Web.Library.Extensions;
using System.Web;

namespace PFG.Inventory.Web.Library.Filters
{
    /// <summary>
    /// 檢查登入的帳號有無權限使用此功能
    /// </summary>
    public class OperationCheckAttribute : ActionFilterAttribute, IActionFilter
    {
        private EnumOperation _operation { get; set; }

        public OperationCheckAttribute(EnumOperation operation)
        {
            this._operation = operation;
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            //如果沒特別指定的話 就會照預設 Action=> Index == EnumOperation.Query
            //如果有指定的話 _operation
            if (filterContext.HttpContext.User.Identity.IsAuthenticated)
            {
                var controllerName = filterContext.RouteData.Values["controller"].ToString();
                var actionName = filterContext.RouteData.Values["action"].ToString();
                MVCUser mvcUser = filterContext.HttpContext.User.GetMVCUser();

                var query = filterContext.HttpContext.Request.Url.Query;

                var roleKey = "RoleKey";
                foreach (var item in mvcUser.Roles)
                {
                    roleKey += item;
                }

                var permissionOperationMap = HttpRuntime.Cache.GetOrInsert<Dictionary<string, EnumOperation>>(roleKey, () => DictionaryUtils.GetPermissionOperationMap(mvcUser.Roles));

                var currentOperation = permissionOperationMap.ContainsKey(controllerName) ?
                    permissionOperationMap[controllerName] : EnumOperation.None;

                if (_operation != EnumOperation.None && !currentOperation.HasFlag(_operation))
                {
                    if (filterContext.HttpContext.Request.IsAjaxRequest())
                    {
                        filterContext.Result = new ViewResult
                        {
                            ViewName = "ErrorPermission"
                        };

                    }
                    else
                    {

                        filterContext.Result = new ViewResult
                        {
                            ViewName = "ErrorPermission"
                        };

                    }

                    //(filterContext.Result as ViewResult).ViewBag.HelloWorld = "test";
                }

            }
        }



    }
}
