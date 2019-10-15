using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PFG.Inventory.Web.Models
{
    public class OutBoundInventoryQuery
    {
        public string VhNo { get; set; }
        public string TrDp { get; set; }
        public string TroDat { get; set; }
        public string TriDat { get; set; }
        public string TriStk { get; set; }
        public string TroStk { get; set; }
        public Int64 IT { get; set; }
        public string PdId { get; set; }
        public string PDNO { get; set; }
        public string Gd { get; set; }
        public string Un { get; set; }
        public int Qty { get; set; }
        public int Wgt { get; set; }
        public int Arts { get; set; }
    }
}