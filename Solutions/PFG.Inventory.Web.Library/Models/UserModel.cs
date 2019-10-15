using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PFG.Inventory.Web.Library.Enums;

namespace PFG.Inventory.Web.Library.Models
{
    /// <summary>
    /// 到View會有
    /// </summary>
    public class UserModel
    {
        public bool IsUserAuthenticated { get; set; }
        public string UserName { get; set; }
        public string RoleName { get; set; }
        public EnumOperation Operation { get; set; }

    }
}
