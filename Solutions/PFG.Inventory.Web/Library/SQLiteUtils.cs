using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Web;
using Ionic.Zip;
using NLog;
using PFG.Inventory.Web.Areas.WebApi.Models;



namespace PFG.Inventory.Web.Library
{
    public class SQLiteUtils
    {
        protected static Logger _logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// 指定SQLSERVER到SQLITE
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entities"></param>
        /// <param name="tableName"></param>
        /// <param name="createTableCommand"></param>
        public static string GenerateSQLiteZip(DownloadViewModel viewModel)
        {
            var result = "";
            var downloadFolder = System.Web.Hosting.HostingEnvironment.MapPath(SiteLibrary.AppSettings("DownloadFolder"));//存放資料夾
            var isDebug = bool.Parse(SiteLibrary.AppSettings("IsDebug"));
            var zipFileName = " PFGInventory.zip";
            
            string targetPath = "";//sqlite存放路徑  
            SQLiteConnection con = null;
            try
            {
                if(!isDebug)
                {
                    //上線時,檔案會在存放資料夾產生流水資料夾 EX: Folder\Download\20140829154043_7f12d7b1-f3ca-4456-8527-0baa1d32ce67
                    var folderName = DateTime.Now.ToString("yyyyMMddHHmmss_") + Guid.NewGuid();
                    downloadFolder = Path.Combine(downloadFolder, folderName);//更新存放資料夾
                    if (!Directory.Exists(downloadFolder))
                    {
                        Directory.CreateDirectory(downloadFolder);
                    }
                }

                targetPath = Path.Combine(downloadFolder, "PFGInventory.db");

                string dataSrouce = string.Format("Data Source={0}", targetPath);
                //組出來源
                if (!System.IO.File.Exists(targetPath))
                {
                    SQLiteConnection.CreateFile(targetPath);
                }
                //組出輸出
                using (con = new SQLiteConnection(dataSrouce))
                {
                    

                    using (SQLiteCommand com = new SQLiteCommand(con))
                    {
                        con.Open();
                        using (var trans = con.BeginTransaction())
                        {
                            //
                            com.CommandText = viewModel.BasicSettingLoaction.ToCreateTableScript("BasicSettingLoaction");
                            com.ExecuteNonQuery();
                            var basicSettingLoactionCmds = viewModel.BasicSettingLoaction.ToInsertScript("BasicSettingLoaction");
                            foreach (var item in basicSettingLoactionCmds)
                            {
                                com.CommandText = item;
                                com.ExecuteNonQuery();
                            }


                            //庫存
                            com.CommandText = viewModel.Inventory.ToCreateTableScript("Inventory");
                            com.ExecuteNonQuery();

                            var inventory_index_script = @"
                                CREATE INDEX IDX_WAREHOUSEID on Inventory(WarehouseID ASC);
                                CREATE INDEX IDX_WAREHOUSEID_LOCATION on Inventory(WarehouseID ASC,Location ASC);
                                CREATE INDEX IDX_WAREHOUSEID_STATUSCODE on Inventory(WarehouseID ASC,StatusCode ASC);
                            ";

                            com.CommandText = inventory_index_script;
                            com.ExecuteNonQuery();

                            var inventoryCmds = viewModel.Inventory.ToInsertScript("Inventory");
                            foreach (var item in inventoryCmds)
                            {
                                com.CommandText = item;
                                com.ExecuteNonQuery();
                            }

                            
                            trans.Commit();
                        };

                    }
                }

                //壓縮
                if (!isDebug && File.Exists(targetPath))
                {
                    var saveZipPath = Path.Combine(downloadFolder, zipFileName);
                    //壓縮
                    using (ZipFile zip = new ZipFile())
                    {
                        zip.AddFile(targetPath, "");
                        zip.Save(saveZipPath);
                    }

                    result = saveZipPath;   
                }
                else
                {
                    result = targetPath;
                }

            }
            catch (Exception ex)
            {

                throw ex;
            }
            finally
            {
                if(con!=null)
                {
                    con = null;
                }
            }

            return result;

        }

    }
}