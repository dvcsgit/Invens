﻿//------------------------------------------------------------------------------
// <auto-generated>
//     此代码已从模板生成。
//
//     手动更改此文件可能导致应用程序出现意外的行为。
//     如果重新生成代码，将覆盖对此文件的手动更改。
// </auto-generated>
//------------------------------------------------------------------------------

namespace PFG.Inventory.DataSource
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class PFGWarehouseEntities : DbContext
    {
        public PFGWarehouseEntities()
            : base("name=PFGWarehouseEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<BasicSettingLoaction> BasicSettingLoaction { get; set; }
        public virtual DbSet<BasicSettingWarehouse> BasicSettingWarehouse { get; set; }
        public virtual DbSet<Operations> Operations { get; set; }
        public virtual DbSet<PermissionOperations> PermissionOperations { get; set; }
        public virtual DbSet<Users> Users { get; set; }
        public virtual DbSet<Permissions> Permissions { get; set; }
        public virtual DbSet<Roles> Roles { get; set; }
        public virtual DbSet<OutBoundInventory> OutBoundInventory { get; set; }
        public virtual DbSet<BasicSettingCapacity> BasicSettingCapacity { get; set; }
        public virtual DbSet<Inventory> Inventory { get; set; }
        public virtual DbSet<UploadLogDetail> UploadLogDetail { get; set; }
        public virtual DbSet<UploadLog> UploadLog { get; set; }
        public virtual DbSet<InventoryStock> InventoryStock { get; set; }
        public virtual DbSet<MIS_Mapping_LOC> MIS_Mapping_LOC { get; set; }
        public virtual DbSet<MIS_Mapping_PDNO> MIS_Mapping_PDNO { get; set; }
    }
}
