using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PFG.Inventory.Web.ViewModels
{
    public class BasicSettingWarehouseListViewModels
    {
        /// <summary>
        /// 外倉集合
        /// </summary>
        public List<BasicSettingWarehouseViewModels> ListViewModels { get; set; }
    }
    public class BasicSettingWarehouseViewModels
    {
        [DisplayName("外倉編號")]
        [Required(ErrorMessage="請輸入{0}")]
        public string WarehouseID { get; set; }
        [DisplayName("外倉名稱")]
        [Required(ErrorMessage="請輸入{0}")]
        public string WarehouseName { get; set; }
        [DisplayName("容量")]
        [RegularExpression(@"^[0-9]*[1-9][0-9]*$", ErrorMessage = "您輸入的不是正整數！")]
        public int? Capacity { get; set; }
        public List<BasicSettingLocationViewModels> Locations { get; set; }
        [DisplayName("庫位數")]
        [Required(ErrorMessage="請輸入{0}")]
        [RegularExpression(@"^[0-9]*[1-9][0-9]*$",ErrorMessage="您輸入的不是正整數！")]
        public int? SumLocations { get; set; }
        [DisplayName("啟用")]
        public bool IsEnabled { get; set; }
    }
}