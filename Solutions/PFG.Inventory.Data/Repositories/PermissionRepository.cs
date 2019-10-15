using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PFG.Inventory.Data.Infrastructure;
using PFG.Inventory.DataSource;

namespace PFG.Inventory.Data.Repositories
{
    public class PermissionRepository : RepositoryBase<Permissions>, IPermissionRepository
    {
        public PermissionRepository(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        {
        }
    }
    public interface IPermissionRepository : IRepository<Permissions>
    {
    }
}
