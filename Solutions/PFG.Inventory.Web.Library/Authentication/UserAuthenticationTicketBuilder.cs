using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Security;
using PFG.Inventory.Web.Library.Enums;
using PFG.Inventory.Web.Library.Models;

namespace PFG.Inventory.Web.Library.Authentication
{
    public class UserAuthenticationTicketBuilder
    {
        public static FormsAuthenticationTicket CreateAuthenticationTicket(UserInfo user)
        {
            var ticket = new FormsAuthenticationTicket(
                1,
                user.UserId,
                DateTime.Now,
                DateTime.Now.Add(FormsAuthentication.Timeout),
                false,
                user.ToString());

            return ticket;
        }


    }
}
