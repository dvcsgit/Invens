using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NLog;
using PFG.Inventory.Domain.Enums;
using PFG.Inventory.Domain.Services;
using PFG.Inventory.Web.Library;
using PFG.Inventory.Web.Library.Authentication;
using PFG.Inventory.Web.Library.Extensions;
using PFG.Inventory.Web.Library.Models;
using PFG.Inventory.Web.ViewModels;



namespace PFG.Inventory.Web.Controllers
{
    public class LoginController : Controller
    {
        private readonly IFormsAuthentication _formAuthentication;
        private readonly IUserService _userService;
        protected static Logger _logger = LogManager.GetCurrentClassLogger();

        public LoginController(IFormsAuthentication formsAuthentication, IUserService userService)
        {
            this._userService = userService;
            this._formAuthentication = formsAuthentication;
        }

        /// <summary>
        /// 登入頁
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Index()
        {
            var cookieAccout = UserUtils.GetAccount();

            LoginViewModel viewModel = new LoginViewModel()
            {
                Account = cookieAccout,
                RememberMe = !string.IsNullOrEmpty(cookieAccout) ? true : false
            };

            return View(viewModel);
        }

        [HttpPost]
        public JsonResult Index(LoginViewModel model, string returnUrl)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    LogOnStatus status = LogOnStatus.Failure;
                    var user = _userService.GetUser(ref status, model.Account, model.Password);
                    switch (status)
                    {
                        case LogOnStatus.Successful:
                            //note:
                            UserInfo userInfo = new UserInfo
                            {
                                UserId = user.Account,
                                DisplayName = user.Name
                            };

                            //當都沒有權限 則會用GUEST
                            List<string> roles = new List<string>();

                            if (user.Roles.Count == 0)
                            {
                                roles.Add("Guest");
                            }
                            else
                            {
                                roles = user.Roles.Select(x => x.RoleID).ToList();
                            }

                            userInfo.Roles = roles;


                            _formAuthentication.SetAuthCookie(this.HttpContext, UserAuthenticationTicketBuilder.CreateAuthenticationTicket(userInfo));

                            //記住帳號實做
                            if (model.RememberMe)
                            {
                                if (!UserUtils.IsHaveCookie())
                                {

                                    HttpCookie cookie = new HttpCookie(WebSiteConst.RememberCookie);
                                    cookie["account"] = model.Account;
                                    cookie.Expires = DateTime.Now.AddYears(100);
                                    Response.Cookies.Add(cookie);
                                }
                                else
                                {
                                    if (UserUtils.IsNewAccount(model.Account))
                                    {
                                        this.Request.Cookies[WebSiteConst.RememberCookie].Expires = DateTime.Now.AddHours(-1);
                                        Response.Cookies.Add(Request.Cookies[WebSiteConst.RememberCookie]);
                                        HttpCookie cookie = new HttpCookie(WebSiteConst.RememberCookie);
                                        cookie["account"] = model.Account;
                                        cookie.Expires = DateTime.Now.AddYears(100);
                                        Response.Cookies.Add(cookie);
                                    }
                                }
                            }
                            else
                            {
                                if (UserUtils.IsHaveCookie())
                                {
                                    this.Request.Cookies[WebSiteConst.RememberCookie].Expires = DateTime.Now.AddHours(-1);
                                    Response.Cookies.Add(Request.Cookies[WebSiteConst.RememberCookie]);
                                }
                            }

                            var returnUrlBack = returnUrl;

                            if (!Url.IsLocalUrl(returnUrl))
                            {
                                returnUrlBack = Url.Action("Index","Home");
                            }
                            
                            return Json(new { success = true , returnUrl = returnUrlBack });

                        case LogOnStatus.Disabled:
                            ModelState.AddModelError("", "該帳號已被停權");
                            break;
                        case LogOnStatus.Failure:
                            ModelState.AddModelError("", "帳號或密碼錯誤");
                            break;
                        case LogOnStatus.UnActived:
                            ModelState.AddModelError("", "帳號尚未啟用");
                            break;
                        default:
                            ModelState.AddModelError("", "帳號或密碼錯誤");
                            break;
                    }
                }

            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                _logger.Info("Index - Error{0}", ex);
            }
            finally
            {
                _logger.Info("Index - end");
            }


            // If we got this far, something failed
            return Json(new {  errors = ModelState.GetErrors() });

        }

        /// <summary>
        /// 登出
        /// </summary>
        /// <returns></returns>
        public ActionResult LogOff()
        {
            _formAuthentication.Signout();
            return RedirectToAction("Index", "Login");
        }

    }
}

