using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PFG.Inventory.Data.Infrastructure;
using PFG.Inventory.DataSource;

namespace PFG.Inventory.Data.Repositories
{
    public class PermissionOperationRepository : RepositoryBase<PermissionOperations>, IPermissionOperationRepository
    {
        public PermissionOperationRepository(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        {
        }
    }
    public interface IPermissionOperationRepository : IRepository<PermissionOperations>
    {
    }
}
