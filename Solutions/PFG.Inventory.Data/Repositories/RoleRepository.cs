using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PFG.Inventory.Data.Infrastructure;
using PFG.Inventory.DataSource;

namespace PFG.Inventory.Data.Repositories
{
    public class RoleRepository : RepositoryBase<Roles>, IRoleRepository
    {
        public RoleRepository(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        {
        }
    }
    public interface IRoleRepository : IRepository<Roles>
    {
    }
}
