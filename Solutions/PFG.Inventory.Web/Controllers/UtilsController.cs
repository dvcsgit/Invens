using PFG.Inventory.Web.Library;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using PFG.Inventory.DataSource;
using NLog;
using System.Data.Entity.SqlServer;

namespace PFG.Inventory.Web.Controllers
{
    public class UtilsController : Controller
    {
        protected static Logger _logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// 檔案下載
        /// </summary>
        /// <param name="fullFilePath"></param>
        /// <returns></returns>
        public ActionResult Download(string fullFilePath)
        {
            try
            {
                //file
                FileStream fileStream = new FileStream(fullFilePath, FileMode.Open);
                var fileName = Path.GetFileName(fullFilePath);
                string fileDownloadName = fileName;

                if (Request.Browser.Browser == "IE")
                {
                    fileDownloadName = Server.UrlPathEncode(fileName);
                }

                return File(fileStream, SiteLibrary.GetMIMETypeString(fileName), fileDownloadName);

            }
            catch (Exception e)
            {
                return Content("");
            }
        }

        /// <summary>
        /// 取得外倉
        /// </summary>
        /// <param name="pageNo"></param>
        /// <param name="pageSize"></param>
        /// <param name="term"></param>
        /// <returns></returns>
        public ActionResult GetWarehouseID([DefaultValue(1)]int pageNo, [DefaultValue(10)]int pageSize, string term)
        {
            try
            {
                using (PFGWarehouseEntities db = new PFGWarehouseEntities())
                {
                    var query = db.BasicSettingWarehouse.Select(x => new { text = x.WarehouseName + "(" + x.WarehouseID + ")", id = x.WarehouseID }).AsQueryable();
                    if (!string.IsNullOrEmpty(term))
                    {
                        query = query.Where(x => x.text.Contains(term));
                    }

                    var result = query
                        .Select(x => new { id = x.id, text = x.text })
                        .OrderBy(x=>x.text)
                        .ToPagedList(pageNo, pageSize);

                    return Json(new { success = true, data = result, total = result.TotalItemCount }, "text/plain", JsonRequestBehavior.AllowGet);
                }

            }
            catch (Exception ex)
            {
                _logger.Error("GetWarehouseID - 發生錯誤:{0}", ex);
            }
            return Json(new { success = false, message = "發生錯誤" }, "text/plain", JsonRequestBehavior.AllowGet);

        }

        public ActionResult GetOptions([DefaultValue(1)]int pageNo,[DefaultValue(10)]int pageSize)
        {
            var ls = SelectListUtils.GetInOrOutWarehouse().ToList();
            if (ls.Count > 0)
            {
                ls.RemoveAt(0);
            }
            var result = ls.Select(x => new { id = x.Value, text = x.Text }).OrderBy(x => x.id).ToPagedList(pageNo, pageSize);
            return Json(new { success = true, data = result, total = result.TotalItemCount }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 取得庫位
        /// </summary>
        /// <param name="pageNo"></param>
        /// <param name="pageSize"></param>
        /// <param name="warehouseID"></param>
        /// <param name="term"></param>
        /// <returns></returns>
        public ActionResult GetLocation([DefaultValue(1)]int pageNo, [DefaultValue(10)]int pageSize, string warehouseID,string term)
        {
            try
            {
                using (PFGWarehouseEntities db = new PFGWarehouseEntities())
                {
                    

                    var query = db.BasicSettingLoaction.Where(x => x.WarehouseID == warehouseID)
                         .Select(x => new { x.Location, StrLocation = SqlFunctions.StringConvert((decimal?)x.Location).Trim() })
                         .AsQueryable();

                    //關鍵字
                    if (!string.IsNullOrEmpty(term))
                    {
                        query = query.Where(x => x.StrLocation.Contains(term));
                    }

                    var result = query
                        .OrderBy(x => x.Location)
                        .Select(x => new { id = x.StrLocation, text = x.StrLocation })
                        .ToPagedList(pageNo, pageSize);

                    return Json(new { success = true, data = result, total = result.TotalItemCount }, "text/plain", JsonRequestBehavior.AllowGet);
                }

            }
            catch (Exception ex)
            {
                _logger.Error("GetLocation - 發生錯誤:{0}", ex);
            }
            return Json(new { success = false, message = "發生錯誤" }, "text/plain", JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 取得產品代號
        /// </summary>
        /// <param name="pageNo"></param>
        /// <param name="pageSize"></param>
        /// <param name="warehouseID"></param>
        /// <param name="location">庫位(選擇)</param>
        /// <param name="term"></param>
        /// <returns></returns>
        public ActionResult GetProductCode([DefaultValue(1)]int pageNo, [DefaultValue(10)]int pageSize, string warehouseID, int? location, string term)
        {
            try
            {
                using (PFGWarehouseEntities db = new PFGWarehouseEntities())
                {
                    var query = db.Inventory.Where(x => x.WarehouseID == warehouseID).AsQueryable();

                    if(location.HasValue)
                    {
                        query = query.Where(x => x.Location == location);
                    }

                    //關鍵字
                    if (!string.IsNullOrEmpty(term))
                    {
                        query = query.Where(x => x.ProductCode.Contains(term));
                    }

                    var result = query
                        .Select(x =>  x.ProductCode )
                        .Distinct()
                        .OrderBy(x => x)
                        .Select(x => new { id = x, text = x })
                        .ToPagedList(pageNo, pageSize);

                    return Json(new { success = true, data = result, total = result.TotalItemCount }, "text/plain", JsonRequestBehavior.AllowGet);
                }

            }
            catch (Exception ex)
            {
                _logger.Error("GetProductCode - 發生錯誤:{0}", ex);
            }
            return Json(new { success = false, message = "發生錯誤" }, "text/plain", JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 取得等級
        /// </summary>
        /// <param name="pageNo"></param>
        /// <param name="pageSize"></param>
        /// <param name="warehouseID">必要</param>
        /// <param name="location">選擇</param>
        /// <param name="productCode">選擇</param>
        /// <param name="term"></param>
        /// <returns></returns>
        public ActionResult GetClass([DefaultValue(1)]int pageNo, [DefaultValue(10)]int pageSize,string warehouseID, int? location, string productCode, string term)
        {
            try
            {
                using (PFGWarehouseEntities db = new PFGWarehouseEntities())
                {
                    var query = db.Inventory.Where(x => x.WarehouseID == warehouseID)
                         .AsQueryable();

                    if (location.HasValue)
                    {
                        query = query.Where(x => x.Location == location);
                    }

                    //關鍵字
                    if (!string.IsNullOrEmpty(productCode))
                    {
                        query = query.Where(x => x.ProductCode == productCode);
                    }

                    //關鍵字
                    if (!string.IsNullOrEmpty(term))
                    {
                        query = query.Where(x => x.Class.Contains(term));
                    }


                    var result = query
                        .Select(x => x.Class)
                        .Distinct()
                        .OrderBy(x => x)
                        .Select(x => new { id = x, text = x })
                        .ToPagedList(pageNo, pageSize);

                    return Json(new { success = true, data = result, total = result.TotalItemCount }, "text/plain", JsonRequestBehavior.AllowGet);
                }

            }
            catch (Exception ex)
            {
                _logger.Error("GetClass - 發生錯誤:{0}", ex);
            }
            return Json(new { success = false, message = "發生錯誤" }, "text/plain", JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// TODO 需確認是否還有使用
        /// </summary>
        /// <param name="pageNo"></param>
        /// <param name="pageSize"></param>
        /// <param name="warehouseID"></param>
        /// <param name="term"></param>
        /// <returns></returns>
        public ActionResult GetProductCodeByHouseID([DefaultValue(1)]int pageNo, [DefaultValue(10)]int pageSize, string warehouseID,string term)
        {
            //var ls = SelectListUtils.GetProductCodeOptions(warehouseID, null).ToList();
            //if (ls.Count > 0)
            //{
            //    ls.RemoveAt(0);
            //}
            //var result = ls.Select(x => new { id = x.Value, text = x.Text }).OrderBy(x => x.id).ToPagedList(pageNo, pageSize);
            //return Json(new { success = true, data = result, total = result.TotalItemCount }, JsonRequestBehavior.AllowGet);
            try
            {

                using (PFGWarehouseEntities db = new PFGWarehouseEntities())
                {
                    var query = db.Inventory.Where(x => x.WarehouseID == warehouseID)
                         .AsQueryable();

                    if (!string.IsNullOrEmpty(term))
                    {
                        query = query.Where(x => x.ProductCode.Contains(term));

                    }


                    var result = query.Select(x => new { ProductCode=x.ProductCode})
                        .Distinct()
                        .OrderBy(x => x.ProductCode)
                        .Select(x => new { id = x.ProductCode, text = x.ProductCode })
                        .ToPagedList(pageNo, pageSize);

                    return Json(new { success = true, data = result, total = result.TotalItemCount }, "text/plain", JsonRequestBehavior.AllowGet);
                }

            }
            catch (Exception ex)
            {

            }
            return Json(new { success = false, message = "發生錯誤" }, "text/plain", JsonRequestBehavior.AllowGet);

        }

        /// <summary>
        /// TODO 需確認是否還有使用
        /// </summary>
        /// <param name="pageNo"></param>
        /// <param name="pageSize"></param>
        /// <param name="warehouseID"></param>
        /// <param name="location"></param>
        /// <param name="term"></param>
        /// <returns></returns>
        public ActionResult GetProductCodeByLocation([DefaultValue(1)]int pageNo,[DefaultValue(10)]int pageSize,string warehouseID,string location,string term)
        {
            //var ls = SelectListUtils.GetProductCodeOptions(warehouseID, location).ToList();
            //if (ls.Count > 0)
            //{
            //    ls.RemoveAt(0);
            //}
            //var result = ls.Select(x => new { id = x.Value, text = x.Text }).OrderBy(x => x.id).ToPagedList(pageNo, pageSize);
            //return Json(new { success = true, data = result, total = result.TotalItemCount }, JsonRequestBehavior.AllowGet);
            try
            {
                int loc = Convert.ToInt32(location);
                using (PFGWarehouseEntities db = new PFGWarehouseEntities())
                {
                    var query = db.Inventory.Where(x => x.WarehouseID == warehouseID&&x.Location==loc)
                         .AsQueryable();

                    if (!string.IsNullOrEmpty(term))
                    {
                        query = query.Where(x => x.ProductCode.Contains(term));

                    }


                    var result = query.Select(x => new { ProductCode=x.ProductCode})
                        .Distinct()
                        .OrderBy(x => x.ProductCode)
                        .Select(x => new { id = x.ProductCode, text = x.ProductCode })
                        .ToPagedList(pageNo, pageSize);

                    return Json(new { success = true, data = result, total = result.TotalItemCount }, "text/plain", JsonRequestBehavior.AllowGet);
                }

            }
            catch (Exception ex)
            {

            }
            return Json(new { success = false, message = "發生錯誤" }, "text/plain", JsonRequestBehavior.AllowGet);
        }


    }
}