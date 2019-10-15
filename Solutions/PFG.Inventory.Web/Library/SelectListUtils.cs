using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PFG.Inventory.DataSource;

namespace PFG.Inventory.Web.Library
{
    /// <summary>
    /// 提供下拉選單類
    /// </summary>
    public class SelectListUtils
    {
        /// <summary>
        /// 角色選單 多選
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<SelectListItem> GetRoleOptions()
        {
            List<SelectListItem> items = new List<SelectListItem>();
            using (PFGWarehouseEntities db = new PFGWarehouseEntities())
            {
                items = db.Roles.Select(x => new SelectListItem() { Text = x.RoleName, Value = x.RoleID }).ToList();
            }
            return items.AsEnumerable();
        }
        /// <summary>
        /// 外倉選擇
        /// </summary>
        /// <returns>外倉選項數據集</returns>
        public static IEnumerable<SelectListItem> GetWarehouseOptions()
        {
            List<SelectListItem> result = new List<SelectListItem>();
            result.Add(new SelectListItem() { Text = "", Value = "" });
            using (PFGWarehouseEntities db=new PFGWarehouseEntities())
            {
                result.AddRange ( db.BasicSettingWarehouse.Select(x => new SelectListItem() { Text = x.WarehouseName + "(" + x.WarehouseID + ")", Value = x.WarehouseID }).ToList());
                
            }
            return result;
        }
        /// <summary>
        /// 根據外倉選擇庫位
        /// </summary>
        /// <param name="houseId">外倉代號</param>
        /// <returns>庫位選項數據集</returns>
        public static IEnumerable<SelectListItem> GetLocationOptions(string houseId)
        {
            var items = new List<SelectListItem>();
            items.Add(new SelectListItem()
            {
                Text = "",
                Value = ""
            });
            using ( PFGWarehouseEntities db=new PFGWarehouseEntities())
            {
                 items .AddRange( db.BasicSettingLoaction.Where(x => x.WarehouseID == houseId)
                    .Select(x => new SelectListItem() { 
                    Text=x.Location.ToString(),
                    Value=x.Location.ToString()
                    }).ToList());
                
            }
            return items;
        }
        /// <summary>
        /// 產品批號選擇
        /// </summary>
        /// <param name="houseId">外倉代號</param>
        /// <param name="location">庫位（可以為null）</param>
        /// <returns>產品批號選項數據集</returns>
        public static IEnumerable<SelectListItem> GetProductCodeOptions(string houseId, string location)
        {
            var items = new List<SelectListItem>();
            items.Add(new SelectListItem()
            {
                Text = "",
                Value = ""
            });
            using (PFGWarehouseEntities db = new PFGWarehouseEntities())
            {
                if (location != null && location != "")
                {
                    int loc = int.Parse(location);
                     items.AddRange(db.Inventory.Where(x => x.WarehouseID == houseId && x.Location == loc)
                        .Select(x => new SelectListItem()
                        {
                            Text = x.ProductCode,
                            Value = x.ProductCode
                        }).Distinct().AsEnumerable());
                    
                }
                else
                {
                   items.AddRange( db.Inventory.Where(x => x.WarehouseID == houseId)
                        .Select(x => new SelectListItem()
                        {
                            Text = x.ProductCode,
                            Value = x.ProductCode
                        }).Distinct().AsEnumerable());
                    
                }

            }
            return items;
        }
        /// <summary>
        /// 根據產品批號產生產品等級
        /// </summary>
        /// <param name="productCode">產品批號</param>
        /// <returns>等級選項數據集</returns>
        public static IEnumerable<SelectListItem> GetClassOptions(string productCode)
        {
            var items = new List<SelectListItem>();
            items.Add(new SelectListItem()
            {
                Text = "",
                Value = ""
            });
            using(PFGWarehouseEntities db=new PFGWarehouseEntities())
            {
                items.AddRange(db.Inventory.Where(x => x.ProductCode == productCode)
                     .Select(x => new SelectListItem()
                     {
                         Text = x.Class,
                         Value = x.Class
                     }).Distinct().AsEnumerable());
                
            }
            return items;
        }

        /// <summary>
        /// 外倉庫存查詢——出入庫查詢
        /// 出入庫選項
        /// </summary>
        /// <returns>出入庫選項的集合</returns>
        public static IEnumerable<SelectListItem> GetInOrOutWarehouse()
        {
            var items = new List<SelectListItem>();
            items.Add(new SelectListItem() { Text="",Value=""});
            items.Add(new SelectListItem() { Text="入庫",Value="IN"});
            items.Add(new SelectListItem() { Text="出庫",Value="OUT"});
            return items;
        }
    }
}