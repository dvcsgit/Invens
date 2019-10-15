using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Validation;
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
using PFG.Inventory.DataSource;
using PFG.Inventory.Domain.Enums;
using PFG.Inventory.Web.Areas.WebApi.Models;
using PFG.Inventory.Web.Library;
using PFG.Inventory.Web.Library.Extensions;

namespace PFG.Inventory.Web.Areas.WebApi.Controllers
{
    public class UploadController : ApiController
    {
        private static readonly string CONST_NAME = "UploadApi";
        protected static Logger _logger = LogManager.GetLogger(CONST_NAME);
        private readonly string _uploadPath = System.Web.Hosting.HostingEnvironment.MapPath(SiteLibrary.AppSettings("UploadFolder"));
        private readonly string _tempPath = System.Web.Hosting.HostingEnvironment.MapPath(SiteLibrary.AppSettings("TempFolder"));
        /// <summary>
        /// 上傳
        /// </summary>
        /// <param name="account">帳號</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<HttpResponseMessage> Post(string account)
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
                _logger.Info("UploadApi start");

                List<string> uploadFileList = putUploadFileToTempTask(httpRequest,account);
                
                if(uploadFileList.Count>0)
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
                        using(SQLiteConnection conn = new SQLiteConnection(dataSrouce))
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
                    _logger.Error("UploadApi 發生錯誤:{0} 檔案無法解壓縮", ex.Message);
                    newLogItem.ExceptionMessage = "檔案無法解壓縮";
                }
                else if (ex is DbEntityValidationException)
                {
                    var dbEx = (DbEntityValidationException)ex;
                    var message = "UploadApi 發生錯誤: ";
                    foreach (var validationErrors in dbEx.EntityValidationErrors)
                    {
                        foreach (var validationError in validationErrors.ValidationErrors)
                        {
                            message += string.Format("屬性名稱: {0} 錯誤訊息: {1}",validationError.PropertyName, validationError.ErrorMessage);
                        }
                    }
                    newLogItem.ExceptionMessage = message;
                    _logger.Error(message);
                }
                else
                {
                    _logger.Error("UploadApi 發生錯誤:{0}", ex.Message);
                    newLogItem.ExceptionMessage = ex.Message;
                }

                result = Request.CreateResponse(HttpStatusCode.InternalServerError, new { Success = false, Message = errorMessage });
            }
            finally
            {
                //寫入log 區塊
                ApiUtils.WriteDBLog(newLogItem);
                _logger.Info("UploadApi end");
            }

            return result;
        }

        /// <summary>
        /// 將前端送來的檔案丟到暫存檔 允許多個檔案
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        private List<string> putUploadFileToTempTask(HttpRequest request,string account)
        {
            var result = new List<string>();
            var saveFileName = string.Format("Upload_{0}_{1}.zip", account ,Guid.NewGuid());
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
        private void processDataToServer(List<InventoryItem> source, string account,UploadLog uploadLog)
        {
            var currentDate =  DateTime.Now;
            //物件轉成資料庫物件
            Mapper.CreateMap<InventoryItem, PFG.Inventory.DataSource.Inventory>()
                .ForMember(dest => dest.DateCreated, opt => opt.MapFrom(src => ApiUtils.stringTimeToDataTimeParse(src.DateCreated)));
            
            //InventoryItem 轉成 OutBoundInventory
            Mapper.CreateMap<PFG.Inventory.DataSource.Inventory, PFG.Inventory.DataSource.OutBoundInventory>();
            List<string> summary_message = new List<string>();
            var tempList = Mapper.Map<List<InventoryItem>, List<PFG.Inventory.DataSource.Inventory>>(source);
            using(TransactionScope scope = new TransactionScope())
            {
                using (PFGWarehouseEntities db = new PFGWarehouseEntities())
                {
                    //處理資料
                    //1.      Y資料未做更動，不執行
                    //2.      I入庫Insert，除StatusCode變更為Y外，其餘依資料庫資料進行Insert
                    //3.      O出庫依資料庫資料更新StatusCode、ModifierAccount、DateModified
                    //4.      C換排除StatusCode變更為Y外，依資料庫更新Location、ModifierAccount、DateModified
                    _logger.Debug("SQLite 資料總筆數:{0}", tempList.Count);
                    uploadLog.TotalRecords = tempList.Count;
                    var insertList = tempList.Where(x => x.StatusCode == StatusCodeConst.I).ToList();
                    _logger.Debug("SQLite I入庫 筆數:{0}", insertList.Count);
                    uploadLog.UploadLogDetail.Add(new UploadLogDetail { LogType = ApiLogTypeConst.Debug, Message = string.Format("I入庫 筆數:{0}", insertList.Count), DateCreated = DateTime.Now });
                    summary_message.Add(string.Format("I入庫 筆數:{0}", insertList.Count));

                    foreach (var item in insertList)
                    {
                        var existItem = db.Inventory.Where(x => x.BoxNumber == item.BoxNumber).FirstOrDefault();

                        if (existItem == null)
                        {
                            //不存在才能新增
                            item.StatusCode = StatusCodeConst.Y;
                            item.DataUpload = currentDate;
                            item.UploadAccount = account;
                            db.Inventory.Add(item);
                        }
                        else
                        {
                            if (existItem.StatusCode == StatusCodeConst.O)
                            {
                                //表示該資料出庫後又入庫了 進行DELETE INSERT
                                _logger.Warn("BoxNumber: {0} 已存在,已入庫又再入庫", item.BoxNumber);
                                uploadLog.UploadLogDetail.Add(new UploadLogDetail { LogType = ApiLogTypeConst.Warn, Message = string.Format("BoxNumber: {0} 已存在,已入庫又再入庫", item.BoxNumber), DateCreated = DateTime.Now });
                                db.Inventory.Remove(existItem);
                                item.StatusCode = StatusCodeConst.Y;
                                item.DataUpload = currentDate;
                                item.UploadAccount = account;
                                db.Inventory.Add(item);
                            }
                            else
                            {
                                _logger.Warn("BoxNumber: {0} 已存在,無法新增", item.BoxNumber);
                                uploadLog.UploadLogDetail.Add(new UploadLogDetail { LogType = ApiLogTypeConst.Warn, Message = string.Format("BoxNumber: {0} 已存在,無法新增", item.BoxNumber), DateCreated = DateTime.Now });
                            }

                            
                        }
                        
                    }
                    db.SaveChanges();

                    var outputList = tempList.Where(x => x.StatusCode == StatusCodeConst.O).ToList();
                    _logger.Debug("SQLite O出庫 筆數:{0}", outputList.Count);
                    uploadLog.UploadLogDetail.Add(new UploadLogDetail { LogType = ApiLogTypeConst.Debug, Message = string.Format("O出庫 筆數:{0}", outputList.Count), DateCreated = DateTime.Now });
                    summary_message.Add(string.Format("O出庫 筆數:{0}", outputList.Count));
                    foreach (var item in outputList)
                    {
                        var oldItem = db.Inventory.Where(x => x.BoxNumber == item.BoxNumber).FirstOrDefault();
                        if (oldItem!=null)
                        {
                            oldItem.StatusCode = StatusCodeConst.O;
                            oldItem.ModifierAccount = item.ModifierAccount;
                            oldItem.DateModified = item.DateModified;
                            oldItem.DataUpload = currentDate;
                            oldItem.UploadAccount = account;
                            //NOTE:增加寫入到 OutBoundInventory 資料表中
                            var newItem = Mapper.Map<PFG.Inventory.DataSource.Inventory, OutBoundInventory>(oldItem);
                            newItem.ModifierAccount = null;
                            newItem.DateModified = null;
                            db.OutBoundInventory.Add(newItem);
                            db.SaveChanges();
                        }
                        else
                        {
                            _logger.Warn("BoxNumber: {0} 不存在,無法出庫", item.BoxNumber);
                            uploadLog.UploadLogDetail.Add(new UploadLogDetail { LogType = ApiLogTypeConst.Warn, Message = string.Format("BoxNumber: {0} 不存在,無法出庫", item.BoxNumber), DateCreated = DateTime.Now });
                        }
                    }
                    db.SaveChanges();

                    var changeList = tempList.Where(x => x.StatusCode == StatusCodeConst.C).ToList();
                    _logger.Debug("SQLite C換排 筆數:{0}", changeList.Count);
                    uploadLog.UploadLogDetail.Add(new UploadLogDetail { LogType = ApiLogTypeConst.Debug, Message = string.Format("C換排 筆數:{0}", changeList.Count), DateCreated = DateTime.Now });
                    summary_message.Add(string.Format("C換排 筆數:{0}", changeList.Count));
                    foreach (var item in changeList)
                    {
                        var oldItem = db.Inventory.Where(x => x.BoxNumber == item.BoxNumber).FirstOrDefault();
                        if (oldItem != null)
                        {
                            oldItem.StatusCode = StatusCodeConst.Y;
                            oldItem.ModifierAccount = item.ModifierAccount;
                            oldItem.DateModified = item.DateModified;
                            oldItem.Location = item.Location;
                            oldItem.DataUpload = currentDate;
                            oldItem.UploadAccount = account;
                            db.SaveChanges();
                        }
                        else
                        {
                            _logger.Warn("BoxNumber: {0} 不存在,無法換排", item.BoxNumber);
                            uploadLog.UploadLogDetail.Add(new UploadLogDetail { LogType = ApiLogTypeConst.Warn, Message = string.Format("BoxNumber: {0} 不存在,無法換排", item.BoxNumber), DateCreated = DateTime.Now });
                        }
                    }
                    db.SaveChanges();
                   
                }
                uploadLog.Summary = string.Join(",",summary_message);

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
