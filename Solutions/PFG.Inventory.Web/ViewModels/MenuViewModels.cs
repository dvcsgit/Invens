using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PFG.Inventory.Web.ViewModels
{
    /// <summary>
    /// 選單
    /// </summary>
    public class MenuItem 
    {
        /// <summary>
        /// 識別
        /// </summary>
        [Display(Name = "權限代號")]
        public string MenuId { get; set; }

        /// <summary>
        /// 父節點
        /// </summary>
        [Display(Name = "父節點")]
        public string ParentId { get; set; }

        /// <summary>
        /// 名稱
        /// </summary>
        [Display(Name = "權限名稱")]
        public string Name { get; set; }

        public string Controller { get; set; }

        public string Action { get; set; }

        [Display(Name = "權重")]
        public int Weight { get; set; }

        public bool IsChecked { get; set; }

        [Display(Name = "在新視窗開啟")]
        public bool OpenInNewWindow { get; set; }

        [Display(Name = "樣式名稱")]
        public string Icon { get; set; }

        public List<MenuItem> Childrens { get; set; }

        public string Url { get; set; }

        /// <summary>
        /// UI 使用 無法進行編輯
        /// </summary>
        public string ClassName { get; set; }

        /// <summary>
        /// 該選單能操作的項目
        /// </summary>
        public List<MenuOperation> MenuOperations { get; set; }

        public MenuItem()
        {
            this.Childrens = new List<MenuItem>();
            this.MenuOperations = new List<MenuOperation>();
        }
    }

    public class MenuOperation
    {
        public string OperationId { get; set; }

        public string OperationName { get; set; }

        public string ChineseName { get; set; }

        public bool IsChecked { get; set; }

        public string PermissionOperationId { get; set; }
    }

    /// <summary>
    /// 麵包使用
    /// </summary>
    public class BreadcrumbItem
    {
        public string Name { get; set; }

        public string Controller { get; set; }

        public string Action { get; set; }

        public string ClassName { get; set; }
    }

    public class CreateOrEditMenuViewModel : MenuItem
    {
        /// <summary>
        /// 路徑
        /// </summary>
        public string Breadcrumb { get; set; }

    }
}