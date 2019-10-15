using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PFG.Inventory.Data.Infrastructure;
using PFG.Inventory.DataSource;

namespace PFG.Inventory.Data.Repositories
{
    public class UserRepository : RepositoryBase<Users>, IUserRepository
    {
        public UserRepository(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        {
        }
    }
    public interface IUserRepository : IRepository<Users>
    {
    }
}
