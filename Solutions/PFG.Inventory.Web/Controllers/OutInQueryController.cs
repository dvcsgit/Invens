using PFG.Inventory.Web.Library.Enums;
using PFG.Inventory.Web.Library.Filters;
using PFG.Inventory.Web.ViewModels;
using PFG.Inventory.DataSource;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using System.ComponentModel;
using PFG.Inventory.Web.Library;

namespace PFG.Inventory.Web.Controllers
{
    [SiteAuthorize]
    public class OutInQueryController : Controller
    {
        // GET: OutInQuery
        [OperationCheck(EnumOperation.Query)]
        public ActionResult Index()
        {
            var viewModels = new InventoryListViewModels();
            viewModels.Parameters = new InventoryParameters();
            viewModels.Parameters.SelectedDate = DateTime.Now.ToString("yyyy/MM/dd");
            return View(viewModels);
        }
        [OperationCheck(EnumOperation.Query|EnumOperation.Export)]
        public ActionResult Query(InventoryParameters parameters,[DefaultValue(false)]bool isExport)
        {
            //if(string.IsNullOrEmpty(parameters.WarehouseInfo))
            //{
            //    return Json(new { errors="請選擇外倉"});
            //}
            //if (parameters.InOrOut == "") 
            //{
            //    return Json(new { errors="請選擇出\\入庫選項"});
            //}
            //if(string.IsNullOrEmpty(parameters.SelectedDate))
            //{
            //    return Json(new { errors="請選擇日期"});
            //}
            var viewModels = new InventoryListViewModels() { 
            Parameters=parameters
            };
            PFGWarehouseEntities db = new PFGWarehouseEntities();
            var query = db.Inventory.Where(x => x.WarehouseID == parameters.WarehouseInfo);
            var selectedDate=Convert.ToDateTime(parameters.SelectedDate);
            var selectedEndDate=selectedDate.AddDays(1);
            switch(parameters.InOrOut)
            {
                case "IN":
                    var resultListIn = query.Where(x => x.DateCreated >= selectedDate && x.DateCreated < selectedEndDate)
                        .Select(x => new InventoryViewModels()
                        {
                            WarehouseInfo = x.WarehouseID,
                            Location = x.Location,
                            ProductCode = x.ProductCode,
                            BoxNumber = x.BoxNumber,
                            Class = x.Class,
                            NetWeight = x.NetWeight,
                            CrossWeight = x.GrossWeight,
                            EnterFlag = x.Remark
                        }).AsQueryable();
                        viewModels.GridList = resultListIn.OrderBy(x=>x.Location).ToPagedList(parameters.PageNo, parameters.PageSize);
                        if (isExport)
                        {
                            //List<int> outCol = new List<int>() { 8};
                            var desFilePath = resultListIn.ExportExcel("外倉(" + parameters.WarehouseInfo + ")入庫信息.xls", "外倉入庫信息", parameters.WarehouseInfo, null, null);
                            var url = Url.Action("Download", "Utils", new { @fullFilePath = desFilePath });
                            return Json(new { success = true, url = url });
                        }
                    break;
                case "OUT":
                         var resultListOut = query.Where(x => x.StatusCode == "O" && x.DateModified >= selectedDate && x.DateModified < selectedEndDate)
                            .Select(
                            x => new InventoryViewModels()
                            {
                                WarehouseInfo = x.WarehouseID,
                                Location = x.Location,
                                ProductCode = x.ProductCode,
                                BoxNumber = x.BoxNumber,
                                Class = x.Class,
                                NetWeight = x.NetWeight,
                                CrossWeight = x.GrossWeight,
                                EnterFlag = x.Remark
                            }
                            ).AsQueryable();
                        viewModels.GridList = resultListOut.OrderBy(x => x.Location).ToPagedList(parameters.PageNo,parameters.PageSize);
                        if (isExport)
                        {
                            //List<int> outCol = new List<int>() { 8};
                            var desFilePath = resultListOut.ExportExcel("外倉(" + parameters.WarehouseInfo + ")出庫信息.xls", "外倉出庫信息", parameters.WarehouseInfo, null, null);
                            var url = Url.Action("Download", "Utils", new { @fullFilePath = desFilePath });
                            return Json(new { success = true, url = url });
                        }
                    break;
                default:
                    viewModels.GridList = null;
                    return Json(new { errors="請選擇出\\入庫選項"});
            }

            return PartialView("_GridList",viewModels);
        }
    }
}