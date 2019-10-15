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
    /// 帳號列表 列表
    /// </summary>
    public class AccountListViewModel
    {
        public IPagedList<UserItem> GridList { get; set; }

        public AccountListParameter Parameter { get; set; }
    }

    /// <summary>
    /// 帳號資料 項目
    /// </summary>
    public class UserItem
    {
        [Display(Name = "NotesID")]
        public string Account { get; set; }

        [Display(Name = "Email")]
        public string Email { get; set; }

        [Display(Name = "姓名")]
        public string Name { get; set; }

        [Display(Name = "密碼")]
        public string PasswordHash { get; set; }

        [Display(Name = "上一次登入日期")]
        public DateTime? DateLastLogin { get; set; }

        [Display(Name = "上一次活躍日期")]
        public DateTime? DateLastActivity { get; set; }

        [Display(Name = "電話")]
        public string Tel { get; set; }

        [Display(Name = "分機")]
        public string Ext { get; set; }

        [Display(Name = "新增人員")]
        public string CreatorAccount { get; set; }

        [Display(Name = "新增日期")]
        public DateTime DateCreated { get; set; }

        [Display(Name = "修改人員")]
        public string ModifierAccount { get; set; }

        [Display(Name = "修改日期")]
        public DateTime? DateModified { get; set; }

        [Display(Name = "隸屬角色")]
        public List<string> UserRoles { get; set; }
    }

    /// <summary>
    /// 帳號列表 參數
    /// </summary>
    public class AccountListParameter : BaseViewModel<AccountSortingField>
    {
        [Display(Name = "Notes ID")]
        public string Account { get; set; }

        [Display(Name = "姓名")]
        public string Name { get; set; }
    }

    /// <summary>
    /// 帳號列表 新增&修改
    /// </summary>
    public class CreateOrEditAccountViewModel : BaseCreateOrEditViewModel
    {
        [Display(Name = "NotesID")]
        [Required(ErrorMessage = "{0} 欄位必填")]
        public string Account { get; set; }

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

        public CreateOrEditAccountViewModel()
        {
            UserRoles = new List<string>() { "GUEST" };
        }
    }

    /// <summary>
    /// 帳號列表 詳細
    /// </summary>
    public class DetailAccountViewModel : UserItem
    {
        
    }


}