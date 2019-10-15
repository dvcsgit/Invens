using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PFG.Inventory.Domain.Enums
{
    public enum EnumUploadLogFlag
    {
        /// <summary>
        /// 未處理
        /// </summary>
        None = 0,

        /// <summary>
        /// 成功
        /// </summary>
        Success = 1,

        /// <summary>
        /// 失敗
        /// </summary>
        Fail = 2
        
    }
}
