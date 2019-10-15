using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PFG.Inventory.DataSource;

namespace PFG.Inventory.Data.Infrastructure
{
    public class DatabaseFactory : Disposable, IDatabaseFactory
    {
        private PFGWarehouseEntities dataContext;
        public PFGWarehouseEntities Get()
        {
            return dataContext ?? (dataContext = new PFGWarehouseEntities());
        }
        protected override void DisposeCore()
        {
            if (dataContext != null)
                dataContext.Dispose();
        }
    }
}
