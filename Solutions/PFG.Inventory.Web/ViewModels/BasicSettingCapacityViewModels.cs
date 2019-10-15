using PagedList;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PFG.Inventory.Web.ViewModels
{
    public class BasicSettingCapacityViewModels
    {
        [DisplayName("產品批號")]
        [Required(ErrorMessage="{0}不能為空！")]
        [StringLength(2,ErrorMessage="長度只能為2"),MinLength(2)]
        [RegularExpression("^[A-Za-z0-9]+$", ErrorMessage = "只能輸入英文字符或數字")]
        public string ProductCode { get; set; }
        [DisplayName("產品體積")]
        //[Required(ErrorMessage="{0}不能為空！")]
        [RegularExpression(@"^(\d|9)(?:\.\d{1,2})?$", ErrorMessage = "只能0-10的數(兩位小數)")]
        //^(\d|10)(\.\d)?$
        //[RegularExpression(@"^0\.[1-9]\d*$", ErrorMessage = "不能輸入大於1或小於0的數")]
        public decimal? CapacityProduct { get; set; }
        [DisplayName("創建者")]
        public string CreatorAccount { get; set; }
        [DisplayName("創建日期")]
        public DateTime DateCreated { get; set; }
        [DisplayName("修改者")]
        public string ModifierAccount { get; set; }
        [DisplayName("修改日期")]
        public DateTime? DateModified { get; set; }
    }

    public class BasicSettingCapacityListViewModels
    {
        public IPagedList<BasicSettingCapacityViewModels> GridList { get; set; }
        public BasicSettingCapacityParameters Parameters { get; set; }
    }

    public class BasicSettingCapacityParameters:BaseViewModel
    {
        //public string ProductCode { get; set; }
        //public double CapracityProduct { get; set; }
        //public BasicSettingCapacityParameters() 
        //{
        //    this.CapracityProduct = Math.Round(this.CapracityProduct,2);
        //}
    }

    public class CapacityViewModels
    {
        public decimal? Capacity { get; set; }
        public string ProductCode { get; set; }
    }
}