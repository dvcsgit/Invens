//------------------------------------------------------------------------------
// <auto-generated>
//     此代码已从模板生成。
//
//     手动更改此文件可能导致应用程序出现意外的行为。
//     如果重新生成代码，将覆盖对此文件的手动更改。
// </auto-generated>
//------------------------------------------------------------------------------

namespace PFG.Inventory.DataSource
{
    using System;
    using System.Collections.Generic;
    
    public partial class OutBoundInventory
    {
        public long No { get; set; }
        public string BoxNumber { get; set; }
        public string WarehouseID { get; set; }
        public int Location { get; set; }
        public string ProductCode { get; set; }
        public string Class { get; set; }
        public string GrossWeight { get; set; }
        public string NetWeight { get; set; }
        public string CarNo { get; set; }
        public string StatusCode { get; set; }
        public string CreatorAccount { get; set; }
        public Nullable<System.DateTime> DateCreated { get; set; }
        public string ModifierAccount { get; set; }
        public Nullable<System.DateTime> DateModified { get; set; }
        public string UploadAccount { get; set; }
        public Nullable<System.DateTime> DataUpload { get; set; }
        public string PrintFlag { get; set; }
        public string MISFlag { get; set; }
        public Nullable<System.DateTime> MISTime { get; set; }
    }
}
