using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace PFG.Inventory.Web.Library.Enums
{
    /// <summary>
    /// 匯出報表格式
    /// </summary>
    public enum EnumFormat
    {
        [Description("無")]
        None,

        [Description("PDF")]
        PDF,

        [Description("Excel")]
        Excel,

        [Description("Tif圖檔")]
        Image

    }
}
