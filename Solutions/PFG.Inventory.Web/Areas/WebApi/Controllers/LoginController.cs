using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using NLog;
using PFG.Inventory.DataSource;
using PFG.Inventory.Web.Areas.WebApi.Models;

namespace PFG.Inventory.Web.Areas.WebApi.Controllers
{
    public class LoginController : ApiController
    {
        protected static Logger _logger = LogManager.GetLogger("LoginApi");

        // GET api/login
        public UserLoginInfo Get(string account, string password)
        {
            UserLoginInfo viewModel = new UserLoginInfo();
            try
            {

                using (PFGWarehouseEntities db = new PFGWarehouseEntities())
                {
                    var user = db.Users.Where(x => x.Account == account).FirstOrDefault();
                    if (user != null)
                    {
                        if (user.PasswordHash == password)
                        {
                            viewModel.Account = user.Account;
                            viewModel.Name = user.Name;
                            viewModel.IsLoginValid = true;
                            //成功登入
                            //更新登入時間
                            user.DateLastLogin = DateTime.Now;
                            db.SaveChanges();

                        }
                        else
                        {
                            viewModel.ErrorMessage = "密碼錯誤";
                        }
                    }
                    else
                    {
                        viewModel.ErrorMessage = string.Format("使用者:{0}不存在", account);
                    }
                }
            }
            catch (Exception ex)
            {

                viewModel.ErrorMessage = ex.Message;
                _logger.Error("發生錯誤:{0}", ex);
            }


            return viewModel;
        }
    }
}
