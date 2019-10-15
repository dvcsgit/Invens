using PagedList;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PFG.Inventory.Web.ViewModels
{
    public class OutBoundInventoryViewModels
    {
        [DisplayName("主鍵編號")]
        public long No { get; set; }
        [DisplayName("編號")]
        public int Index { get; set; }
        [DisplayName("箱號")]
        public string BoxNumber { get; set; }
        [DisplayName("外倉")]
        [Required(ErrorMessage = "外倉必選")]
        public string WarehouseID { get; set; }
        [DisplayName("庫位")]
        public int Location { get; set; }
        [DisplayName("產品批號")]
        public string ProductCode { get; set; }
        [DisplayName("等級")]
        public string Class { get; set; }
        [DisplayName("毛重")]
        public string GrossWeight { get; set; }
        [DisplayName("淨重")]
        public string NetWeight { get; set; }
        [DisplayName("日期")]
        [Required(ErrorMessage = "日期必選")]
        public DateTime? DateUpload { get; set; }
        [DisplayName("上傳人員")]
        public string UploadAccount { get; set; }
        [DisplayName("車次")]
        public string CarNo { get; set; }
        [Display(Name="是否列印")]
        public string PrintFlag { get; set; }

        /// <summary>
        /// 交運單號
        /// </summary>
        public string Vhno { get; set; }
    }

    public class OutBoundInventoryDistinctViewModels 
    {
        public int Index { get; set; }
        public DateTime? DateUpload { get; set; }
        public string UploadAccount { get; set; }
        public string CarNo { get; set; }

        public long No { get; set; }
        public string PrintFlag { get; set; }
    }

    public class OutBoundInventoryParametersViewModels:BaseViewModel
    {
        [DisplayName("日期")]
        [Required(ErrorMessage="日期必選")]
        public string DateUpload { get; set; }
        [DisplayName("外倉")]
        [Required(ErrorMessage="外倉必選")]
        public string WarehouseID { get; set; }
    }

    public class OutBoundInventoryListViewModels
    {
        public IPagedList<OutBoundInventoryViewModels> GridList { get; set; }
        public IPagedList<OutBoundInventoryDistinctViewModels> DistinctGridList { get; set; }
        public OutBoundInventoryParametersViewModels Parameters { get; set; }
    }
}