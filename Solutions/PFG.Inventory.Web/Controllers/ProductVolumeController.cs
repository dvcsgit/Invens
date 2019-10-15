using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PFG.Inventory.DataSource;
using PFG.Inventory.Web.ViewModels;
using PagedList;
using PFG.Inventory.Web.Library.Filters;
using PFG.Inventory.Web.Library.Enums;
using PFG.Inventory.Web.Library.Models;
using System.Data.Entity;
using PFG.Inventory.Web.Library.Extensions;

namespace PFG.Inventory.Web.Controllers
{
    [SiteAuthorize]
    public class ProductVolumeController : Controller
    {
        // GET: ProductVolume
        [OperationCheck(EnumOperation.Query)]
        public ActionResult Index(BasicSettingCapacityParameters parameters)
        {
            var viewModels = new BasicSettingCapacityListViewModels
            {
                Parameters = parameters
            };
            using (PFGWarehouseEntities db = new PFGWarehouseEntities())
            {
                var query = db.BasicSettingCapacity.Select(x => new BasicSettingCapacityViewModels()
                {
                    CapacityProduct = x.CapacityProduct,
                    CreatorAccount = x.CreatorAccount,
                    DateCreated = x.DateCreated,
                    DateModified = x.DateModified,
                    ModifierAccount = x.ModifierAccount,
                    ProductCode = x.ProductCode
                });
                viewModels.GridList = query.OrderByDescending(x=>x.DateCreated).ToPagedList(parameters.PageNo, parameters.PageSize);
                if(Request.IsAjaxRequest())
                {
                    return PartialView("_GridList",viewModels);
                }
                return View(viewModels);
            }
        }
        [OperationCheck(EnumOperation.Create)]
        public ActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [OperationCheck(EnumOperation.Create)]
        public ActionResult Create(BasicSettingCapacityViewModels viewModels)
        {
            if (ModelState.IsValid)
            {
                var userModel = User.GetMVCUser();
                

                using (PFGWarehouseEntities db = new PFGWarehouseEntities())
                {
                    var query = db.BasicSettingCapacity.Where(x => x.ProductCode == viewModels.ProductCode.ToUpper()).Count();
                    if(query>0)
                    {
                        return Json(new { success=false,errors="已存在該產品批號，不能重複輸入！"});
                    }
                    BasicSettingCapacity volume = new BasicSettingCapacity()
                    {
                        CapacityProduct = viewModels.CapacityProduct,
                        CreatorAccount = userModel.UserId,
                        DateCreated = DateTime.Now,
                        ProductCode = viewModels.ProductCode.ToUpper()
                    };
                    db.Entry<BasicSettingCapacity>(volume).State = EntityState.Added;
                    db.SaveChanges();
                    return Json(new { success=true});
                }
            }
            return Json(new { success=false,errors=ModelState.GetErrors()});
        }
        [HttpGet]
        [OperationCheck(EnumOperation.Delete)]
        public ActionResult Delete(string productCode)
        {
            using (PFGWarehouseEntities db=new PFGWarehouseEntities())
            {
                var willDelete = db.BasicSettingCapacity.Single(x => x.ProductCode == productCode);
                db.Entry<BasicSettingCapacity>(willDelete).State = EntityState.Deleted;
                db.SaveChanges();
                return Json(new { success = true }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpGet]
        [OperationCheck(EnumOperation.Edit)]
        public ActionResult Edit(string productCode)
        {
            BasicSettingCapacityViewModels viewModel = new BasicSettingCapacityViewModels();
            using (PFGWarehouseEntities db = new PFGWarehouseEntities())
            {
                var query = db.BasicSettingCapacity.Single(x => x.ProductCode == productCode);
                viewModel.CapacityProduct = query.CapacityProduct;
                viewModel.ProductCode = query.ProductCode;
                return View(viewModel);
            }
        }

        [HttpPost]
        public ActionResult Save(BasicSettingCapacityViewModels viewModels)
        {
            if (ModelState.IsValid)
            {
                using (PFGWarehouseEntities db = new PFGWarehouseEntities())
                {
                    var query = db.BasicSettingCapacity.Single(x => x.ProductCode == viewModels.ProductCode);
                    query.CapacityProduct = viewModels.CapacityProduct;
                    var userID = User.GetMVCUser().UserId;
                    query.DateModified = DateTime.Now;
                    query.ModifierAccount = userID;
                    db.Entry<BasicSettingCapacity>(query).State = EntityState.Modified;
                    db.SaveChanges();
                    return Json(new { success = true });
                }
            }
            return Json(new { errors=ModelState.GetErrors()});
        }
    }
}