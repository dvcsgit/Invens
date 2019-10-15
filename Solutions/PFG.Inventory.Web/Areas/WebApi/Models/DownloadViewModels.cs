using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PFG.Inventory.Web.Areas.WebApi.Models
{
    public class DownloadViewModel
    {
        public List<BasicSettingLoactionItem> BasicSettingLoaction { get; set; }

        public List<InventoryItem> Inventory { get; set; }

        public DownloadViewModel()
        {
            this.BasicSettingLoaction = new List<BasicSettingLoactionItem>();
            this.Inventory = new List<InventoryItem>();
        }
    }

    public class BasicSettingLoactionItem
    {
        [MaxLength(10)]
        public string WarehouseID { get; set; }

        public int Location { get; set; }
    }

    public class InventoryItem
    {
        [MaxLength(10)]
        public string WarehouseID { get; set; }

        public int Location { get; set; }

        [MaxLength(7)]
        public string Remark { get; set; }
        
        [MaxLength(7)]
        public string BoxNumber { get; set; }

        [MaxLength(10)]
        public string ProductCode { get; set; }

        [MaxLength(2)]
        public string Class { get; set; }

        [MaxLength(10)]
        public string GrossWeight { get; set; }

        [MaxLength(10)]
        public string NetWeight { get; set; }

        [MaxLength(50)]
        public string CarNo { get; set; }

        [MaxLength(20)]
        public string CreatorAccount { get; set; }

        public string DateCreated { get; set; }

        [MaxLength(2)]
        public string StatusCode { get; set; }

        [MaxLength(20)]
        public string ModifierAccount { get; set; }

        public string DateModified { get; set; }

        /// <summary>
        /// 盤點使用 Y/N
        /// </summary>
        [MaxLength(1)]
        public string MakeInventory { get; set; }

        /// <summary>
        /// 盤點使用 盤點日期 前端實際盤點 存檔的時間 (應該會跟DateCreated 一樣)
        /// </summary>
        public string DateStockTime { get; set; }
        
    }
}