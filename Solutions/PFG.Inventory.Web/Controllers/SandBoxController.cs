using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PFG.Inventory.Web.Controllers
{
    public class SandBoxController : Controller
    {
        // GET: SandBox
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Show()
        {
            return View("_Show");
        }
    }
}