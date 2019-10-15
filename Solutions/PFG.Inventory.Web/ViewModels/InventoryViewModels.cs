using PagedList;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PFG.Inventory.Web.ViewModels
{
    public class InventoryViewModels
    {
        /// <summary>
        /// 包含WarehouseName(WarehouseID)
        /// </summary>
        [DisplayName("外倉")]
        [Required(ErrorMessage="外倉為必選項")]
        public string WarehouseInfo { get; set; }
        [DisplayName("庫位")]
        public int? Location { get; set; }
        [DisplayName("箱號")]
        [Required(ErrorMessage="箱號為必要項")]
        [MaxLength(7,ErrorMessage="箱號不能超過7個字符")]
        public string BoxNumber { get; set; }
        [DisplayName("產品批號")]
        [Required(ErrorMessage = "{0}為必要項")]
        [MaxLength(10,ErrorMessage = "{0}不能超過10個字符")]
        public string ProductCode { get; set; }
        [DisplayName("等級")]
        public string Class { get; set; }
        [DisplayName("淨增")]
        [Required(ErrorMessage = "{0}為必要項")]
        public string NetWeight { get; set; }
        [DisplayName("毛重")]
        [Required(ErrorMessage = "{0}為必要項")]
        public string CrossWeight { get; set; }

        [DisplayName("輸入註記")]
        //[Required(ErrorMessage = "{0}為必要項")]
        [MaxLength(10, ErrorMessage = "{0}不能超過10個字符")]
        public string EnterFlag { get; set; }

        public string OldBoxNumber { get; set; }
    }

    public class InventoryParameters:BaseViewModel
    {
        /// <summary>
        /// 包含WarehouseName(WarehouseID)
        /// </summary>
        [DisplayName("外倉")]
        [Required(ErrorMessage = "外倉為必選項")]
        public string WarehouseInfo { get; set; }
        //public string WarehouseID { get; set; }
        [DisplayName("庫位")]
        public int? Location { get; set; }
        [DisplayName("產品批號")]
        public string ProductCode { get; set; }
        [DisplayName("等級")]
        public string Class { get; set; }
        /// <summary>
        /// 出入庫（入庫為true，出庫為false）
        /// </summary>
        [DisplayName("入庫\\出庫")]
        public string InOrOut { get; set; }
        /// <summary>
        /// 日期
        /// 日期：提供使用者選擇日期，若入庫/出庫有選擇，則此欄必選
        /// 當使用者選擇入庫及日期時，則條件如右：Select * From Inventory where DateCreated=”選擇的日期”
        /// 當使用者選擇出庫及日期時，則條件如右：Select * From Inventory where StatusCode=’O’ and DateModified=”選擇的日期”
        /// </summary>
        [DisplayName("日期")]
        public string SelectedDate { get; set; }
        [DisplayName("輸入註記")]
        public string EnterFlag { get; set; }
        [DisplayName("輸入註記")]
        public bool IsPointer { get; set; }
    }

    public class InventoryListViewModels
    {
        public IPagedList<InventoryViewModels> GridList { get; set; }
        /// <summary>
        /// 查詢參數
        /// </summary>
        public InventoryParameters Parameters { get; set; }
        /// <summary>
        /// 容積率（數據部分）
        /// </summary>
        public string CapacityPercentage { get; set; }
        /// <summary>
        /// 淨重
        /// </summary>
        public decimal SumNetWeight { get; set; }
        /// <summary>
        /// 毛重
        /// </summary>
        public decimal SumGrossWeight { get; set; }
    }
}