using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PFG.Inventory.Web.ViewModels
{
    public class BasicSettingLocationViewModels
    {
        [DisplayName("外倉編號")]
        [Required(ErrorMessage="請輸入{0}")]
        public string WarehouseID { get; set; }
        [DisplayName("庫位")]
        [Required(ErrorMessage="請輸入{0}")]
        public string Location { get; set; }
    }
}