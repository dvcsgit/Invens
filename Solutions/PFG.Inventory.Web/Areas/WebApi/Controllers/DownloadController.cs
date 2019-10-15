using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;
using AutoMapper;
using NLog;
using PFG.Inventory.DataSource;
using PFG.Inventory.Domain.Resources;
using PFG.Inventory.Web.Areas.WebApi.Models;
using PFG.Inventory.Web.Library;


namespace PFG.Inventory.WebAPI.Controllers
{
    public class DownloadController : ApiController
    {
        protected static Logger _logger = LogManager.GetLogger("DownloadApi");

        /// <summary>
        /// 
        /// </summary>
        /// <param name="warehouseID"></param>
        /// <returns></returns>
        public HttpResponseMessage Get(string warehouseID)
        {
            HttpResponseMessage response = new HttpResponseMessage();
            _logger.Debug("Download 參數 warehouseID:{0}", warehouseID);
            var isDebug = bool.Parse(SiteLibrary.AppSettings("IsDebug"));
            
            
            DownloadViewModel viewModel = new DownloadViewModel();
            string packageFileName = string.Format("PFGInventory_{0}.zip", DateTime.Now.ToString("yyyyMMddHHmmssfff"));
            var currentDate = DateTime.Now.ToString("yyyyMMdd hhmmss");

            try
            {
                using (PFGWarehouseEntities db = new PFGWarehouseEntities())
                {
                    createMapInit();

                    var location = db.BasicSettingLoaction.Where(x => x.WarehouseID == warehouseID)
                        .Select(x=> new BasicSettingLoactionItem{
                                  Location = x.Location,
                                  WarehouseID = x.WarehouseID
                        })
                        .ToList();
                    
                    viewModel.BasicSettingLoaction = location;

                    var inventoryList = db.Inventory.Where(x => x.WarehouseID == warehouseID && x.StatusCode != "O")
                        .ToList();
                    var inventory = Mapper.Map<List<PFG.Inventory.DataSource.Inventory>, List<InventoryItem>>(inventoryList);
                    viewModel.Inventory = inventory;

                    //viewModel.Inventory;
                    //產出SQLite
                    var tempSqliteFilePath = SQLiteUtils.GenerateSQLiteZip(viewModel);

                    if (isDebug)
                    {
                        response = Request.CreateResponse(HttpStatusCode.OK);
                    }
                    else
                    {
                        if (File.Exists(tempSqliteFilePath))
                        {
                            FileStream fileStream = new FileStream(tempSqliteFilePath, FileMode.Open, FileAccess.Read)
                            {

                            };
                            response = Request.CreateResponse(HttpStatusCode.OK);
                            response.Content = new StreamContent(fileStream);
                            response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                            response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
                            {
                                FileName = packageFileName
                            };
                        }
                        else
                        {
                            response = Request.CreateResponse(HttpStatusCode.InternalServerError);
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                _logger.Error("發生錯誤:{0}", ex.Message);
                response = Request.CreateResponse(HttpStatusCode.InternalServerError);
                throw ex;
            }
            finally
            {

            }

            return response;
        }

        /// <summary>
        /// AutoMappper對應設定
        /// </summary>
        private void createMapInit()
        {
            
            Mapper.CreateMap<PFG.Inventory.DataSource.Inventory, InventoryItem>()
                .ForMember(dest => dest.DateModified, opt => opt.MapFrom(src => src.DateModified.HasValue ? src.DateModified.Value.ToString(Resources.StrDateTimeFormat) : null))
                .ForMember(dest => dest.DateCreated, opt => opt.MapFrom(src => src.DateCreated.HasValue ? src.DateCreated.Value.ToString(Resources.StrDateTimeFormat) : null));
      
        }
    }
}
