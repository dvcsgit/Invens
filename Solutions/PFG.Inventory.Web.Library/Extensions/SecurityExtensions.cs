using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using PFG.Inventory.Web.Library.Models;

namespace PFG.Inventory.Web.Library.Extensions
{
    public static class SecurityExtensions
    {
        public static string Name(this IPrincipal user)
        {
            return user.Identity.Name;
        }

        public static bool InAnyRole(this IPrincipal user, params string[] roles)
        {
            foreach (string role in roles)
            {
                if (user.IsInRole(role)) return true;
            }
            return false;
        }
        public static MVCUser GetMVCUser(this IPrincipal principal)
        {
            if (principal.Identity is MVCUser)
                return (MVCUser)principal.Identity;
            else
                return new MVCUser(string.Empty, new UserInfo());
        }
    }
}
