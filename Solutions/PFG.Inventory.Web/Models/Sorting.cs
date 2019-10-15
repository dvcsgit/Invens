using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PFG.Inventory.Web.Models
{
    public enum SortingDirection
    {
        None,
        Desc,
        Asc
    }

    /// <summary>
    /// 排序欄位 - 帳號
    /// </summary>
    public enum AccountSortingField
    {
        None,
        DateLastLogin, // 上一次登入日期
        DateCreated //新增日期
    }

    /// <summary>
    /// 排序欄位 - 帳號
    /// </summary>
    public enum UploadLogSortingField
    {
        None,
        Account, // 上傳者
        DataUpload //上傳日期
    }
}