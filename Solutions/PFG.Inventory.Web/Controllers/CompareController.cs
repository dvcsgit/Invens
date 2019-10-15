using PFG.Inventory.Web.Library.Enums;
using PFG.Inventory.Web.Library.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PFG.Inventory.Web.ViewModels;
using PFG.Inventory.DataSource;
using PagedList;

namespace PFG.Inventory.Web.Controllers
{
    [SiteAuthorize]
    public class CompareController : Controller
    {
        // GET: Compare
        [OperationCheck(EnumOperation.Query)]
        public ActionResult Index()
        {
            return View();
        }
        [OperationCheck(EnumOperation.Query)]
        [HttpPost]
        public ActionResult Query(CompareParametersViewModels parameters)
        {
            var viewModel = new CompareListViewModels()
            {
                Parameters = parameters
            };
            PFGWarehouseEntities db = new PFGWarehouseEntities();
            PFGWarehouseSSISEntities dbSSIS = new PFGWarehouseSSISEntities();
            if (!string.IsNullOrEmpty(parameters.WarehouseID))
            {
                var query = db.Inventory.Where(x => x.WarehouseID == parameters.WarehouseID && x.StatusCode != "O")
                    .GroupBy(
                    x => new
                    {
                        WarehouseID = x.WarehouseID,
                        ProductCode = x.ProductCode.Trim(),
                        Class = x.Class.Trim()
                    }).AsEnumerable()
                    .Select(
                    x => new
                    {



                        WarehouseID = x.Key.WarehouseID,
                        ProductCode = x.Key.ProductCode,
                        Class = x.Key.Class,
                        CrossWeight = (decimal?)x.Sum(y => decimal.Parse(y.GrossWeight)),
                        NetWeight = (decimal?)x.Sum(y => decimal.Parse(y.NetWeight)),
                        BoxCount = (decimal?)x.Count()
                    }
                    );
                var querySSIS = dbSSIS.MIS_Stock.Where(x => x.STK == parameters.WarehouseID).GroupBy(
                    x => new
                    {
                        STK = x.STK,
                        PDNO = x.PDNO.Substring(14, 5),
                        GD = x.GD.Trim()
                    }).AsEnumerable().Select(
                    x => new
                    {
                        STK = x.Key.STK,
                        ARTS = x.Sum(y => y.ARTS),
                        GD = x.Key.GD,
                        PDNO = x.Key.PDNO,
                        QTY = x.Sum(y => y.QTY),
                        WGT = x.Sum(y => y.WGT)
                    });
                var leftOuterJoin = (from left in query
                                     join right in querySSIS
                                     on new { ProductCode = left.ProductCode, Class = left.Class } equals
                                     new { ProductCode = right.PDNO, Class = right.GD }
                                     into r
                                     from lojoin in r.DefaultIfEmpty(
                                     new
                                     {
                                         //WarehouseID=default(string),
                                         //ProductCode = default(string),
                                         //Class = default(string),
                                         //SumGrossWeight=default(decimal?),
                                         //SumNetWeight=default(decimal?),
                                         //BoxCount=default(int),
                                         STK = default(string),
                                         ARTS = default(decimal?),
                                         GD = default(string),
                                         PDNO = default(string),

                                         QTY = default(decimal?),
                                         WGT = default(decimal?),

                                     })
                                     select new
                                     {
                                         WarehouseID = left.WarehouseID,
                                         ProductCode = left.ProductCode,
                                         Class = left.Class,
                                         SumGrossWeight = (decimal?)left.CrossWeight,
                                         SumNetWeight = (decimal?)left.NetWeight,
                                         BoxCount = (decimal?)left.BoxCount,
                                         STK = lojoin.STK,
                                         PDNO = lojoin.PDNO,
                                         GD = lojoin.GD,
                                         SumQTY = (decimal?)lojoin.QTY,
                                         SumWGT = (decimal?)lojoin.WGT,
                                         SumARTS = (decimal?)lojoin.ARTS

                                     });
                var rightOuterJoin = (from right in querySSIS
                                      join left in query
                                      on new { ProductCode = right.PDNO, Class = right.GD }
                                      equals new { ProductCode = left.ProductCode, Class = left.Class }
                                      into r
                                      from rojoin in r.DefaultIfEmpty(
                                      new
                                      {
                                          WarehouseID = default(string),
                                          ProductCode = default(string),
                                          Class = default(string),
                                          CrossWeight = default(decimal?),
                                          NetWeight = default(decimal?),
                                          BoxCount = default(decimal?),

                                      })
                                      select
                                      new
                                      {
                                          WarehouseID = rojoin.WarehouseID,

                                          ProductCode = rojoin.ProductCode,
                                          Class = rojoin.Class,
                                          SumGrossWeight = (decimal?)rojoin.CrossWeight,
                                          SumNetWeight = (decimal?)rojoin.NetWeight,
                                          BoxCount = (decimal?)rojoin.BoxCount,
                                          STK = right.STK,
                                          PDNO = right.PDNO,
                                          GD = right.GD,
                                          SumQTY = (decimal?)right.QTY,
                                          SumWGT = (decimal?)right.WGT,
                                          SumARTS = (decimal?)right.ARTS

                                      });



                var result = leftOuterJoin.Union(rightOuterJoin);
                var ls = new List<CompareViewModels>();
                foreach (var item in result)
                {
                    var compare = new CompareViewModels()
                    {
                        BoxCount = item.BoxCount,
                        Class = item.Class,
                        GD = item.GD,
                        PDNO = item.PDNO,
                        Pointer = 1,
                        ProductCode = item.ProductCode,
                        STK = item.STK,
                        SumARTS = item.SumARTS,
                        SumGrossWeight = item.SumGrossWeight,
                        SumNetWeight = item.SumNetWeight,
                        SumQTY = item.SumQTY,
                        SumWGT = item.SumWGT,
                        WarehouseID = string.IsNullOrEmpty(item.WarehouseID)?item.STK:item.WarehouseID
                    };
                    compare.DiffBoxCount = (decimal)((item.SumARTS.HasValue ? item.SumARTS : 0) - (item.BoxCount.HasValue ? item.BoxCount : 0));
                    compare.DiffGrossWeight = (decimal)((item.SumWGT.HasValue ? item.SumWGT : 0) - (item.SumGrossWeight.HasValue ? item.SumGrossWeight : 0));
                    compare.DiffNetWeight = (decimal)((item.SumQTY.HasValue ? item.SumQTY : 0) - (item.SumNetWeight.HasValue ? item.SumNetWeight : 0));
                    if (Convert.ToInt32(item.SumARTS) != Convert.ToInt32(item.BoxCount) || item.SumQTY != item.SumNetWeight || item.SumGrossWeight != item.SumWGT)
                    {
                        compare.Pointer = 0;
                    }

                    ls.Add(compare);
                }
                viewModel.GridList = ls.OrderBy(x => x.Pointer).ToList();
                //viewModel.GridList = ls.OrderByDescending(x=>x.Pointer).ToPagedList(parameters.PageNo, parameters.PageSize);
                return PartialView("_GridList", viewModel);
            }
            else
            {
                var query = db.Inventory.Where(x=> x.StatusCode != "O").GroupBy(
                            x => new
                            {
                                WarehouseID = x.WarehouseID,
                                ProductCode = x.ProductCode.Trim(),
                                Class = x.Class.Trim()
                            }).AsEnumerable()
                              .Select(
                              x => new
                              {



                                  WarehouseID = x.Key.WarehouseID,
                                  ProductCode = x.Key.ProductCode,
                                  Class = x.Key.Class,
                                  CrossWeight = (decimal?)x.Sum(y => decimal.Parse(y.GrossWeight)),
                                  NetWeight = (decimal?)x.Sum(y => decimal.Parse(y.NetWeight)),
                                  BoxCount = (decimal?)x.Count()
                              });
                var querySSIS = dbSSIS.MIS_Stock.GroupBy(
                    x => new
                    {
                        STK = x.STK,
                        PDNO = x.PDNO.Substring(14, 5),
                        GD = x.GD.Trim()
                    }).AsEnumerable().Select(
                    x => new
                    {
                        STK = x.Key.STK,
                        ARTS = x.Sum(y => y.ARTS),
                        GD = x.Key.GD,
                        PDNO = x.Key.PDNO,
                        QTY = x.Sum(y => y.QTY),
                        WGT = x.Sum(y => y.WGT)
                    });
                var leftOuterJoin = (from left in query
                                     join right in querySSIS
                                     on new { ProductCode = left.ProductCode, Class = left.Class } equals
                                     new { ProductCode = right.PDNO, Class = right.GD }
                                     into r
                                     from lojoin in r.DefaultIfEmpty(
                                     new
                                     {
                                         //WarehouseID=default(string),
                                         //ProductCode = default(string),
                                         //Class = default(string),
                                         //SumGrossWeight=default(decimal?),
                                         //SumNetWeight=default(decimal?),
                                         //BoxCount=default(int),
                                         STK = default(string),
                                         ARTS = default(decimal?),
                                         GD = default(string),
                                         PDNO = default(string),

                                         QTY = default(decimal?),
                                         WGT = default(decimal?),

                                     })
                                     select new
                                     {
                                         WarehouseID = left.WarehouseID,
                                         ProductCode = left.ProductCode,
                                         Class = left.Class,
                                         SumGrossWeight = (decimal?)left.CrossWeight,
                                         SumNetWeight = (decimal?)left.NetWeight,
                                         BoxCount = (decimal?)left.BoxCount,
                                         STK = lojoin.STK,
                                         PDNO = lojoin.PDNO,
                                         GD = lojoin.GD,
                                         SumQTY = (decimal?)lojoin.QTY,
                                         SumWGT = (decimal?)lojoin.WGT,
                                         SumARTS = (decimal?)lojoin.ARTS

                                     });
                var rightOuterJoin = (from right in querySSIS
                                      join left in query
                                      on new { ProductCode = right.PDNO, Class = right.GD }
                                      equals new { ProductCode = left.ProductCode, Class = left.Class }
                                      into r
                                      from rojoin in r.DefaultIfEmpty(
                                      new
                                      {
                                          WarehouseID = default(string),
                                          ProductCode = default(string),
                                          Class = default(string),
                                          CrossWeight = default(decimal?),
                                          NetWeight = default(decimal?),
                                          BoxCount = default(decimal?),

                                      })
                                      select
                                      new
                                      {
                                          WarehouseID = rojoin.WarehouseID,

                                          ProductCode = rojoin.ProductCode,
                                          Class = rojoin.Class,
                                          SumGrossWeight = (decimal?)rojoin.CrossWeight,
                                          SumNetWeight = (decimal?)rojoin.NetWeight,
                                          BoxCount = (decimal?)rojoin.BoxCount,
                                          STK = right.STK,
                                          PDNO = right.PDNO,
                                          GD = right.GD,
                                          SumQTY = (decimal?)right.QTY,
                                          SumWGT = (decimal?)right.WGT,
                                          SumARTS = (decimal?)right.ARTS

                                      });



                var result = leftOuterJoin.Union(rightOuterJoin);
                var ls = new List<CompareViewModels>();
                foreach (var item in result)
                {
                    var compare = new CompareViewModels()
                    {
                        BoxCount = item.BoxCount,
                        Class = item.Class,
                        GD = item.GD,
                        PDNO = item.PDNO,
                        Pointer = 1,
                        ProductCode = item.ProductCode,
                        STK = item.STK,
                        SumARTS = item.SumARTS,
                        SumGrossWeight = item.SumGrossWeight,
                        SumNetWeight = item.SumNetWeight,
                        SumQTY = item.SumQTY,
                        SumWGT = item.SumWGT,
                        //WarehouseID = string.IsNullOrEmpty(item.WarehouseID) ? item.STK : item.WarehouseID
                        WarehouseID=item.WarehouseID
                    };
                    compare.DiffBoxCount = (decimal)((item.SumARTS.HasValue ? item.SumARTS : 0) - (item.BoxCount.HasValue ? item.BoxCount : 0));
                    compare.DiffGrossWeight = (decimal)((item.SumWGT.HasValue ? item.SumWGT : 0) - (item.SumGrossWeight.HasValue ? item.SumGrossWeight : 0));
                    compare.DiffNetWeight = (decimal)((item.SumQTY.HasValue ? item.SumQTY : 0) - (item.SumNetWeight.HasValue ? item.SumNetWeight : 0));
                    if (Convert.ToInt32(item.SumARTS) != Convert.ToInt32(item.BoxCount) || item.SumQTY != item.SumNetWeight || item.SumGrossWeight != item.SumWGT)
                    {
                        compare.Pointer = 0;
                    }
                    if (compare.Pointer == 0&&!string.IsNullOrEmpty(compare.WarehouseID))
                    {
                        ls.Add(compare);
                    }

                }
                viewModel.GridList = ls.OrderBy(x => x.WarehouseID).ToList();
                //viewModel.GridList = ls.OrderByDescending(x=>x.Pointer).ToPagedList(parameters.PageNo, parameters.PageSize);
                return PartialView("_GridList", viewModel);
            }

        }
    }
}