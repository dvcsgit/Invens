using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using NLog;
using PFG.Inventory.DataSource;

namespace PFG.Inventory.WebAPI.Controllers
{
    public class WarehouseController : ApiController
    {
        protected static Logger _logger = LogManager.GetCurrentClassLogger();

        // GET api/warehouse
        public IEnumerable<Warehouse> Get()
        {
            List<Warehouse> viewModel = new List<Warehouse>();
            try
            {
                using (PFGWarehouseEntities db = new PFGWarehouseEntities())
                {
                    var query = db.BasicSettingWarehouse.Select(x => new Warehouse { 
                        WarehouseID = x.WarehouseID,
                        WarehouseName = x.WarehouseName,
                        Capacity = x.Capacity
                    });

                    viewModel = query.ToList();
                }
            }
            catch (Exception ex)
            {

                _logger.Error("發生錯誤:{0}", ex);
            }

            return viewModel;
        }

        public class Warehouse
        {
            /// <summary>
            /// 是否成功登入
            /// </summary>
            public string WarehouseID { get; set; }

            /// <summary>
            /// 無法登入錯誤訊息
            /// </summary>
            public string WarehouseName { get; set; }

            /// <summary>
            /// 成功登入後-帳號
            /// </summary>
            public int? Capacity { get; set; }
        }
    }
}
