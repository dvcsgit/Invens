using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;

namespace PFG.Inventory.Web.ViewModels
{
    /// <summary>
    /// 權限群組管理 列表
    /// </summary>
    public class RoleListViewModel
    {
        public RoleListParameter Parameter { get; set; }

        public IPagedList<RoleItem> GridList { get; set; }
    }

    /// <summary>
    /// 權限群組管理 項目
    /// </summary>
    public class RoleItem
    {
        /// <summary>
        /// 群組識別
        /// </summary>
        public string RoleId { get; set; }

        /// <summary>
        /// 群組名稱
        /// </summary>
        public string RoleName { get; set; }

        /// <summary>
        /// 權限群組描述
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 是否為預設
        /// </summary>
        public bool IsDefault { get; set; }

        public IEnumerable<string> PermissionIdList { get; set; }

        public string MainPermission { get; set; }
    }

    /// <summary>
    /// 權限群組管理 參數
    /// </summary>
    public class RoleListParameter : BaseViewModel
    {
        /// <summary>
        /// 群組名稱
        /// </summary>
        [Display(Name = "群組名稱")]
        public string RoleName { get; set; }
    }

    /// <summary>
    /// 權限群組管理 新增&修改
    /// </summary>
    public class CreateOrEditRoleViewModel : BaseCreateOrEditViewModel
    {
        [Display(Name = "群組代碼")]
        [Required(ErrorMessage = "{0}必填")]
        [StringLength(10, ErrorMessage = "{0} 長度 需為 {2}~{1}之間字元 ", MinimumLength = 4)]
        [RegularExpression("^[A-Za-z0-9]+$",ErrorMessage= "{0} 只接受英文與數字")]
        public string RoleId { get; set; }

        /// <summary>
        /// 群組名稱
        /// </summary>
        [Display(Name = "群組名稱")]
        [Required(ErrorMessage = "{0}必填")]
        [StringLength(10, ErrorMessage = "{0} 長度 最多為{2}個字 ")]
        public string RoleName { get; set; }

        [Display(Name = "描述")]
        [StringLength(100, ErrorMessage = "{0} 長度 最多為{2}個字 ")]
        [Required(ErrorMessage = "{0}必填")]
        public string Description { get; set; }

        /// <summary>
        /// 使用者挑的權限
        /// </summary>
        public List<string> PermissionList { get; set; }

        /// <summary>
        /// 拋給前端串接權限的物件 (UI使用)
        /// </summary>
        public Dictionary<string, MenuItem> AllMenu { get; set; }

        public CreateOrEditRoleViewModel()
        {
            this.PermissionList = new List<string>();
        }
    }

    /// <summary>
    /// 權限群組管理 詳細
    /// </summary>
    public class DetailRoleViewModel 
    {
        [Display(Name = "群組代碼")]
        public string RoleId { get; set; }
        [Display(Name = "群組名稱")]
        public string RoleName { get; set; }
        [Display(Name = "描述")]
        public string Description { get; set; }
        /// <summary>
        /// 拋給前端串接權限的物件 (UI使用)
        /// </summary>
        public Dictionary<string, MenuItem> AllMenu { get; set; }
    }


}