using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PFG.Inventory.Data.Infrastructure;
using PFG.Inventory.DataSource;

namespace PFG.Inventory.Data.Repositories
{
    public class OperationRepository : RepositoryBase<Operations>, IOperationRepository
    {
        public OperationRepository(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        {
        }
    }
    public interface IOperationRepository : IRepository<Operations>
    {
    }
}
