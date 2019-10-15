using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PFG.Inventory.DataSource;
using PFG.Inventory.Web.Library.Enums;
using PFG.Inventory.Web.Library.Extensions;
using PFG.Inventory.Web.Library;
using PFG.Inventory.Web.ViewModels;


namespace PFG.Inventory.Web.Library
{
    /// <summary>
    /// 權限功能
    /// </summary>
    public class MenuUtils
    {
        /// <summary>
        /// 取得目前登入帳號有那些使用權限
        /// </summary>
        /// <param name="httpContext"></param>
        /// <returns></returns>
        private static List<Permissions> _getPermission(HttpContext httpContext)
        {
            var result = new List<Permissions>();
            // 取得使用者的資料
            var mvcUser = httpContext.User.GetMVCUser();
            var roleIDList = mvcUser.Roles;
            using (PFGWarehouseEntities db = new PFGWarehouseEntities())
            {
                //取出目前使用者的群組
                //列出群組所擁有那些權限 (去重覆)
                //因為是組選單 應該只要有就permission就要列出選單

                var tempRolePermissionList = db.Roles.Where(x => roleIDList.Contains(x.RoleID)).ToList();
                HashSet<Permissions> tempPermissionHashSet = new HashSet<Permissions>();
                //將所有的Permission取出來
                foreach (var item in tempRolePermissionList)
                {
                    var permission = item.PermissionOperations.Select(x => x.Permissions).ToList();
                    tempPermissionHashSet.AddRange(permission);
                }

                //取得根節點
                var parentIdList = tempPermissionHashSet.Select(x => x.ParentID).Distinct().ToList();
                var rootPermision = db.Permissions.Where(x => parentIdList.Contains(x.PermissionID))
                    .OrderByDescending(x => x.Weight)
                    .ToList();

                tempPermissionHashSet.AddRange(rootPermision);
                result = tempPermissionHashSet.OrderBy(x => x.ParentID).ToList();
                return result;
            }
        }

        /// <summary>
        /// view選單使用 會依照目前登入的使用者取出該使用者能使用的選單link
        /// 上面選單使用與sitemap
        /// </summary>
        /// <param name="httpContext"></param>
        /// <returns></returns>
        public static Dictionary<string, MenuItem> GetSubMenu(HttpContext httpContext)
        {
            var menuMap = new Dictionary<string, MenuItem>();
            var permissionList = _getPermission(httpContext);
            var request = httpContext.Request;
            string currentController = request.RequestContext.RouteData.Values["controller"].ToString();
            string currentAction = request.RequestContext.RouteData.Values["action"].ToString();
            string activeLinkId = "";
            string activeLinkSubId = "";
            //使用上面的值去找目前在那一個選單下面
            var currentLink = permissionList.Where(x => x.Controller == currentController && x.Action == currentAction).FirstOrDefault();
            if (currentLink != null)
            {
                activeLinkId = string.IsNullOrEmpty(currentLink.ParentID) ? currentLink.PermissionID : currentLink.ParentID;
                activeLinkSubId = currentLink.PermissionID;
            }

            //這邊邊組的時候直接去問 treeInfo 因該物件為Dictionary類 使用索引值去找 所耗時間為常數時間 極快
            foreach (var item in permissionList)
            {
                MenuItem child = new MenuItem()
                {
                    MenuId = item.PermissionID,
                    Name = item.PermissionName,
                    ParentId = item.ParentID,
                    Controller = item.Controller,
                    ClassName = activeLinkId == item.PermissionID ? "active open" : "",
                    Action = item.Action,
                    Weight = item.Weight,
                    Icon = item.Icon
                };

                if (activeLinkSubId == item.PermissionID)
                {
                    child.ClassName = "active open";
                }

                if (menuMap.ContainsKey(item.PermissionID))
                {
                    continue;
                }
                else
                {
                    //check要加在那
                    if (string.IsNullOrEmpty(item.ParentID))
                    {
                        menuMap.Add(item.PermissionID, child);//根節點
                    }
                    else
                    {
                        menuMap[item.ParentID].Childrens.Add(child);//根節點底下子節點
                    }

                }
            }
            return menuMap;

        }

        /// <summary>
        /// exten 取選單
        /// </summary>
        /// <returns></returns>
        public static Dictionary<string, MenuItem> GetSubMenu()
        {
            return GetSubMenu(string.Empty);
        }

        /// <summary>
        /// 取選單 依據群組 取出勾選的
        /// 群組管理使用
        /// </summary>
        /// <param name="roleId">權限ID</param>
        /// <returns></returns>
        public static Dictionary<string, MenuItem> GetSubMenu(string roleId)
        {
            using (PFGWarehouseEntities db = new PFGWarehouseEntities())
            {
                Dictionary<string, MenuItem> menuMap = new Dictionary<string, MenuItem>();
                var rolePermissionOperationMap = new Dictionary<string, EnumOperation>();//存放目前使用者勾選的權限

                if (!string.IsNullOrEmpty(roleId))
                {
                    //目前群組所擁有的權限
                    var rolePermissionOperationIdList = db.Roles
                        .Where(x => x.RoleID == roleId)
                        .Select(x => x.PermissionOperations).ToList();
                    foreach (var item in rolePermissionOperationIdList)
                    {
                        foreach (var subItem in item)
                        {
                            var enumFunction = EnumExtensions<EnumOperation>.Parse(subItem.OperationID);

                            if (rolePermissionOperationMap.ContainsKey(subItem.PermissionID))
                            {
                                var tempData = rolePermissionOperationMap[subItem.PermissionID];
                                rolePermissionOperationMap[subItem.PermissionID] = tempData | enumFunction;

                            }
                            else
                            {
                                //not exist in map    
                                rolePermissionOperationMap.Add(subItem.PermissionID, enumFunction);
                            }

                        }
                    }
                }

                List<Permissions> permissionList = db.Permissions
                    .OrderBy(x => x.ParentID).ThenBy(x => x.PermissionID).ToList();

                //loop 所有功能權限
                foreach (var item in permissionList)
                {
                    //檢查使用者能否使用
                    var isChecked = false;
                    MenuItem child = new MenuItem()
                    {
                        MenuId = item.PermissionID,
                        Name = item.PermissionName,
                        ParentId = item.ParentID,
                        Controller = item.Controller,
                        Action = item.Action,
                        Weight = item.Weight,
                        Icon = item.Icon
                    };

                    //MenuOperations 檢查該功能權限有那些操作動作
                    List<MenuOperation> menuOperations = new List<MenuOperation>();
                    // 確認勾勾
                    foreach (var subItem in item.PermissionOperations)
                    {
                        var enumFunction = EnumExtensions<EnumOperation>.Parse(subItem.OperationID);
                        var subItemChecked = false;
                        if (rolePermissionOperationMap.ContainsKey(subItem.PermissionID))
                        {
                            subItemChecked = rolePermissionOperationMap[subItem.PermissionID].HasFlag(enumFunction);
                        }

                        menuOperations.Add(new MenuOperation
                        {
                            PermissionOperationId = subItem.PermissionID + "-" + subItem.OperationID,
                            ChineseName = subItem.Operations.Name,
                            OperationId = subItem.Operations.OperationID,
                            OperationName = subItem.Operations.Name,
                            IsChecked = subItemChecked
                        });
                    }
                    child.MenuOperations = menuOperations;


                    if (menuMap.ContainsKey(item.PermissionID))
                    {
                        continue;
                    }
                    else
                    {
                        //check要加在那
                        if (string.IsNullOrEmpty(item.ParentID))
                        {
                            menuMap.Add(item.PermissionID, child);
                        }
                        else
                        {
                            menuMap[item.ParentID].Childrens.Add(child);
                        }

                    }

                }
                return menuMap;

            }
        }

        /// <summary>
        /// 取得當前頁面的標題
        /// </summary>
        /// <param name="httpContext"></param>
        /// <returns></returns>
        public static string GetPageTitle(HttpContext httpContext)
        {
            var result = string.Empty;
            var request = httpContext.Request;
            string currentController = request.RequestContext.RouteData.Values["controller"].ToString();
            string currentAction = request.RequestContext.RouteData.Values["action"].ToString();
            var tempQueryString = request.QueryString.ToString();


            var getPermissionsNameMap = HttpRuntime.Cache.GetOrInsert<Dictionary<string, string>>("getPermissionsNameMap", DictionaryUtils.GetPermissionsNameMap);
            if (getPermissionsNameMap != null)
            {
                var stringFormat = "{0}/{1}";
                string currentUrl = string.Format(stringFormat, currentController, currentAction);

                result = getPermissionsNameMap.ContainsKey(currentUrl) ? getPermissionsNameMap[currentUrl] : string.Empty;

            }

            return result;
        }
    }
}