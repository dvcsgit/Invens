using PFG.Inventory.DataSource;
using PFG.Inventory.Web.Library.Enums;
using PFG.Inventory.Web.Library.Filters;
using PFG.Inventory.Web.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PFG.Inventory.Web.Library.Extensions;
namespace PFG.Inventory.Web.Controllers
{
    [SiteAuthorize]
    public class WarehouseController : Controller
    {
        // GET: Warehouse
        [OperationCheck(EnumOperation.Query)]
        public ActionResult Index()
        {

            using (PFGWarehouseEntities db = new PFGWarehouseEntities())
            {
                var viewModel = new BasicSettingWarehouseListViewModels();
                var result = from house in db.BasicSettingWarehouse
                             join location in db.BasicSettingLoaction
                             on house.WarehouseID equals location.WarehouseID
                             into hl
                             select new BasicSettingWarehouseViewModels()
                             {
                                 WarehouseID = house.WarehouseID,
                                 WarehouseName = house.WarehouseName,
                                 Capacity = house.Capacity,
                                 SumLocations = hl.Count()

                             };
                //var query = db.BasicSettingWarehouse.Select(x => new BasicSettingWarehouseViewModels() {
                //    WarehouseID=x.WarehouseID,
                //    WarehouseName=x.WarehouseName,
                //    Capacity=x.Capacity
                //}).ToList();
                viewModel.ListViewModels = result.ToList();
                return View(viewModel);
            }
        }
        [OperationCheck(EnumOperation.Edit)]
        public ActionResult Edit(string id)
        {

            using (PFGWarehouseEntities db = new PFGWarehouseEntities())
            {
                var result = (from house in db.BasicSettingWarehouse
                              join location in db.BasicSettingLoaction
                              on house.WarehouseID equals location.WarehouseID
                              into hl
                              select new BasicSettingWarehouseViewModels()
                              {
                                  WarehouseID = house.WarehouseID,
                                  WarehouseName = house.WarehouseName,
                                  Capacity = house.Capacity,
                                  SumLocations = hl.Count(),
                                  IsEnabled=house.IsEnabled
                              }).Single(x => x.WarehouseID == id);
                return View(result);
            }
        }

        [HttpPost]
        [OperationCheck(EnumOperation.Delete|EnumOperation.Create|EnumOperation.Edit)]
        public ActionResult Save(BasicSettingWarehouseViewModels house)
        {
            if (ModelState.IsValid)
            {
                try
                {

                    using (PFGWarehouseEntities db = new PFGWarehouseEntities())
                    {
                        var entity = db.BasicSettingWarehouse.Single(x => x.WarehouseID == house.WarehouseID);
                        entity.Capacity = house.Capacity;
                        entity.IsEnabled = house.IsEnabled;
                        db.Entry<BasicSettingWarehouse>(entity).State = EntityState.Modified;
                        db.SaveChanges();
                        int sum = db.BasicSettingLoaction.Where(x => x.WarehouseID == house.WarehouseID).Count();
                        if (sum < house.SumLocations)
                        {
                            //var locations = new List<BasicSettingLoaction>();
                            int itemSum = sum;
                            for (int i=0; i < house.SumLocations-sum; i++) 
                            {
                                itemSum+=1;
                                var locations = new BasicSettingLoaction()
                                {
                                    WarehouseID = house.WarehouseID,
                                    Location = itemSum
                                };
                                db.Entry<BasicSettingLoaction>(locations).State = EntityState.Added;
                            }
                            db.SaveChanges();
                        }
                        if (sum > house.SumLocations)
                        {
                            var locations = db.BasicSettingLoaction.Where(x => x.Location > house.SumLocations&&x.WarehouseID==house.WarehouseID).AsQueryable();
                            db.BasicSettingLoaction.RemoveRange(locations);
                            //foreach (var item in locations)
                            //{
                            //    db.BasicSettingLoaction.Attach(item);
                            //    db.Entry<BasicSettingLoaction>(item).State = EntityState.Deleted;
                            //}
                            db.SaveChanges();


                        }

                        db.SaveChanges();
                        return Json(new { success = true, redirect = Url.Action("Index") });
                    }
                }
                catch
                {
                    return Json(new { success = false,errors=ModelState.GetErrors() });
                }
            }
            return Json(new { errors = ModelState.GetErrors() });
        }
        
        [HttpGet]
        [OperationCheck(EnumOperation.Edit|EnumOperation.Create)]
        public ActionResult Create() 
        {
            return View();
        }

        [HttpPost]
        [OperationCheck(EnumOperation.Create|EnumOperation.Edit)]
        public ActionResult Create(BasicSettingWarehouseViewModels house)
        {
            if (ModelState.IsValid)
            {
                using (PFGWarehouseEntities db = new PFGWarehouseEntities())
                {
                    BasicSettingWarehouse newHouse = new BasicSettingWarehouse()
                    {
                        WarehouseID = house.WarehouseID,
                        WarehouseName = house.WarehouseName,
                        Capacity = house.Capacity,
                        IsEnabled=true
                    };
                    db.Entry<BasicSettingWarehouse>(newHouse).State = EntityState.Added;
                    if (house.SumLocations != null)
                    {
                        for (int i = 1; i <= house.SumLocations; i++)
                        {
                            BasicSettingLoaction locationEntity = new BasicSettingLoaction()
                            {
                                WarehouseID = house.WarehouseID,
                                Location = i
                            };
                            db.Entry<BasicSettingLoaction>(locationEntity).State = EntityState.Added;
                        }
                    }
                    db.SaveChanges();
                    return Json(new { success=true,redirect=Url.Action("Index")});
                }
            }
            return Json(new { success = false,errors=ModelState.GetErrors() });
        }

        [HttpGet]
        [OperationCheck(EnumOperation.Delete)]
        public ActionResult Delete(string id)
        {
            try
            {
                using (PFGWarehouseEntities db = new PFGWarehouseEntities())
                {
                    var warehouse = db.BasicSettingWarehouse.SingleOrDefault(x => x.WarehouseID == id);
                    db.BasicSettingWarehouse.Attach(warehouse);
                    db.Entry<BasicSettingWarehouse>(warehouse).State = EntityState.Deleted;
                    db.SaveChanges();
                    return Json(new { success = true, redirect = Url.Action("Index") },JsonRequestBehavior.AllowGet);
                }
            }
            catch
            {
                return Json(new { success = false, errors = "資料庫連接或操作失敗！" },JsonRequestBehavior.AllowGet);
            }
            
        }
    }
}