using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using Newtonsoft.Json;
using NLog;
using PFG.Inventory.Web.Library.Models;

namespace PFG.Inventory.Web.Library
{
    public class UserUtils
    {
        protected static Logger _logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// 取得使用者目前身份別(Controller使用)
        /// </summary>
        public static UserInfo GetUserInfo(HttpContextBase httpContext)
        {
            // 用法：GetUserInfo userData = UserUtils.GetUserInfo(this.HttpContext);

            // 尚未通過驗証
            if (!httpContext.User.Identity.IsAuthenticated) return null;

            try
            {
                // 取得使用者的群組定義
                string cookieValue = httpContext.Request.Cookies[FormsAuthentication.FormsCookieName].Value;
                UserInfo userData = JsonConvert.DeserializeObject<UserInfo>(FormsAuthentication.Decrypt(cookieValue).UserData);
                return userData;
            }
            catch
            {
                return null;
            }
        }


        /// <summary>
        /// 取回使用者資料
        /// </summary>
        /// <returns></returns>
        public static UserInfo GetUserInfo()
        {
            try
            {
                string cookieValue = HttpContext.Current.Request.Cookies[FormsAuthentication.FormsCookieName].Value;
                UserInfo userData = JsonConvert.DeserializeObject<UserInfo>(FormsAuthentication.Decrypt(cookieValue).UserData);
                return userData;
            }
            catch (Exception ex)
            {
                _logger.Error("發生錯誤:{0}", ex.Message);
                return new UserInfo();
            }
        }

        /// <summary>
        /// 取得使用者目前資料(View使用)
        /// </summary>
        public static UserInfo GetUserInfo(HttpContext httpContext)
        {
            // 用法：UserInfo userData = UserUtils.GetUserInfo(HttpContext.Current);

            UserInfo userDataModel = new UserInfo();

            // 尚未通過驗証
            if (!httpContext.User.Identity.IsAuthenticated) return null;

            try
            {
                // 取得使用者的資料
                string cookieValue = httpContext.Request.Cookies[FormsAuthentication.FormsCookieName].Value;
                userDataModel = JsonConvert.DeserializeObject<UserInfo>(FormsAuthentication.Decrypt(cookieValue).UserData);

                return userDataModel;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 取出帳號(記住帳號時用)
        /// </summary>
        /// <returns></returns>
        public static String GetAccount()
        {
            try
            {

                string cookieValue = HttpContext.Current.Request.Cookies[WebSiteConst.RememberCookie].Values["account"];
                return cookieValue;
            }
            catch
            {
                return "";
            }
        }



        public static Boolean IsHaveCookie()
        {
            bool result = true;
            try
            {
                string cookie = HttpContext.Current.Request.Cookies[WebSiteConst.RememberCookie].Values["account"];
                if (string.IsNullOrEmpty(cookie))
                {
                    result = false;
                }
            }
            catch
            {
                result = false;
            }
            return result;
        }

        public static Boolean IsNewAccount(string account)
        {
            bool result = false;
            try
            {
                string cookie = HttpContext.Current.Request.Cookies[WebSiteConst.RememberCookie].Values["account"];
                if (account != cookie)
                    result = true;
            }
            catch
            {
                result = true;
            }
            return result;
        }



    }
}