using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PFG.Inventory.Web.Library;
using PFG.Inventory.Web.ViewModels;

namespace PFG.Inventory.Web.Controllers
{
    public class SelectListUtilsController : Controller
    {
        [HttpPost]
        public JsonResult GetLocationsByWarehouse(string warehouseId)
        {
            
            var itemsLocation = SelectListUtils.GetLocationOptions(warehouseId);
            var itemsProductCode = SelectListUtils.GetProductCodeOptions(warehouseId, null);
            var items = new { location=itemsLocation,code=itemsProductCode};
            return Json(items);

        }
        [HttpPost]
        public JsonResult GetProductCode(string warehouseId,string location)
        {
            var items = SelectListUtils.GetProductCodeOptions(warehouseId, location);
            return Json(items);
        }
        [HttpPost]
        public JsonResult GetClassByPcode(string code)
        {
            var items = SelectListUtils.GetClassOptions(code);
            return Json(items);
        }
    }
}