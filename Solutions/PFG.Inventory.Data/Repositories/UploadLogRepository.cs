using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PFG.Inventory.Data.Infrastructure;
using PFG.Inventory.DataSource;

namespace PFG.Inventory.Data.Repositories
{
    public class UploadLogRepository : RepositoryBase<UploadLog>, IUploadLogRepository
    {
        public UploadLogRepository(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        {
        }
    }
    public interface IUploadLogRepository : IRepository<UploadLog>
    {
    }
}
