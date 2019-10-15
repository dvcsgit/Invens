using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using PagedList;
using PFG.Inventory.DataSource;
using PFG.Inventory.Web.Library.Enums;
using PFG.Inventory.Web.Models;


namespace PFG.Inventory.Web.ViewModels
{
    /// <summary>
    /// 上傳日誌 列表
    /// </summary>
    public class UploadLogListViewModel
    {
        public IPagedList<UploadLogItem> GridList { get; set; }

        public UploadLogListParameter Parameter { get; set; }
    }

    /// <summary>
    /// 上傳日誌 項目
    /// </summary>
    public class UploadLogItem
    {
        public string UploadLogID { get; set; }

        [Display(Name = "上傳者")]
        public string Account { get; set; }

        [Display(Name = "上傳檔案名稱")]
        public string UploadFileName { get; set; }

        [Display(Name = "解壓後暫存檔案位置")]
        public string TempFileName { get; set; }

        [Display(Name = "上傳時間")]
        public DateTime DataUpload { get; set; }

        [Display(Name = "處理狀態")]
        public int Flag { get; set; }

        [Display(Name = "電話")]
        public string Tel { get; set; }

        [Display(Name = "錯誤訊息")]
        public string ExceptionMessage { get; set; }

        [Display(Name = "資料筆數")]
        public int TotalRecords { get; set; }

        /// <summary>
        /// 區分 入出庫 還是盤點
        /// </summary>
        public string ControllerName { get; set; }

        [Display(Name = "摘要")]
        public string Summary { get; set; }

    }

    /// <summary>
    /// 帳號列表 參數
    /// </summary>
    public class UploadLogListParameter : BaseViewModel<UploadLogSortingField>
    {
        [Display(Name = "上傳者")]
        public string Account { get; set; }

        [Display(Name = "上傳時間起")]
        public DateTime? DataUploadStart { get; set; }

        public DateTime? DataUploadEnd { get; set; }
    }

    /// <summary>
    /// 帳號列表 新增&修改
    /// </summary>
    public class CreateOrEditUploadLogViewModel : BaseCreateOrEditViewModel
    {
        [Display(Name = "NotesID")]
        [Required(ErrorMessage = "{0} 欄位必填")]
        public string UploadLog { get; set; }

        [Display(Name = "Email")]
        public string Email { get; set; }

        [Display(Name = "姓名")]
        [Required(ErrorMessage = "{0} 欄位必填")]
        public string Name { get; set; }

        
        [Display(Name = "密碼")]
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "{0} 欄位必填")]
        public string PasswordHash { get; set; }

        [Display(Name = "確認密碼")]
        [DataType(DataType.Password)]
        [Compare("PasswordHash", ErrorMessage = "密碼與確認新密碼不符合")]
        public string ConfirmPassword { get; set; }

        [Display(Name = "隸屬角色")]
        [Required(ErrorMessage = "{0} 欄位必填")]
        public List<string> UserRoles { get; set; }

        public CreateOrEditUploadLogViewModel()
        {
            UserRoles = new List<string>() { "GUEST" };
        }
    }

    /// <summary>
    /// 帳號列表 詳細
    /// </summary>
    public class DetailUploadLogViewModel : UserItem
    {
        
    }


}