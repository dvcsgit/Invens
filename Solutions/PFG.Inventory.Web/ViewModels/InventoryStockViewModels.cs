using PagedList;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace PFG.Inventory.Web.ViewModels
{
    public class InventoryStockViewModelsExtend
    {
        [DisplayName("外倉")]
        public string WarehouseID { get; set; }
        [DisplayName("庫位")]
        public int Location { get; set; }
        [DisplayName("產品批號")]
        public string ProductCode { get; set; }
        [DisplayName("等級")]
        public string Class { get; set; }
        [DisplayName("箱號")]
        public string BoxNumber { get; set; }
        [DisplayName("淨重")]
        public string NetWeight { get; set; }
        [DisplayName("毛重")]
        public string GrossWeight { get; set; }
    }

    public class InventoryStockParametersViewModels:BaseViewModel
    {
        [DisplayName("盤點日")]
        public string DateStock { get; set; }
        [DisplayName("外倉")]
        public string WarehouseID { get; set; }
        [DisplayName("庫位")]
        public int Location { get; set; }
        [DisplayName("異常")]
        public bool IsMakeInventory { get; set; }

        public InventoryStockParametersViewModels()
        {
            this.DateStock = DateTime.Today.ToString("yyyy/MM/dd");
        }
    }

    public class InventoryStockListViewModels
    {
        public IPagedList<InventoryStockViewModelsExtend> GridList { get; set; }
        public InventoryStockParametersViewModels Parameters { get; set; }
    }

}