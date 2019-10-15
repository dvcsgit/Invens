using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Web.Security;
using PFG.Inventory.Web.Library.Enums;

namespace PFG.Inventory.Web.Library.Models
{
    [Serializable]
    public class MVCUser : IIdentity
    {

        public MVCUser() { }
        public MVCUser(string name, string displayName, string userId, string roleName)
        {
            this.Name = name;
            this.DisplayName = displayName;
            this.UserId = userId;
            this.RoleName = roleName;
        }

        public MVCUser(string name, UserInfo userInfo)
        {
            if (userInfo == null) throw new ArgumentNullException("userInfo");
            this.Name = name;
            this.DisplayName = userInfo.DisplayName;
            this.UserId = userInfo.UserId;
            this.RoleName = userInfo.RoleName;
            this.Roles = userInfo.Roles;
            // this.PermissionOperation = userInfo.PermissionOperation;
        }

        /// <summary>
        /// web global use
        /// </summary>
        /// <param name="ticket"></param>
        public MVCUser(FormsAuthenticationTicket ticket)
            : this(ticket.Name, UserInfo.FromString(ticket.UserData))
        {
            if (ticket == null) throw new ArgumentNullException("ticket");
        }

        public string AuthenticationType
        {
            get { return "MVCForms"; }
        }

        public bool IsAuthenticated
        {
            get { return true; }
        }

        public string Name { get; private set; }
        public string DisplayName { get; private set; }
        public string RoleName { get; private set; }
        public List<string> Roles { get; private set; }
        public string UserId { get; private set; }
        public Dictionary<string, EnumOperation> PermissionOperation { get; set; }
    }
}
