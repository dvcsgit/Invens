using PagedList;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace PFG.Inventory.Web.ViewModels
{
    public class CompareViewModels
    {

        public string STK { get; set; }
        [DisplayName("產品批號")]
        public string PDNO { get; set; }
        [DisplayName("等級")]
        public string GD { get; set; }
        [DisplayName("淨重")]
        public decimal? SumQTY { get; set; }
        [DisplayName("毛重")]
        public decimal? SumWGT { get; set; }
        [DisplayName("箱數")]
        public decimal? SumARTS { get; set; }

        public string WarehouseID { get; set; }
        [DisplayName("產品批號")]
        public string ProductCode { get; set; }
        [DisplayName("等級")]
        public string Class { get; set; }
        [DisplayName("淨重")]
        public decimal? SumGrossWeight { get; set; }
        [DisplayName("毛重")]
        public decimal? SumNetWeight { get; set; }
        [DisplayName("箱數")]
        public decimal? BoxCount { get; set; }
        /// <summary>
        /// 標記淨重、毛重、箱數是否相同
        /// 相同標記為1，不相同標記為0
        /// </summary>
        public int Pointer { get; set; }
        /// <summary>
        /// （差筆）毛重
        /// </summary>
        public decimal DiffGrossWeight { get; set; }
        /// <summary>
        /// （差筆）淨重
        /// </summary>
        public decimal DiffNetWeight { get; set; }
        public decimal DiffBoxCount { get; set; }
    }

    public class StockViewModels
    {
        [DisplayName("外倉")]
        public string STK { get; set; }
        [DisplayName("產品批號")]
        public string PDNO { get; set; }
        [DisplayName("等級")]
        public string GD { get; set; }
        [DisplayName("淨重")]
        public decimal? QTY { get; set; }
        [DisplayName("毛重")]
        public decimal? WGT { get; set; }
        [DisplayName("箱數")]
        public decimal? ARTS { get; set; }
    }

    public class CompareParametersViewModels:BaseViewModel
    {
        [DisplayName("外倉")]
        public string WarehouseID { get; set; }
    }
    public class CompareListViewModels
    {
        public List<CompareViewModels> GridList { get; set; }
        public IPagedList<StockViewModels> StockGridList { get; set; }
        public IPagedList<InventoryViewModels> InventoryGridList { get; set; }
        public CompareParametersViewModels Parameters { get; set; }
    }
}