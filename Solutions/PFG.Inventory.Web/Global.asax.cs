using System;
using System.Security.Principal;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Security;
using PFG.Inventory.Web.Library.Authentication;
using PFG.Inventory.Web.Library.Models;
using PFG.Inventory.Web.App_Start;
using Newtonsoft.Json.Serialization;


namespace PFG.Inventory.Web
{
    public class MvcApplication : HttpApplication
    {
        public override void Init()
        {
            this.PostAuthenticateRequest += this.PostAuthenticateRequestHandler;
            base.Init();
        }

        private void PostAuthenticateRequestHandler(object sender, EventArgs e)
        {
            HttpCookie authCookie = this.Context.Request.Cookies[FormsAuthentication.FormsCookieName];
            if (isValidAuthCookie(authCookie))
            {
                var formsAuthentication = DependencyResolver.Current.GetService<IFormsAuthentication>();

                var ticket = formsAuthentication.Decrypt(authCookie.Value);
                try
                {
                    var mvcUser = new MVCUser(ticket);
                    //string[] userRoles = { efmvcUser.RoleName };
                    this.Context.User = new GenericPrincipal(mvcUser, null);
                    formsAuthentication.SetAuthCookie(this.Context, ticket);
                }
                catch (Exception ex)
                {
                    //清掉Session重登
                    formsAuthentication.Signout();
                    HttpContext.Current.User = new GenericPrincipal(new GenericIdentity(string.Empty), null);
                }

            }
        }

        private static bool isValidAuthCookie(HttpCookie authCookie)
        {
            return authCookie != null && !String.IsNullOrEmpty(authCookie.Value);
        }

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            GlobalConfiguration.Configuration.Formatters.XmlFormatter.SupportedMediaTypes.Clear();
            GlobalConfiguration.Configuration.Formatters.JsonFormatter.SerializerSettings.ContractResolver = new DefaultContractResolver();
            Bootstrapper.Run();

        }
    }
}
