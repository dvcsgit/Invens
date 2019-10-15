using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Security;


namespace PFG.Inventory.Web.Library.Authentication
{
    public interface IFormsAuthentication
    {
        /// <summary>
        /// 登出
        /// </summary>
        void Signout();

        /// <summary>
        /// 寫入cookie
        /// </summary>
        /// <param name="httpContext"></param>
        /// <param name="authenticationTicket"></param>
        void SetAuthCookie(HttpContextBase httpContext, FormsAuthenticationTicket authenticationTicket);

        /// <summary>
        /// 寫入cookie
        /// </summary>
        /// <param name="httpContext"></param>
        /// <param name="authenticationTicket"></param>
        void SetAuthCookie(HttpContext httpContext, FormsAuthenticationTicket authenticationTicket);

        /// <summary>
        /// 讀取cookie
        /// </summary>
        /// <param name="encryptedTicket"></param>
        /// <returns></returns>
        FormsAuthenticationTicket Decrypt(string encryptedTicket);
    }
}
