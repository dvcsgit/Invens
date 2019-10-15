using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PFG.Inventory.DataSource;
using PFG.Inventory.Web.Library.Enums;
using NLog;
using PFG.Inventory.Web.Library.Extensions;

namespace PFG.Inventory.Web.Library
{
    public class DictionaryUtils
    {
        protected static Logger _logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// 取得目前功能有哪些權限
        /// </summary>
        /// <returns></returns>
        public static Dictionary<string, EnumOperation> GetPermissionOperationMap(List<string> roles)
        {
            var result = new Dictionary<string, EnumOperation>();//存到cache
            try
            {
                using (PFGWarehouseEntities db = new PFGWarehouseEntities())
                {
                    //轉換DB裡頭的編號=>EnumOperation
                    var operationEnumMap = db.Operations
                        .ToDictionary(x => x.OperationID, x => (EnumOperation)Enum.Parse(typeof(EnumOperation), x.OperationID));

                    var tempRoles = db.Roles.Where(x => roles.Contains(x.RoleID)).ToList();
                    HashSet<int> permissionHashSet = new HashSet<int>();
                    foreach (var item in tempRoles)
                    {
                        var permissionIdList = item.PermissionOperations
                            .Select(x => x.PermissionOperationID);
                        permissionHashSet.AddRange(permissionIdList);
                    }

                    var permissionOperationList = db.PermissionOperations.Where(x => permissionHashSet.Contains(x.PermissionOperationID)).ToList();
                    foreach (var item in permissionOperationList)
                    {

                        var controllerKey = item.Permissions.Controller;
                        if (string.IsNullOrEmpty(controllerKey)) continue;

                        //沒有URL去區分 =>關掉
                        //if (!string.IsNullOrEmpty(item.PERMISSION.Url))
                        //    controllerKey += item.PERMISSION.Url;

                        if (result.ContainsKey(controllerKey))
                        {
                            var tempData = result[controllerKey];
                            result[controllerKey] = tempData | operationEnumMap[item.OperationID];
                        }
                        else
                        {
                            result.Add(controllerKey, operationEnumMap[item.OperationID]);
                        }
                    }

                }
            }
            catch (Exception ex)
            {

                _logger.Error("發生錯誤:{0}", ex);
            }
            return result;

        }

        /// <summary>
        /// 取得目前頁面名稱對應
        /// </summary>
        /// <returns></returns>
        public static Dictionary<string, string> GetPermissionsNameMap()
        {
            var result = new Dictionary<string, string>();

            using (PFGWarehouseEntities db = new PFGWarehouseEntities())
            {
                //  Controller/Actrion?Url => key Name=> value
                var query = db.Permissions
                    .Where(x => x.Controller != null && x.Action != null)
                    .Select(x => new { x.Controller, x.Action, x.PermissionName })
                    .ToList();

                var stringFormat = "{0}/{1}";
                foreach (var item in query)
                {
                    var key = string.Format(stringFormat, item.Controller, item.Action);
                    if (!result.ContainsKey(key))
                    {
                        result.Add(key, item.PermissionName);
                    }
                }
            }
            return result;
        }
    }
}