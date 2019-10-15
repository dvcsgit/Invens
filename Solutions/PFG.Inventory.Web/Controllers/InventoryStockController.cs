using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PFG.Inventory.Web.Library.Filters;
using PFG.Inventory.Web.Library.Enums;
using PFG.Inventory.Web.ViewModels;
using System.ComponentModel;
using PFG.Inventory.DataSource;
using PFG.Inventory.Web.Library;
using PagedList;
using System.Data.Entity.SqlServer;

namespace PFG.Inventory.Web.Controllers
{
    [SiteAuthorize]
    public class InventoryStockController : Controller
    {
        // GET: InventoryStock
        [OperationCheck(EnumOperation.Query|EnumOperation.Export)]
        public ActionResult Index(InventoryStockParametersViewModels parameters,[DefaultValue(false)]bool isExport)
        {
            InventoryStockListViewModels viewModel = new InventoryStockListViewModels() { 
            Parameters=parameters
            };
            try
            {
                string dateStock = parameters.DateStock.Substring(0,4)+parameters.DateStock.Substring(5,2)+parameters.DateStock.Substring(8,2);
                using(PFGWarehouseEntities db=new PFGWarehouseEntities())
                {
                    var query = db.InventoryStock.Where(x => x.DateStock == dateStock).AsQueryable();
                    if(!string.IsNullOrEmpty(parameters.WarehouseID))
                    {
                        query = query.Where(x => x.WarehouseID == parameters.WarehouseID);
                    }
                    if(parameters.Location!=0)
                    {
                        query = query.Where(x => x.Location == parameters.Location);
                    }
                    if(parameters.IsMakeInventory)
                    {
                        query = query.Where(x => string.IsNullOrEmpty(x.MakeInventory));
                    }
                    var resultList = query.Select(x => new InventoryStockViewModelsExtend 
                    {
                      BoxNumber=x.BoxNumber,
                      Class=x.Class,
                      GrossWeight=x.GrossWeight,
                      Location=x.Location,
                      NetWeight=x.NetWeight,
                      ProductCode=x.ProductCode,
                      WarehouseID=x.WarehouseID
                    });
                    viewModel.GridList = resultList.OrderBy(x => x.WarehouseID).OrderBy(x => SqlFunctions.StringConvert((decimal?)x.Location))
                            .OrderBy(x => x.ProductCode).OrderBy(x => x.Class).ToPagedList(parameters.PageNo, parameters.PageSize);
                    if (isExport)
                    {
                        //List<string> titleList = new List<string>() { "外倉","庫位","產品批號","等級","箱號","淨重","毛重"};
                        var desFilePath = resultList.ToList().OrderBy(x => x.WarehouseID).OrderBy(x => x.Location)
                            .OrderBy(x => x.ProductCode).OrderBy(x => x.Class).ExportExcel(dateStock + "盤點查詢信息.xls", "外倉盤點查詢信息");
                        var url = Url.Action("Download", "Utils", new { @fullFilePath = desFilePath });
                        return Json(new { success = true, url = url });
                    }
                    if (Request.IsAjaxRequest())
                    {
                        return PartialView("_GridList", viewModel);
                    }
                    return View(viewModel);
                }
            }
            catch
            {
                return Json(new { success = false, errors = "伺服器錯誤" });
            }
        }
    }
}