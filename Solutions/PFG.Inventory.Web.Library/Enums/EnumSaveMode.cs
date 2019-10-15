using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace PFG.Inventory.Web.Library.Enums
{
    /// <summary>
    /// 判斷是新增還編輯
    /// </summary>
    [Flags]
    public enum EnumSaveMode
    {
        /// <summary>
        /// Create
        /// </summary>
        [Description("新增")]
        Create,

        /// <summary>
        /// Edit
        /// </summary>
        [Description("編輯")]
        Update

    }
}
