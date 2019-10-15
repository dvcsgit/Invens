using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using PFG.Inventory.Web.Library.Filters;

namespace PFG.Inventory.Web.Controllers
{
    [SiteAuthorize]
    public class HomeController : Controller
    {
        public ActionResult Index()
        {

            return View();
        }
    }
}
