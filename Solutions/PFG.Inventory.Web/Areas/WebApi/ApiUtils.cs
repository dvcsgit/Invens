using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using NLog;
using PFG.Inventory.DataSource;

namespace PFG.Inventory.Web.Areas.WebApi
{
    /// <summary>
    /// 給api 共用的方法
    /// </summary>
    public class ApiUtils
    {
        protected static Logger _logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// 處理Android 丟上來的時間
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static DateTime? stringTimeToDataTimeParse(string source)
        {
            DateTime? result = null;
            if (!string.IsNullOrEmpty(source))
            {
                try
                {
                    result = DateTime.ParseExact(source, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
                }
                catch (Exception)
                {


                }

            }
            return result;
        }

        /// <summary>
        /// 寫入上傳的log 不管處理結果是成功或失敗都會寫近來
        /// 射後不理
        /// </summary>
        /// <param name="source"></param>
        public static async Task WriteDBLog(UploadLog source)
        {
            try
            {
                using (PFGWarehouseEntities db = new PFGWarehouseEntities())
                {
                    db.UploadLog.Add(source);
                    await db.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                if (ex is DbEntityValidationException)
                {
                    var dbEx = (DbEntityValidationException)ex;
                    var message = "WriteDBLog 發生錯誤: ";
                    foreach (var validationErrors in dbEx.EntityValidationErrors)
                    {
                        foreach (var validationError in validationErrors.ValidationErrors)
                        {
                            message += string.Format("屬性名稱: {0} 錯誤訊息: {1}", validationError.PropertyName, validationError.ErrorMessage);
                        }
                    }
                    _logger.Error(message);
                }
                else
                {
                    _logger.Error("寫入日誌失敗:{0}", ex.Message);
                }
                
            }

        }
    }
}