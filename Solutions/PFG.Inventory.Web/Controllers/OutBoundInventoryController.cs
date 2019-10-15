using PFG.Inventory.DataSource;
using PFG.Inventory.Web.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PFG.Inventory.Web.Library;
using PagedList;
using PFG.Inventory.Web.Library.Filters;
using System.ComponentModel;
using PFG.Inventory.Web.Library.Enums;
using System.Data.Entity;
using System.Data.Entity.SqlServer;
using System.Data.SqlClient;
using PFG.Inventory.Web.Models;

namespace PFG.Inventory.Web.Controllers
{
    [SiteAuthorize]
    public class OutBoundInventoryController : Controller
    {
        // GET: OutBoundInventory
        [OperationCheck(EnumOperation.Query)]
        public ActionResult Index()
        {
            OutBoundInventoryListViewModels viewModel = new OutBoundInventoryListViewModels();
            viewModel.Parameters = new OutBoundInventoryParametersViewModels()
            {
                DateUpload = DateTime.Now.ToString("yyyy/MM/dd")
            };
            return View(viewModel);
        }

        [OperationCheck(EnumOperation.Query|EnumOperation.Export)]
        public ActionResult Query(OutBoundInventoryParametersViewModels parameters,[DefaultValue(false)]bool isExport,[DefaultValue(0)]int exp)
        {
            if (ModelState.IsValid)
            {
                PFGWarehouseEntities db = new PFGWarehouseEntities();
                var beginDate = Convert.ToDateTime(parameters.DateUpload);
                var endDate = beginDate.AddDays(1);
                #region 2015年6月15日功能調整
                /*
                 *6/15日調整功能——按倉庫進行重新生成交運單編號
                 *如20150615141209_1 
                 */
                var loc = db.MIS_Mapping_LOC;
                var pdno = db.MIS_Mapping_PDNO;
                var t_inventory = db.OutBoundInventory;
                /*
                 * Select     Substring(Replace(Replace(Replace(CONVERT(char(19), DataUpload, 120),'-',''),':',''),' ',''),3,12) as VhNo
                                     ,t3.LOC as TriStk
                                     ,ROW_NUMBER() OVER(ORDER BY LOC) AS IT
                        From [PFGWarehouse].[dbo].[OutBoundInventory] t1
                        left join [PFGWarehouse].[dbo].[MIS_Mapping_PDNO] t2 on t1.ProductCode=t2.PDID
                        left join [PFGWarehouse].[dbo].[MIS_Mapping_LOC] t3 on  substring(t2.PDNO,14,1)=t3.PLANT and                    
                           t1.ProductCode=t3.PDID and t1.Class=t3.GD
                        group by DataUpload,LOC

                 */
                //var it = 0;
                //var t4 = from t1 in t_inventory
                //         join t2 in pdno
                //         on t1.ProductCode.Trim() equals t2.PDID.Trim()
                //         into t_pdno
                //         from t2 in t_pdno
                //         join t3 in loc
                //         on new { pdno = t2.PDID.Substring(13, 1), code = t1.ProductCode.Trim(), gd = t1.Class.Trim() } equals new { pdno = t3.PLANT.Trim(), code = t3.PDID, gd = t3.GD }
                //         into t_loc
                //         from t in t_loc
                //         group t1 by new { t1.DataUpload, t.LOC }
                //             into k
                //             from r in k
                //             select new
                //             {
                //                 Vhno = PDFExtensions.GetExpno(r.DataUpload.Value),
                //                 TriStk = k.Key.LOC,
                //                 IT = 
                //             };
                       
                       
                       
                       

                #endregion


                var query = (from x in db.OutBoundInventory
                            where x.DataUpload >=beginDate&&x.DataUpload<endDate
                            && x.WarehouseID == parameters.WarehouseID
                            select new OutBoundInventoryViewModels() {
                                BoxNumber = x.BoxNumber,
                                CarNo = x.CarNo,
                                Class = x.Class,
                                DateUpload = x.DataUpload,
                                GrossWeight = x.GrossWeight,
                                No=x.No,
                                Location = x.Location,
                                NetWeight = x.NetWeight,
                                ProductCode = x.ProductCode,
                                WarehouseID = x.WarehouseID,
                                UploadAccount = x.UploadAccount,
                                PrintFlag=string.IsNullOrEmpty(x.PrintFlag)?"N":x.PrintFlag
                            }).ToList();
                
                ////var query = db.OutBoundInventory.Where(x => ((TimeSpan)(x.DataUpload - date)).Hours <= 24 && x.WarehouseID == parameters.WarehouseID).Select(
                ////    x => new OutBoundInventoryViewModels()
                ////    {
                ////        BoxNumber = x.BoxNumber,
                ////        CarNo = x.CarNo,
                ////        Class = x.Class,
                ////        DateUpload = x.DataUpload,
                ////        GrossWeight = x.GrossWeight,

                ////        Location = x.Location,
                ////        NetWeight = x.NetWeight,
                ////        ProductCode = x.ProductCode,
                ////        WarehouseID = x.WarehouseID,
                ////        UploadAccount = x.UploadAccount
                ////    }
                ////    ).ToList();
                var distinctList = new List<OutBoundInventoryDistinctViewModels>();
                int id = 0;
                foreach (var item in query)
                {
                   // var test = ((DateTime)x.DateUpload).Subtract((DateTime)item.DateUpload).TotalSeconds;
                    var inv = distinctList.Where(x => x.CarNo == item.CarNo && ((DateTime)x.DateUpload).Subtract((DateTime)item.DateUpload).Ticks==0 && x.UploadAccount == item.UploadAccount);
                    //if (inv.Count() > 0)
                    //{
                    //    distinctList.ElementAt(inv.Count() - 1).DateUpload = item.DateUpload;
                    //}
                    if(inv.Count()==0)
                    {
                        id += 1;
                        OutBoundInventoryDistinctViewModels obid = new OutBoundInventoryDistinctViewModels();
                        obid.CarNo = item.CarNo;
                        obid.Index = id;
                        obid.DateUpload = item.DateUpload;
                        obid.UploadAccount = item.UploadAccount;
                        obid.No = item.No;
                        obid.PrintFlag = item.PrintFlag;
                        distinctList.Add(obid);
                    }
                }
                
                var viewModels = new OutBoundInventoryListViewModels()
                {
                    Parameters = parameters
                };
                if (isExport)
                {
                    
                    if(exp==0)
                    {
                        return Json(new { success=false,errors="請選擇一項列印"});
                    }
                    var inventoryList = new List<OutBoundInventoryViewModels>();
                    var distictInv = distinctList.Single(x=>x.Index==exp);

                    #region 最新交運單編號設置方法2015/06/15-06/16
                    var dateUpload = distictInv.DateUpload;
                    var sql = @"select Convert(varchar(20),t4.VhNo)+'_'+Convert(varchar(20),t4.IT) as VhNo
                  ,'0160' as TrDp
                  ,Convert(varchar(10),Convert(int,substring(Convert(char(8),DataUpload,112),1,4))-1911) +substring(Convert(char(8),DataUpload,112),5,4) as TroDat
                  ,Convert(varchar(10),Convert(int,substring(Convert(char(8),DataUpload,112),1,4))-1911) +substring(Convert(char(8),DataUpload,112),5,4) as TriDat
                  ,t3.LOC as TriStk
                  ,WarehouseID as TroStk
                  ,ROW_NUMBER() OVER (PARTITION BY t3.LOC ORDER BY t2.PDNO) AS IT
                  ,ProductCode as PdId
                  ,t2.PDNO
                  ,Class as Gd
                  ,'KG' as Un
                  ,Sum(Convert(int,NetWeight))  as Qty 
                  ,Sum(Convert(int,GrossWeight)) as Wgt
                  ,Count(*) as Arts
        From [PFGWarehouse].[dbo].[OutBoundInventory] t1
        left join [PFGWarehouse].[dbo].[MIS_Mapping_PDNO] t2 on t1.ProductCode=t2.PDID
       left join [PFGWarehouse].[dbo].[MIS_Mapping_LOC] t3 on  substring(t2.PDNO,14,1)=t3.PLANT and  t1.ProductCode=t3.PDID and t1.Class=t3.GD
        left join (
                      Select     Substring(Replace(Replace(Replace(CONVERT(char(19), DataUpload, 120),'-',''),':',''),' ',''),3,12) as VhNo
                                ,t3.LOC as TriStk
                                ,ROW_NUMBER() OVER(ORDER BY LOC) AS IT
                     From [PFGWarehouse].[dbo].[OutBoundInventory] t1
                     left join [PFGWarehouse].[dbo].[MIS_Mapping_PDNO] t2 on t1.ProductCode=t2.PDID
                     left join [PFGWarehouse].[dbo].[MIS_Mapping_LOC] t3 on  substring(t2.PDNO,14,1)=t3.PLANT and  t1.ProductCode=t3.PDID and t1.Class=t3.GD
                     where WarehouseID=@WarehouseID    and DataUpload=@DateUpload
                     group by DataUpload,LOC
        ) as t4 on t3.LOC=t4.TriStk
       where WarehouseID=@WarehouseID    and DataUpload=@DateUpload
       group by DataUpload,WarehouseID,ProductCode,Class,LOC,PDNO,t4.VhNo,t4.IT";
                    var para = new SqlParameter[]{
                    new SqlParameter("@WarehouseID",parameters.WarehouseID),
                    new SqlParameter("@DateUpload",dateUpload)
                    };
                    db.Database.CommandTimeout = 999999999;
                    var l = db.Database.SqlQuery<OutBoundInventoryQuery>(sql, para).ToList();

                    #endregion

                    int i = 0;
                    
                    foreach (var item in query)
                    {
                        if (item.CarNo == distictInv.CarNo && item.DateUpload == distictInv.DateUpload && item.UploadAccount == distictInv.UploadAccount)
                        {
                            i += 1;
                            OutBoundInventoryViewModels inventory = item;
                            inventory.Index = i;
                            inventoryList.Add(inventory);
                            var modifyData = from x in db.OutBoundInventory
                                             where x.DataUpload >= beginDate && x.DataUpload < endDate
                                             && x.WarehouseID == parameters.WarehouseID
                                             &&x.CarNo==item.CarNo&&x.DataUpload==item.DateUpload&&x.UploadAccount==item.UploadAccount
                                             select x;
                            foreach (var mitem in modifyData)
                            {
                                mitem.PrintFlag = "Y";
                                db.Entry<OutBoundInventory>(mitem).State = EntityState.Modified;
                            }
                        }
                        //item.PrintFlag = "Y";
                        //db.Entry<OutBoundInventory>(item).State = EntityState.Modified;
                    }
                    db.SaveChanges();
                    

                    
                    if (inventoryList.Count == 0)
                    {
                        return Json(new { success = false, errors = "沒有可列印的數據", JsonRequestBehavior.AllowGet });
                    }
                    db.SaveChanges();
                    //var setFlag = db.OutBoundInventory.Single(x => x.No == distictInv.No);
                    //setFlag.PrintFlag = "Y";
                    //db.Entry<OutBoundInventory>(setFlag).State = EntityState.Modified;
                    //db.SaveChanges();

                    var desFilePath = inventoryList.GroupByProductCode(l).ExportPDF("交運明細單.pdf");
                    var url = Url.Action("Download", "Utils", new { @fullFilePath = desFilePath });
                    return Json(new { success = true, url = url,JsonRequestBehavior.AllowGet });
                }
                //.GridList = inventoryList.ToPagedList(parameters.PageNo, parameters.PageSize);
                viewModels.DistinctGridList = distinctList.ToPagedList(parameters.PageNo, parameters.PageSize);
                return PartialView("_GridList", viewModels);
            }
            return Json(new { success = false, errors = "請選擇日期和外倉", JsonRequestBehavior.AllowGet });
        }
    }
}