using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.UI.WebControls;

namespace PFG.Inventory.Web.ViewModels
{
    public class LoginCookieViewModel
    {
        [Display(Name ="NOTES ID")]
        [Required]
        public string Account { get; set; }


        [Required(ErrorMessage = "Email is required")]
        [MaxLength(200)]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
    }

    /// <summary>
    /// 登入
    /// </summary>
    public class LoginViewModel
    {
        [Display(Name = "Notes ID")]
        [Required(ErrorMessage = "{0}必填")]
        public string Account { get; set; }

        [Display(Name = "密碼")]
        [Required(ErrorMessage = "{0}必填")]
        public string Password { get; set; }

        public bool RememberMe { get; set; }
    }


    /// <summary>
    /// 更換密碼
    /// </summary>
    public class ChangePasswordViewModel
    {
        [Required(ErrorMessage = "{0}必填")]
        [DataType(DataType.Password)]
        [Display(Name = "原密碼")]
        public string OldPassword { get; set; }

        [Required(ErrorMessage = "{0}必填")]
        [StringLength(100, ErrorMessage = "{0} 長度至少需為 {2} 個字元 ", MinimumLength = 4)]
        [DataType(DataType.Password)]
        [Display(Name = "新密碼")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "確認密碼")]
        [Compare("NewPassword", ErrorMessage = "新密碼與確認新密碼不符合")]
        public string ConfirmPassword { get; set; }
    }
}