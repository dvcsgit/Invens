using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using PFG.Inventory.Web.Library;
using PFG.Inventory.Web.Library.Enums;
using PFG.Inventory.Web.Library.Extensions;
using PFG.Inventory.Web.Library.Models;

namespace PFG.Inventory.Web.Filters
{
    /// <summary>
    /// Inject a ViewBag object to Views for getting information about an authenticated user
    /// 進來此功能會將 Operation給塞入
    /// 1.當Action 有套用 AllowAnonymousAttribute 則會不會產生 UserModel
    /// </summary>
    public class UserPermissionAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            if (!filterContext.ActionDescriptor.IsDefined(typeof(AllowAnonymousAttribute), true) && !filterContext.ActionDescriptor.ControllerDescriptor.IsDefined(typeof(AllowAnonymousAttribute), true))
            {
                UserModel userModel;
                if (filterContext.Controller.ViewBag.UserModel == null)
                {
                    userModel = new UserModel();
                    filterContext.Controller.ViewBag.UserModel = userModel;
                }
                else
                {
                    userModel = filterContext.Controller.ViewBag.UserModel as UserModel;
                }

                if (filterContext.HttpContext.User.Identity.IsAuthenticated)
                {
                    MVCUser mvcUser = filterContext.HttpContext.User.GetMVCUser();
                    userModel.IsUserAuthenticated = mvcUser.IsAuthenticated;
                    userModel.UserName = mvcUser.DisplayName;
                    userModel.RoleName = mvcUser.RoleName;

                    var roleKey = "RoleKey";
                    foreach (var item in mvcUser.Roles)
                    {
                        roleKey += item;
                    }

                    var controllerName = filterContext.RouteData.Values["controller"].ToString();

                    //只能從Controller 因設計上是用Controller去切功能模組
                    var permissionOperationMap = HttpRuntime.Cache.GetOrInsert<Dictionary<string, EnumOperation>>(roleKey, () => DictionaryUtils.GetPermissionOperationMap(mvcUser.Roles));

                    userModel.Operation = permissionOperationMap.ContainsKey(controllerName) ?
                        permissionOperationMap[controllerName] : EnumOperation.None;


                }

                base.OnActionExecuted(filterContext);
            }


        }
    }




}
