using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace PFG.Inventory.Web.ViewModels
{
    public class ERPCompareViewModels
    {
        /// <summary>
        /// （基準日）產品批號
        /// </summary>
        public string PDNO { get; set; }
        /// <summary>
        /// （比較日）產品批號
        /// </summary>
        public string PDNO_His { get; set; }
        /// <summary>
        /// (基準日)等級
        /// </summary>
        public string GD { get; set; }
        /// <summary>
        /// （比較日）等級
        /// </summary>
        public string GD_His { get; set; }
        ///<summary>
        ///（基準日）淨重
        ///<summary>
        public decimal SumQTY { get; set; }
        /// <summary>
        /// （基準日）毛重
        /// </summary>
        public decimal SumWGT { get; set; }
        /// <summary>
        /// （基準日）箱數
        /// </summary>
        public decimal ARTS { get; set; }
        /// <summary>
        /// (比較日)淨重
        /// </summary>
        public decimal SumQTY_His { get; set; }
        /// <summary>
        /// （比較日）毛重
        /// </summary>
        public decimal SumWGT_His { get; set; }
        /// <summary>
        /// (比較日)箱數
        /// </summary>
        public decimal ARTS_His { get; set; }
        /// <summary>
        /// (差筆)淨重
        /// </summary>
        public decimal DiffQTY { get; set; }
        /// <summary>
        /// (差筆)毛重
        /// </summary>
        public decimal DiffWGT { get; set; }
        /// <summary>
        /// (差筆)箱數
        /// </summary>
        public decimal DiffARTS { get; set; }
        /// <summary>
        /// 標記比較值是否相等
        /// 相等1，不相等0。
        /// </summary>
        public int Pointer { get; set; }

        public string WarehouseID { get; set; }
    }
    public class ERPCompareParametersViewModels
    {
        [DisplayName("外倉")]
        public string WarehouseID { get; set; }
        [DisplayName("基準日")]
        public string CurrentDate { get; set; }
        [DisplayName("比較日")]
        public string HistoryDate { get; set; }
    }
    public class ERPCompareListViewModels 
    {
        public ERPCompareParametersViewModels Parameters { get; set; }
        public List<ERPCompareViewModels> GridList { get; set; }
    }
}