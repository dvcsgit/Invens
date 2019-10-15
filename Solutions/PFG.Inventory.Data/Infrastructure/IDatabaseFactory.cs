using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PFG.Inventory.DataSource;

namespace PFG.Inventory.Data.Infrastructure
{
    public interface IDatabaseFactory : IDisposable
    {
        PFGWarehouseEntities Get();
    }
}
