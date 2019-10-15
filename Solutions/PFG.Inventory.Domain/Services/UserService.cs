using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PFG.Inventory.Data.Infrastructure;
using PFG.Inventory.Data.Repositories;
using PFG.Inventory.DataSource;
using PFG.Inventory.Domain.Enums;

namespace PFG.Inventory.Domain.Services
{
    public interface IUserService
    {
        Users GetUser(ref LogOnStatus logOnStatus, string account, string password);
        void Save();
    }

    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork unitOfWork;

        public UserService(IUserRepository userRepository, IUnitOfWork unitOfWork)
        {
            this._userRepository = userRepository;
            this.unitOfWork = unitOfWork;
        }

        public Users GetUser(ref LogOnStatus logOnStatus, string account, string password)
        {
            var currentDate = DateTime.Now;
            var user = _userRepository.Get(x => x.Account == account);
            if (user != null)
            {
                if (user.PasswordHash == password)
                {
                    //成功的流程 寫入登入時間
                    logOnStatus = LogOnStatus.Successful;
                    user.DateLastLogin = currentDate;
                    _userRepository.Update(user);

                    Save();
                    return user;
                }
                else
                {
                    logOnStatus = LogOnStatus.FailPassword;

                }
            }
            else
            {
                logOnStatus = LogOnStatus.NotExist;
            }
            return null;
        }

        public void Save()
        {
            unitOfWork.Commit();
        }
    }

}
