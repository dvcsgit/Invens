using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Validation;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Transactions;
using System.Web;
using System.Web.Http;
using AutoMapper;
using Ionic.Zip;
using NLog;
using PFG.Inventory.Data.Repositories;
using PFG.Inventory.DataSource;
using PFG.Inventory.Domain.Enums;
using PFG.Inventory.Web.Areas.WebApi.Models;
using PFG.Inventory.Web.Library;
using PFG.Inventory.Web.Library.Extensions;

namespace PFG.Inventory.Web.Areas.WebApi.Controllers
{
    public class StockUploadController: ApiController
    {
        private static readonly string CONST_NAME = "StockUploadApi";
        protected static Logger _logger = LogManager.GetLogger(CONST_NAME);
        private readonly string _uploadPath = System.Web.Hosting.HostingEnvironment.MapPath(SiteLibrary.AppSettings("UploadFolder"));
        private readonly string _tempPath = System.Web.Hosting.HostingEnvironment.MapPath(SiteLibrary.AppSettings("TempFolder"));
        


        /// <summary>
        /// 上傳
        /// </summary>
        /// <param name="account">帳號</param>
        /// <returns></returns>
        [HttpPost]
        public HttpResponseMessage Post(string account)
        {
            HttpResponseMessage result = null;
            var httpRequest = HttpContext.Current.Request;

            //NOTE 先放log物件 因如果放在scope中,會因為沒有complete而無法寫入
            UploadLog newLogItem = new UploadLog
            {
                Account = account,
                ControllerName = CONST_NAME,
                DataUpload = DateTime.Now,
                //UploadFileName = uploadFileName,
                //TempFileName = tempFileName,
                //DataUpload = currentDate,
                Flag = (int)EnumUploadLogFlag.None
            };
            newLogItem.UploadLogDetail = new List<UploadLogDetail>();


            try
            {
                _logger.Info("StockUploadApi start");

                List<string> uploadFileList = putUploadFileToTempTask(httpRequest, account);

                if (uploadFileList.Count > 0)
                {
                    //1.解壓縮
                    string unpackDirectory = Guid.NewGuid().ToString();
                    string zipExtract = Path.Combine(_tempPath, unpackDirectory);
                    createDirectory(zipExtract);
                    foreach (var item in uploadFileList)
                    {
                        newLogItem.UploadFileName = item;//TODO 目前原則只會收到一個檔案裏面有一個有壓縮檔
                        using (ZipFile zip = ZipFile.Read(item))
                        {
                            foreach (ZipEntry e in zip)
                            {
                                e.Extract(zipExtract, ExtractExistingFileAction.OverwriteSilently);

                            }
                        }
                    }



                    //2.更新DB資料 
                    //Note:此專案原則檔案只會只有一個
                    //讀取路徑
                    var tempFileList = Directory.GetFiles(zipExtract, "*.db");//只抓取.db檔案

                    foreach (var tempFilePath in tempFileList)
                    {
                        var fileName = Path.GetFileName(tempFilePath);
                        var dataSrouce = string.Format("Data Source={0}", tempFilePath);
                        newLogItem.TempFileName = tempFilePath;
                        DataTable dt = new DataTable();
                        using (SQLiteConnection conn = new SQLiteConnection(dataSrouce))
                        {
                            //TODO 未來資料量一多的話可能要做分頁 或 多執行續
                            var sqlText = "SELECT * FROM Inventory";
                            using (SQLiteDataAdapter adapter = new SQLiteDataAdapter(sqlText, conn))
                            {
                                adapter.Fill(dt);
                            }

                        }

                        var inventoryList = dt.DataTableToList<InventoryItem>();

                        processDataToServer(inventoryList, account, newLogItem);

                    }
                    // return result
                    result = Request.CreateResponse(HttpStatusCode.Created, new { Success = true, Message = "已接收到檔案,主機處理中" });
                    //執行到這邊表示成功
                    newLogItem.Flag = (int)EnumUploadLogFlag.Success;
                }
                else
                {
                    string system_no_file = "偵測不到有檔案上傳的痕跡";
                    // return BadRequest (no file(s) available)
                    result = Request.CreateResponse(HttpStatusCode.BadRequest, new { Success = false, Message = system_no_file });
                    //執行到這邊表示成功
                    newLogItem.Flag = (int)EnumUploadLogFlag.Fail;
                    newLogItem.ExceptionMessage = system_no_file;
                }

            }
            catch (Exception ex)
            {
                newLogItem.Flag = (int)EnumUploadLogFlag.Fail;

                var errorMessage = ex.Message;
                if (ex is Ionic.Zip.ZipException)
                {
                    _logger.Error("StockUploadApi 發生錯誤:{0} 檔案無法解壓縮", ex.Message);
                    newLogItem.ExceptionMessage = "檔案無法解壓縮";
                }
                else if (ex is DbEntityValidationException)
                {
                    var dbEx = (DbEntityValidationException)ex;
                    var message = "StockUploadApi 發生錯誤: ";
                    foreach (var validationErrors in dbEx.EntityValidationErrors)
                    {
                        foreach (var validationError in validationErrors.ValidationErrors)
                        {
                            message += string.Format("屬性名稱: {0} 錯誤訊息: {1}", validationError.PropertyName, validationError.ErrorMessage);
                        }
                    }
                    newLogItem.ExceptionMessage = message;
                    _logger.Error(message);
                }
                else
                {
                    _logger.Error("StockUploadApi 發生錯誤:{0}", ex.Message);
                    newLogItem.ExceptionMessage = ex.Message;
                }

                result = Request.CreateResponse(HttpStatusCode.InternalServerError, new { Success = false, Message = errorMessage });
            }
            finally
            {
                //寫入log 區塊
                ApiUtils.WriteDBLog(newLogItem);
                _logger.Info("StockUploadApi end");
            }

            return result;
        }

        /// <summary>
        /// 將前端送來的檔案丟到暫存檔 允許多個檔案
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        private List<string> putUploadFileToTempTask(HttpRequest request, string account)
        {
            var result = new List<string>();
            var saveFileName = string.Format("Upload_stock_{0}_{1}.zip", account, Guid.NewGuid());
            if (request.Files.Count > 0)
            {
                foreach (string file in request.Files)
                {
                    var postedFile = request.Files[file];
                    var filePath = System.IO.Path.Combine(_uploadPath, saveFileName);
                    postedFile.SaveAs(filePath);
                    result.Add(filePath);
                }
            }
            return result;
        }



        /// <summary>
        /// 將記憶體的資料丟進去資料庫裡
        /// </summary>
        /// <param name="source"></param>
        /// <param name="account"></param>
        /// <param name="uploadFileName"></param>
        /// <param name="tempFileName"></param>
        private void processDataToServer(List<InventoryItem> source, string account, UploadLog uploadLog)
        {
            var currentDate = DateTime.Now;
            //物件轉成資料庫物件
            Mapper.CreateMap<InventoryItem, PFG.Inventory.DataSource.InventoryStock>()
                .ForMember(dest => dest.DateCreated, opt => opt.MapFrom(src => ApiUtils.stringTimeToDataTimeParse(src.DateCreated)))
                .ForMember(dest => dest.DateStockTime, opt => opt.MapFrom(src => ApiUtils.stringTimeToDataTimeParse(src.DateStockTime)));

            List<string> summary_message = new List<string>();
            var tempList = Mapper.Map<List<InventoryItem>, List<PFG.Inventory.DataSource.InventoryStock>>(source);


            using (TransactionScope scope = new TransactionScope())
            {
                using (PFGWarehouseEntities db = new PFGWarehouseEntities())
                {
                    _logger.Debug("SQLite 資料總筆數:{0}", tempList.Count);
                    uploadLog.TotalRecords = tempList.Count;
                    var date_key = currentDate.ToString("yyyyMMdd");
                    var source_warehouse_id = source.Select(x => x.WarehouseID).Distinct().FirstOrDefault();
                    //檢查有沒有已經存在 相同日期 若有則刪除掉
                    List<SqlParameter> paramList = new List<SqlParameter>();
                    paramList.Add(new SqlParameter("@DateStock", date_key));
                    paramList.Add(new SqlParameter("@WarehouseID", source_warehouse_id));
                    var sqlText = "DELETE FROM InventoryStock WHERE DateStock= @DateStock AND WarehouseID=@WarehouseID ";
                    int deleteRow = db.Database.ExecuteSqlCommand(sqlText, paramList.ToArray());
                    _logger.Debug("SQLite 刪除資料筆數:{0}", deleteRow);

                    //loop data
                    foreach (var item in tempList)
                    {
                        item.DateStock = date_key;
                        item.DataUpload = currentDate;
                        
                    }

                    //var table_insert_script = tempList.ToInsertScriptForMS("InventoryStock");
                    //var debugIdx = 0;
                    //foreach (var item in table_insert_script)
                    //{
                        
                    //    db.Database.ExecuteSqlCommand(item);
                    //    debugIdx++;
                    //}

                    db.BulkInsertAll<InventoryStock>(tempList);
                    

                }                
                

                scope.Complete();
            }
        }

        /// <summary>
        /// 建立目錄
        /// </summary>
        /// <param name="path"></param>
        private static void createDirectory(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }



    }
}
