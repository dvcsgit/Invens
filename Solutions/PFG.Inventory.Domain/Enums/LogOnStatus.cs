using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PFG.Inventory.Domain.Enums
{
    public enum LogOnStatus
    {
        // 尚未驗證郵件地址
        NotActivated,
        // 登入成功
        Successful,
        // 登入失敗
        Failure,
        // 已停用
        Disabled,
        // 未啟用
        UnActived,
        // 不存在
        NotExist,
        // 密碼錯誤
        FailPassword
    }
}
