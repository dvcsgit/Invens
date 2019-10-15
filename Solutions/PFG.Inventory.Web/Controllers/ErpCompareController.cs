using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PFG.Inventory.Web.ViewModels;
using PFG.Inventory.DataSource;
using PFG.Inventory.Web.Library.Enums;
using PFG.Inventory.Web.Library.Filters;
using System.Data.Entity.Core.Objects;

namespace PFG.Inventory.Web.Controllers
{
    [SiteAuthorize]
    public class ErpCompareController : Controller
    {
        // GET: ErpCompare
        [OperationCheck(EnumOperation.Query)]
        public ActionResult Index()
        {
            ERPCompareListViewModels viewModels = new ERPCompareListViewModels();
            viewModels.Parameters = new ERPCompareParametersViewModels() { 
            CurrentDate=DateTime.Now.ToString("yyyy/MM/dd"),
            HistoryDate=DateTime.Now.AddDays(-1).ToString("yyyy/MM/dd")
            };
            return View(viewModels);
        }
        [OperationCheck(EnumOperation.Query)]
        public ActionResult Query (ERPCompareParametersViewModels parameters)
        {
            using (PFGWarehouseSSISEntities db = new PFGWarehouseSSISEntities())
            {
                if (!string.IsNullOrEmpty(parameters.CurrentDate) && !string.IsNullOrEmpty(parameters.HistoryDate))
                {
                    var viewModels = new ERPCompareListViewModels() { 
                    Parameters=parameters
                    };

                    var current = new List<StockViewModels>();
                    if (DateTime.Now.ToString("yyyy/MM/dd") == parameters.CurrentDate)
                    {

                        current = db.MIS_Stock.Where(x => x.STK == parameters.WarehouseID)
                            .GroupBy(x => new
                            {
                                STK = x.STK,
                                PDNO = x.PDNO.Substring(14, 5),
                                GD = x.GD
                            }).AsEnumerable()
                            .Select(x => new StockViewModels()
                            {
                                STK = x.Key.STK,
                                PDNO = x.Key.PDNO,
                                GD = x.Key.GD,
                                QTY = x.Sum(y => y.QTY),
                                WGT = x.Sum(y => y.WGT),
                                ARTS = x.Sum(y => y.ARTS)
                            }).ToList();
                    }
                    else
                    {
                        var beginCurrentDate = Convert.ToDateTime(parameters.CurrentDate);
                        var endCurrentDate = beginCurrentDate.AddDays(1);
                        current = db.MIS_Stock_His.Where(x => x.STK == parameters.WarehouseID&&x.DateHistory>=beginCurrentDate&&x.DateHistory<=endCurrentDate)
                            .GroupBy(x => new
                            {
                                STK = x.STK,
                                PDNO = x.PDNO.Substring(14, 5),
                                GD = x.GD
                            }).AsEnumerable()
                            .Select(x => new  StockViewModels{
                                STK = x.Key.STK,
                                PDNO = x.Key.PDNO,
                                GD = x.Key.GD,
                                QTY = x.Sum(y => y.QTY),
                                WGT = x.Sum(y => y.WGT),
                                ARTS = x.Sum(y => y.ARTS)
                            }).ToList();
                    }
                    var beginHistoryDate = Convert.ToDateTime(parameters.HistoryDate);
                    var endHistoryDate = beginHistoryDate.AddDays(1);
                    var history = db.MIS_Stock_His.Where(x => x.STK == parameters.WarehouseID &&x.DateHistory>=beginHistoryDate&&x.DateHistory<=endHistoryDate)
                                                    .GroupBy(x => new
                                                    {
                                                        STK = x.STK,
                                                        PDNO = x.PDNO.Substring(14, 5),
                                                        GD = x.GD
                                                    }).AsEnumerable()
                            .Select(x => new StockViewModels
                            {
                                STK = x.Key.STK,
                                PDNO = x.Key.PDNO,
                                GD = x.Key.GD,
                                QTY = x.Sum(y => y.QTY),
                                WGT = x.Sum(y => y.WGT),
                                ARTS = x.Sum(y => y.ARTS)
                            }).ToList();
                    var leftOuterJoin = (from left in current
                                         join right in history
                                         on new { ProductCode = left.PDNO, Class = left.GD } equals
                                         new { ProductCode = right.PDNO, Class = right.GD }
                                         into r
                                         from lojoin in r.DefaultIfEmpty(
                                         new StockViewModels
                                         {
                                             STK = default(string),
                                             ARTS = default(decimal?),
                                             GD = default(string),
                                             PDNO = default(string),
                                             QTY = default(decimal?),
                                             WGT = default(decimal?),

                                         })
                                         select new ERPCompareViewModels
                                         {
                                             WarehouseID=left.STK,
                                             ARTS=left.ARTS!=null?(decimal)left.ARTS:0,
                                             ARTS_His=lojoin.ARTS!=null?(decimal)lojoin.ARTS:0,
                                             GD=left.GD,
                                             GD_His=lojoin.GD,
                                             PDNO=left.PDNO,
                                             PDNO_His=lojoin.PDNO,
                                             SumQTY=left.QTY!=null?(decimal)left.QTY:0,
                                             SumQTY_His=lojoin.QTY!=null?(decimal)lojoin.QTY:0,
                                             SumWGT=left.WGT!=null?(decimal)left.WGT:0,
                                             SumWGT_His=lojoin.WGT!=null?(decimal)lojoin.WGT:0,
                                         });
                    var rightOuterJoin = (from right in history
                                          join left in current
                                          on new { ProductCode = right.PDNO, Class = right.GD }
                                          equals new { ProductCode = left.PDNO, Class = left.GD }
                                          into r
                                          from rojoin in r.DefaultIfEmpty(
                                         new StockViewModels
                                         {
                                             STK = default(string),
                                             ARTS = default(decimal?),
                                             GD = default(string),
                                             PDNO = default(string),
                                             QTY = default(decimal?),
                                             WGT = default(decimal?),

                                         })
                                         select new ERPCompareViewModels
                                         {
                                             WarehouseID = right.STK,
                                             ARTS = right.ARTS!=null?(decimal)right.ARTS:0,
                                             ARTS_His = rojoin.ARTS!=null?(decimal)rojoin.ARTS:0,
                                             GD = right.GD,
                                             GD_His = rojoin.GD,
                                             PDNO = right.PDNO,
                                             PDNO_His = rojoin.PDNO,
                                             SumQTY = right.QTY!=null?(decimal)right.QTY:0,
                                             SumQTY_His = rojoin.QTY!=null?(decimal)rojoin.QTY:0,
                                             SumWGT = right.WGT!=null?(decimal)right.WGT:0,
                                             SumWGT_His = rojoin.WGT!=null?(decimal)rojoin.WGT:0
                                         });
                    var result = leftOuterJoin.Union(rightOuterJoin);
                    var ls = new List<ERPCompareViewModels>();
                    foreach(var item in result)
                    {
                        var model = new ERPCompareViewModels() 
                        {
                            WarehouseID = item.WarehouseID,
                            ARTS = item.ARTS,
                            ARTS_His = item.ARTS_His,
                            GD = item.GD,
                            GD_His = item.GD_His,
                            PDNO = item.PDNO,
                            PDNO_His = item.PDNO_His,
                            SumQTY = item.SumQTY,
                            SumQTY_His = item.SumQTY_His,
                            SumWGT = item.SumWGT,
                            SumWGT_His = item.SumWGT_His,
                            Pointer=1
                        };
                        model.DiffARTS =(decimal) (item.ARTS - item.ARTS_His);
                        model.DiffQTY = (decimal)(item.SumQTY - item.SumQTY_His);
                        model.DiffWGT = (decimal)(item.SumWGT - item.SumWGT_His);
                        if (Convert.ToInt32(item.ARTS) != Convert.ToInt32(item.ARTS_His) || (decimal)item.SumQTY != (decimal)item.SumQTY_His || (decimal)item.SumWGT_His!=(decimal)item.SumWGT)
                        {
                            model.Pointer = 0;
                        }
                        ls.Add(model);
                    }
                    viewModels.GridList = ls.OrderBy(x=>x.Pointer).ToList();
                    return PartialView("_GridList",viewModels);
                }
                else
                {
                    return Json(new { errors="請選擇日期"});
                }
            }
        }
    }
}