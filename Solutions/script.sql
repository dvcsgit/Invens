USE [master]
GO
/****** Object:  Database [PFGWarehouse]    Script Date: 2019/10/15 11:12:20 ******/
CREATE DATABASE [PFGWarehouse]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'PFGWarehouse', FILENAME = N'E:\DATA\PFGWarehouse.mdf' , SIZE = 211392KB , MAXSIZE = UNLIMITED, FILEGROWTH = 1024KB )
 LOG ON 
( NAME = N'PFGWarehouse_log', FILENAME = N'E:\DATA\PFGWarehouse_log.ldf' , SIZE = 768KB , MAXSIZE = UNLIMITED, FILEGROWTH = 10%)
GO
ALTER DATABASE [PFGWarehouse] SET COMPATIBILITY_LEVEL = 110
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [PFGWarehouse].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [PFGWarehouse] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [PFGWarehouse] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [PFGWarehouse] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [PFGWarehouse] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [PFGWarehouse] SET ARITHABORT OFF 
GO
ALTER DATABASE [PFGWarehouse] SET AUTO_CLOSE ON 
GO
ALTER DATABASE [PFGWarehouse] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [PFGWarehouse] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [PFGWarehouse] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [PFGWarehouse] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [PFGWarehouse] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [PFGWarehouse] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [PFGWarehouse] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [PFGWarehouse] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [PFGWarehouse] SET  DISABLE_BROKER 
GO
ALTER DATABASE [PFGWarehouse] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [PFGWarehouse] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [PFGWarehouse] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [PFGWarehouse] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [PFGWarehouse] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [PFGWarehouse] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [PFGWarehouse] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [PFGWarehouse] SET RECOVERY SIMPLE 
GO
ALTER DATABASE [PFGWarehouse] SET  MULTI_USER 
GO
ALTER DATABASE [PFGWarehouse] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [PFGWarehouse] SET DB_CHAINING OFF 
GO
ALTER DATABASE [PFGWarehouse] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [PFGWarehouse] SET TARGET_RECOVERY_TIME = 0 SECONDS 
GO
ALTER DATABASE [PFGWarehouse] SET DELAYED_DURABILITY = DISABLED 
GO
ALTER DATABASE [PFGWarehouse] SET QUERY_STORE = OFF
GO
USE [PFGWarehouse]
GO
/****** Object:  Table [dbo].[UploadLogDetail]    Script Date: 2019/10/15 11:12:21 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UploadLogDetail](
	[UploadLogDetailID] [int] IDENTITY(1,1) NOT NULL,
	[UploadLogID] [int] NOT NULL,
	[LogType] [nvarchar](50) NOT NULL,
	[Message] [nvarchar](200) NOT NULL,
	[DateCreated] [datetime] NOT NULL,
 CONSTRAINT [PK_UploadLogDetail] PRIMARY KEY CLUSTERED 
(
	[UploadLogDetailID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[UploadLog]    Script Date: 2019/10/15 11:12:21 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UploadLog](
	[UploadLogID] [int] IDENTITY(1,1) NOT NULL,
	[Account] [nvarchar](20) NOT NULL,
	[UploadFileName] [nvarchar](500) NULL,
	[TempFileName] [nvarchar](500) NULL,
	[DataUpload] [datetime] NOT NULL,
	[Flag] [int] NOT NULL,
	[ExceptionMessage] [text] NULL,
	[TotalRecords] [int] NULL,
	[ControllerName] [nvarchar](20) NOT NULL,
	[Summary] [nvarchar](100) NULL,
 CONSTRAINT [PK_UploadLog] PRIMARY KEY CLUSTERED 
(
	[UploadLogID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  View [dbo].[LOG_TRACE]    Script Date: 2019/10/15 11:12:21 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[LOG_TRACE]
AS
SELECT         dbo.UploadLog.Account, dbo.UploadLog.UploadLogID, 
                          dbo.UploadLog.UploadFileName, dbo.UploadLog.TempFileName, 
                          dbo.UploadLog.DataUpload, dbo.UploadLog.Flag, dbo.UploadLog.Summary, 
                          dbo.UploadLog.ControllerName, dbo.UploadLog.TotalRecords, 
                          dbo.UploadLog.ExceptionMessage, dbo.UploadLogDetail.LogType, 
                          dbo.UploadLogDetail.Message, dbo.UploadLogDetail.DateCreated, 
                          dbo.UploadLogDetail.UploadLogID AS Expr1
FROM             dbo.UploadLog INNER JOIN
                          dbo.UploadLogDetail ON 
                          dbo.UploadLog.UploadLogID = dbo.UploadLogDetail.UploadLogID
GO
/****** Object:  Table [dbo].[BasicSettingCapacity]    Script Date: 2019/10/15 11:12:21 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[BasicSettingCapacity](
	[ProductCode] [nvarchar](2) NOT NULL,
	[CapacityProduct] [decimal](3, 2) NULL,
	[CreatorAccount] [nvarchar](20) NOT NULL,
	[DateCreated] [datetime] NOT NULL,
	[ModifierAccount] [nvarchar](20) NULL,
	[DateModified] [datetime] NULL,
 CONSTRAINT [PK_BasicSettingCapacity] PRIMARY KEY CLUSTERED 
(
	[ProductCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[BasicSettingLoaction]    Script Date: 2019/10/15 11:12:21 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[BasicSettingLoaction](
	[WarehouseID] [nvarchar](10) NOT NULL,
	[Location] [int] NOT NULL,
	[CreatorAccount] [nvarchar](20) NULL,
	[DateCreated] [datetime] NULL,
	[ModifierAccount] [nvarchar](20) NULL,
	[DateModified] [datetime] NULL,
 CONSTRAINT [PK_BasicSettingLoaction] PRIMARY KEY CLUSTERED 
(
	[WarehouseID] ASC,
	[Location] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[BasicSettingWarehouse]    Script Date: 2019/10/15 11:12:21 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[BasicSettingWarehouse](
	[WarehouseID] [nvarchar](10) NOT NULL,
	[WarehouseName] [nvarchar](20) NULL,
	[Capacity] [int] NULL,
	[CreatorAccount] [nvarchar](20) NULL,
	[DateCreated] [datetime] NULL,
	[ModifierAccount] [nvarchar](20) NULL,
	[DateModified] [datetime] NULL,
	[IsEnabled] [bit] NOT NULL,
 CONSTRAINT [PK_BasicSettingWarehouse] PRIMARY KEY CLUSTERED 
(
	[WarehouseID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Inventory]    Script Date: 2019/10/15 11:12:21 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Inventory](
	[BoxNumber] [nvarchar](7) NOT NULL,
	[WarehouseID] [nvarchar](10) NOT NULL,
	[Location] [int] NOT NULL,
	[ProductCode] [nvarchar](10) NULL,
	[Class] [nvarchar](2) NULL,
	[GrossWeight] [nvarchar](10) NULL,
	[NetWeight] [nvarchar](10) NULL,
	[Remark] [nvarchar](10) NULL,
	[CarNo] [nvarchar](50) NULL,
	[StatusCode] [nvarchar](2) NULL,
	[CreatorAccount] [nvarchar](20) NULL,
	[DateCreated] [datetime] NULL,
	[ModifierAccount] [nvarchar](20) NULL,
	[DateModified] [datetime] NULL,
	[UploadAccount] [nvarchar](20) NULL,
	[DataUpload] [datetime] NULL,
 CONSTRAINT [PK_Inventory_1] PRIMARY KEY CLUSTERED 
(
	[BoxNumber] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Inventory_His]    Script Date: 2019/10/15 11:12:21 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Inventory_His](
	[BoxNumber] [nvarchar](7) NOT NULL,
	[DateHistory] [datetime] NOT NULL,
	[WarehouseID] [nvarchar](10) NOT NULL,
	[Location] [int] NOT NULL,
	[ProductCode] [nvarchar](10) NULL,
	[Class] [nvarchar](2) NULL,
	[GrossWeight] [nvarchar](10) NULL,
	[NetWeight] [nvarchar](10) NULL,
	[Remark] [nvarchar](10) NULL,
	[CarNo] [nvarchar](50) NULL,
	[StatusCode] [nvarchar](2) NULL,
	[CreatorAccount] [nvarchar](20) NULL,
	[DateCreated] [datetime] NULL,
	[ModifierAccount] [nvarchar](20) NULL,
	[DateModified] [datetime] NULL,
	[UploadAccount] [nvarchar](20) NULL,
	[DataUpload] [datetime] NULL,
 CONSTRAINT [PK_Inventory_His] PRIMARY KEY CLUSTERED 
(
	[BoxNumber] ASC,
	[DateHistory] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[InventoryStock]    Script Date: 2019/10/15 11:12:21 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[InventoryStock](
	[DateStock] [nvarchar](8) NOT NULL,
	[BoxNumber] [nvarchar](7) NOT NULL,
	[WarehouseID] [nvarchar](10) NOT NULL,
	[Location] [int] NOT NULL,
	[ProductCode] [nvarchar](10) NULL,
	[Class] [nvarchar](2) NULL,
	[GrossWeight] [nvarchar](10) NULL,
	[NetWeight] [nvarchar](10) NULL,
	[Remark] [nvarchar](10) NULL,
	[CarNo] [nvarchar](50) NULL,
	[StatusCode] [nvarchar](2) NULL,
	[CreatorAccount] [nvarchar](20) NULL,
	[DateCreated] [datetime] NULL,
	[ModifierAccount] [nvarchar](20) NULL,
	[DateModified] [datetime] NULL,
	[MakeInventory] [nvarchar](1) NULL,
	[UploadAccount] [nvarchar](20) NULL,
	[DataUpload] [datetime] NULL,
	[DateStockTime] [datetime] NULL,
 CONSTRAINT [PK_InventoryStock] PRIMARY KEY CLUSTERED 
(
	[DateStock] ASC,
	[BoxNumber] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[MIS_Mapping_LOC]    Script Date: 2019/10/15 11:12:21 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[MIS_Mapping_LOC](
	[PLANT] [nvarchar](1) NOT NULL,
	[PDID] [nvarchar](5) NOT NULL,
	[GD] [nvarchar](2) NOT NULL,
	[LOC] [nvarchar](4) NULL,
	[CREATETIME] [datetime] NULL,
	[FLAG] [nvarchar](1) NULL,
 CONSTRAINT [PK_MIS_Mapping_LOC] PRIMARY KEY CLUSTERED 
(
	[PLANT] ASC,
	[PDID] ASC,
	[GD] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[MIS_Mapping_PDNO]    Script Date: 2019/10/15 11:12:21 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[MIS_Mapping_PDNO](
	[PLANT] [nvarchar](1) NOT NULL,
	[PDID] [nvarchar](5) NOT NULL,
	[PDNO] [nvarchar](40) NULL,
	[CREATETIME] [datetime] NULL,
	[FLAG] [nvarchar](1) NULL,
 CONSTRAINT [PK_MIS_Mapping_PDNO] PRIMARY KEY CLUSTERED 
(
	[PLANT] ASC,
	[PDID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Operations]    Script Date: 2019/10/15 11:12:21 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Operations](
	[OperationID] [nvarchar](10) NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
	[Description] [nvarchar](100) NULL,
 CONSTRAINT [PK_dbo.Operations] PRIMARY KEY CLUSTERED 
(
	[OperationID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[OutBoundInventory]    Script Date: 2019/10/15 11:12:21 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[OutBoundInventory](
	[No] [bigint] IDENTITY(1,1) NOT NULL,
	[BoxNumber] [nvarchar](7) NOT NULL,
	[WarehouseID] [nvarchar](10) NOT NULL,
	[Location] [int] NOT NULL,
	[ProductCode] [nvarchar](10) NULL,
	[Class] [nvarchar](2) NULL,
	[GrossWeight] [nvarchar](10) NULL,
	[NetWeight] [nvarchar](10) NULL,
	[CarNo] [nvarchar](50) NULL,
	[StatusCode] [nvarchar](2) NULL,
	[CreatorAccount] [nvarchar](20) NULL,
	[DateCreated] [datetime] NULL,
	[ModifierAccount] [nvarchar](20) NULL,
	[DateModified] [datetime] NULL,
	[UploadAccount] [nvarchar](20) NULL,
	[DataUpload] [datetime] NULL,
	[PrintFlag] [nvarchar](1) NULL,
	[MISFlag] [nvarchar](1) NULL,
	[MISTime] [datetime] NULL,
 CONSTRAINT [PK_OutBoundInventory] PRIMARY KEY CLUSTERED 
(
	[No] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[PermissionOperations]    Script Date: 2019/10/15 11:12:21 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PermissionOperations](
	[PermissionOperationID] [int] IDENTITY(1,1) NOT NULL,
	[PermissionID] [nvarchar](5) NOT NULL,
	[OperationID] [nvarchar](10) NOT NULL,
 CONSTRAINT [PK_dbo.PermissionOperations] PRIMARY KEY CLUSTERED 
(
	[PermissionOperationID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Permissions]    Script Date: 2019/10/15 11:12:21 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Permissions](
	[PermissionID] [nvarchar](5) NOT NULL,
	[ParentID] [nvarchar](5) NULL,
	[PermissionName] [nvarchar](50) NOT NULL,
	[Controller] [nvarchar](50) NULL,
	[Action] [nvarchar](50) NULL,
	[Weight] [int] NOT NULL,
	[Url] [nvarchar](50) NULL,
	[Icon] [nvarchar](20) NULL,
 CONSTRAINT [PK_dbo.Permissions] PRIMARY KEY CLUSTERED 
(
	[PermissionID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[RolePermissionOperations]    Script Date: 2019/10/15 11:12:21 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[RolePermissionOperations](
	[RoleID] [nvarchar](10) NOT NULL,
	[PermissionOperationID] [int] NOT NULL,
 CONSTRAINT [PK_dbo.RolePermissionOperations] PRIMARY KEY CLUSTERED 
(
	[RoleID] ASC,
	[PermissionOperationID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Roles]    Script Date: 2019/10/15 11:12:21 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Roles](
	[RoleID] [nvarchar](10) NOT NULL,
	[RoleName] [nvarchar](50) NOT NULL,
	[Description] [nvarchar](100) NOT NULL,
	[IsDefault] [bit] NOT NULL,
	[CreatorAccount] [nvarchar](20) NOT NULL,
	[DateCreated] [datetime] NOT NULL,
	[ModifierAccount] [nvarchar](20) NULL,
	[DateModified] [datetime] NULL,
 CONSTRAINT [PK_dbo.Roles] PRIMARY KEY CLUSTERED 
(
	[RoleID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[UserRoles]    Script Date: 2019/10/15 11:12:21 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UserRoles](
	[Account] [nvarchar](20) NOT NULL,
	[RoleID] [nvarchar](10) NOT NULL,
 CONSTRAINT [PK_dbo.UserRoles] PRIMARY KEY CLUSTERED 
(
	[Account] ASC,
	[RoleID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Users]    Script Date: 2019/10/15 11:12:21 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Users](
	[Account] [nvarchar](20) NOT NULL,
	[Email] [nvarchar](200) NULL,
	[Name] [nvarchar](20) NOT NULL,
	[PasswordHash] [nvarchar](max) NULL,
	[DateLastLogin] [datetime] NULL,
	[DateLastActivity] [datetime] NULL,
	[Tel] [nvarchar](20) NULL,
	[Ext] [nvarchar](6) NULL,
	[CreatorAccount] [nvarchar](20) NOT NULL,
	[DateCreated] [datetime] NOT NULL,
	[ModifierAccount] [nvarchar](20) NULL,
	[DateModified] [datetime] NULL,
 CONSTRAINT [PK_Users] PRIMARY KEY CLUSTERED 
(
	[Account] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [uc_Account] UNIQUE NONCLUSTERED 
(
	[Account] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [NonClusteredIndex-WarehouseID]    Script Date: 2019/10/15 11:12:21 ******/
CREATE NONCLUSTERED INDEX [NonClusteredIndex-WarehouseID] ON [dbo].[Inventory]
(
	[WarehouseID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [GROUP]    Script Date: 2019/10/15 11:12:21 ******/
CREATE NONCLUSTERED INDEX [GROUP] ON [dbo].[OutBoundInventory]
(
	[WarehouseID] ASC,
	[ProductCode] ASC,
	[Class] ASC,
	[DataUpload] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [NonClusteredIndex-20151106-094117]    Script Date: 2019/10/15 11:12:21 ******/
CREATE NONCLUSTERED INDEX [NonClusteredIndex-20151106-094117] ON [dbo].[OutBoundInventory]
(
	[DataUpload] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
ALTER TABLE [dbo].[BasicSettingWarehouse] ADD  CONSTRAINT [DF_BasicSettingWarehouse_IsEnabled]  DEFAULT ((1)) FOR [IsEnabled]
GO
ALTER TABLE [dbo].[MIS_Mapping_LOC] ADD  CONSTRAINT [DF_MIS_Mapping_LOC_CREATETIME]  DEFAULT (getdate()) FOR [CREATETIME]
GO
ALTER TABLE [dbo].[MIS_Mapping_LOC] ADD  CONSTRAINT [DF_MIS_Mapping_LOC_FLAG]  DEFAULT (N'N') FOR [FLAG]
GO
ALTER TABLE [dbo].[MIS_Mapping_PDNO] ADD  CONSTRAINT [DF_MIS_Mapping_PDNO_CREATETIME]  DEFAULT (getdate()) FOR [CREATETIME]
GO
ALTER TABLE [dbo].[MIS_Mapping_PDNO] ADD  CONSTRAINT [DF_MIS_Mapping_PDNO_FLAG]  DEFAULT (N'N') FOR [FLAG]
GO
ALTER TABLE [dbo].[OutBoundInventory] ADD  CONSTRAINT [DF_OutBoundInventory_PrintFlag]  DEFAULT (N'N') FOR [PrintFlag]
GO
ALTER TABLE [dbo].[OutBoundInventory] ADD  CONSTRAINT [DF_OutBoundInventory_MISFlag]  DEFAULT (N'N') FOR [MISFlag]
GO
ALTER TABLE [dbo].[PermissionOperations]  WITH CHECK ADD  CONSTRAINT [FK_dbo.PermissionOperations_dbo.Operations_OperationId] FOREIGN KEY([OperationID])
REFERENCES [dbo].[Operations] ([OperationID])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[PermissionOperations] CHECK CONSTRAINT [FK_dbo.PermissionOperations_dbo.Operations_OperationId]
GO
ALTER TABLE [dbo].[PermissionOperations]  WITH CHECK ADD  CONSTRAINT [FK_dbo.PermissionOperations_dbo.Permissions_PermissionId] FOREIGN KEY([PermissionID])
REFERENCES [dbo].[Permissions] ([PermissionID])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[PermissionOperations] CHECK CONSTRAINT [FK_dbo.PermissionOperations_dbo.Permissions_PermissionId]
GO
ALTER TABLE [dbo].[RolePermissionOperations]  WITH CHECK ADD  CONSTRAINT [FK_dbo.RolePermissionOperations_dbo.PermissionOperations_PermissionOperationId] FOREIGN KEY([PermissionOperationID])
REFERENCES [dbo].[PermissionOperations] ([PermissionOperationID])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[RolePermissionOperations] CHECK CONSTRAINT [FK_dbo.RolePermissionOperations_dbo.PermissionOperations_PermissionOperationId]
GO
ALTER TABLE [dbo].[RolePermissionOperations]  WITH CHECK ADD  CONSTRAINT [FK_dbo.RolePermissionOperations_dbo.Roles_RoleId] FOREIGN KEY([RoleID])
REFERENCES [dbo].[Roles] ([RoleID])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[RolePermissionOperations] CHECK CONSTRAINT [FK_dbo.RolePermissionOperations_dbo.Roles_RoleId]
GO
ALTER TABLE [dbo].[UploadLogDetail]  WITH CHECK ADD  CONSTRAINT [FK_UploadLogDetail_UploadLog] FOREIGN KEY([UploadLogID])
REFERENCES [dbo].[UploadLog] ([UploadLogID])
GO
ALTER TABLE [dbo].[UploadLogDetail] CHECK CONSTRAINT [FK_UploadLogDetail_UploadLog]
GO
ALTER TABLE [dbo].[UserRoles]  WITH CHECK ADD  CONSTRAINT [FK_dbo.UserRoles_dbo.Roles_RoleId] FOREIGN KEY([RoleID])
REFERENCES [dbo].[Roles] ([RoleID])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[UserRoles] CHECK CONSTRAINT [FK_dbo.UserRoles_dbo.Roles_RoleId]
GO
ALTER TABLE [dbo].[UserRoles]  WITH CHECK ADD  CONSTRAINT [FK_UserRoles_Users] FOREIGN KEY([Account])
REFERENCES [dbo].[Users] ([Account])
GO
ALTER TABLE [dbo].[UserRoles] CHECK CONSTRAINT [FK_UserRoles_Users]
GO
/****** Object:  StoredProcedure [dbo].[SP_Del_MIS_PO]    Script Date: 2019/10/15 11:12:21 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE  [dbo].[SP_Del_MIS_PO]

AS
BEGIN
update OPENQUERY(TPRS22A,'SELECT * FROM uxugecp.wxugfam8')
set CFMMK='D'
where CFMMK is NULL 
and DATEDIFF(Day,Convert(datetime,Convert(varchar(4),Convert(int,Substring(TRODAT,1,3))+1911)+substring(TRODAT,4,2)+substring(TRODAT,6,2)),getdate())>3
END
GO
/****** Object:  StoredProcedure [dbo].[SP_InsertMIS_OutBound]    Script Date: 2019/10/15 11:12:21 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[SP_InsertMIS_OutBound]
AS
declare @WarehouseID varchar(10),@DateUpload datetime;


BEGIN
   
    --因目前產品編號T7ZH1生產廠未建立相對應撥入庫，所以暫時不轉檔，modify 2016/12/29
 --   update [PFGWarehouse].[dbo].[OutBoundInventory]
	--set MISFlag='E',DateModified=GETDATE()
	--where ProductCode='T7ZH1' and MISFlag is null
	
	DECLARE C01 CURSOR FOR
	Select WarehouseID,DataUpload
	From [PFGWarehouse].[dbo].[OutBoundInventory]
	where MISFlag in('N','P') or MISFlag is null
	group by WarehouseID,DataUpload

	SET XACT_ABORT ON
	Begin Tran
	OPEN C01
	FETCH NEXT FROM C01 INTO @WarehouseID,@DateUpload
	WHILE(@@FETCH_STATUS=0)
	BEGIN

	   update [PFGWarehouse].[dbo].[OutBoundInventory]
	   Set MISFlag='P' where WarehouseID=@WarehouseID and DataUpload=@DateUpload

	    Insert into OPENQUERY(TPRS22A, 'SELECT VhNo,TrDp,TroDat,TriDat,TriStk,TroStk,IT,PdId,PDNO,Gd,Un,Qty,Wgt,Arts From uxugecp.wxugfam8') 
        Select     
               Substring(Replace(Replace(Replace(CONVERT(char(19), DataUpload, 120),'-',''),':',''),' ',''),3,12) as VhNo
				,'0160' as TrDp
				,Convert(varchar(10),Convert(int,substring(Convert(char(8),DataUpload,112),1,4))-1911) +substring(Convert(char(8),DataUpload,112),5,4) as TroDat
				,Convert(varchar(10),Convert(int,substring(Convert(char(8),DataUpload,112),1,4))-1911) +substring(Convert(char(8),DataUpload,112),5,4) as TriDat
				,t3.LOC as TriStk
				,WarehouseID as TroStk
				,ROW_NUMBER() OVER(ORDER BY Class) AS IT

				,ProductCode as PdId
				,t2.PDNO
				,Class as Gd
				,'KG' as Un
				,Sum(Convert(int,NetWeight))  as Qty 
				,Sum(Convert(int,GrossWeight)) as Wgt
				,Count(*) as Arts
	   From [PFGWarehouse].[dbo].[OutBoundInventory] t1
	   left join [PFGWarehouse].[dbo].[MIS_Mapping_PDNO] t2 on t1.ProductCode=t2.PDID
       left join [PFGWarehouse].[dbo].[MIS_Mapping_LOC] t3 on  substring(t2.PDNO,14,1)=t3.PLANT and  t1.ProductCode=t3.PDID and t1.Class=t3.GD
       where WarehouseID=@WarehouseID	and DataUpload=@DateUpload and MISFlag='P'
       group by DataUpload,WarehouseID,ProductCode,Class,LOC,PDNO

	   --Insert into OPENQUERY(TPRS22A, 'SELECT VhNo,TrDp,TroDat,TriDat,TriStk,TroStk,IT,PdId,PDNO,Gd,Un,Qty,Wgt,Arts From uxugecp.wxugfam8') 
	   --Select     Convert(varchar(20),t4.VhNo)+'_'+Convert(varchar(20),t4.IT) as VhNo
    --           --Substring(Replace(Replace(Replace(CONVERT(char(19), DataUpload, 120),'-',''),':',''),' ',''),3,12) as VhNo
				--,'0160' as TrDp
				--,Convert(varchar(10),Convert(int,substring(Convert(char(8),DataUpload,112),1,4))-1911) +substring(Convert(char(8),DataUpload,112),5,4) as TroDat
				--,Convert(varchar(10),Convert(int,substring(Convert(char(8),DataUpload,112),1,4))-1911) +substring(Convert(char(8),DataUpload,112),5,4) as TriDat
				--,t3.LOC as TriStk
				--,WarehouseID as TroStk
				----,ROW_NUMBER() OVER(ORDER BY Class) AS IT
				--,ROW_NUMBER() OVER (PARTITION BY t3.LOC ORDER BY t2.PDNO) AS IT
				--,ProductCode as PdId
				--,t2.PDNO
				--,Class as Gd
				--,'KG' as Un
				--,Sum(Convert(int,NetWeight))  as Qty 
				--,Sum(Convert(int,GrossWeight)) as Wgt
				--,Count(*) as Arts
	   --From [PFGWarehouse].[dbo].[OutBoundInventory] t1
	   --left join [PFGWarehouse].[dbo].[MIS_Mapping_PDNO] t2 on t1.ProductCode=t2.PDID
    --   left join [PFGWarehouse].[dbo].[MIS_Mapping_LOC] t3 on  substring(t2.PDNO,14,1)=t3.PLANT and  t1.ProductCode=t3.PDID and t1.Class=t3.GD
	   --left join (
	   --			   Select     Substring(Replace(Replace(Replace(CONVERT(char(19), DataUpload, 120),'-',''),':',''),' ',''),3,12) as VhNo
				--			 ,t3.LOC as TriStk
				--			 ,ROW_NUMBER() OVER(ORDER BY LOC) AS IT
				--   From [PFGWarehouse].[dbo].[OutBoundInventory] t1
				--   left join [PFGWarehouse].[dbo].[MIS_Mapping_PDNO] t2 on t1.ProductCode=t2.PDID
				--   left join [PFGWarehouse].[dbo].[MIS_Mapping_LOC] t3 on  substring(t2.PDNO,14,1)=t3.PLANT and  t1.ProductCode=t3.PDID and t1.Class=t3.GD
				--   where WarehouseID=@WarehouseID	and DataUpload=@DateUpload and MISFlag='P'
				--   group by DataUpload,LOC
	   --) as t4 on t3.LOC=t4.TriStk
    --   where WarehouseID=@WarehouseID	and DataUpload=@DateUpload and MISFlag='P'
	   ----where WarehouseID='RMMY'	and DataUpload='2015-06-06 08:07:44.483'
    --   group by DataUpload,WarehouseID,ProductCode,Class,LOC,PDNO,t4.VhNo,t4.IT

	   update [PFGWarehouse].[dbo].[OutBoundInventory]
	   Set MISFlag='Y',MISTime=GETDATE()  where WarehouseID=@WarehouseID and DataUpload=@DateUpload

	FETCH NEXT FROM C01 INTO @WarehouseID,@DateUpload
	END
	CLOSE C01
	
	commit tran
	DEALLOCATE C01

END
GO
/****** Object:  StoredProcedure [dbo].[SP_MIS_InsertMapping]    Script Date: 2019/10/15 11:12:21 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[SP_MIS_InsertMapping] 
	
AS
BEGIN
	Begin Tran

    Delete [dbo].[MIS_Mapping_PDNO] where Flag='N'
	Insert into [dbo].[MIS_Mapping_PDNO]
	([PLANT]
	,[PDID]
	,[PDNO])
	SELECT * FROM OPENQUERY(TPRS22A,  'SELECT PLANT,PDID,PDNO FROM uxugecp.txucap02')
	
	Delete [dbo].[MIS_Mapping_LOC] where Flag='N'
	Insert into [dbo].[MIS_Mapping_LOC]
	([PLANT]
	,[PDID]
	,[GD]
	,[LOC])
	SELECT * FROM OPENQUERY(TPRS22A,  'SELECT PLANT,PDID,GD,LOC FROM uxugecp.txucap10')
	
	Commit Tran
END
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'PK,自動流水' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'UploadLog', @level2type=N'COLUMN',@level2name=N'UploadLogID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'帳號' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'UploadLog', @level2type=N'COLUMN',@level2name=N'Account'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'上傳檔案名稱' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'UploadLog', @level2type=N'COLUMN',@level2name=N'UploadFileName'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'解壓縮後的檔案位置' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'UploadLog', @level2type=N'COLUMN',@level2name=N'TempFileName'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'上傳日期' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'UploadLog', @level2type=N'COLUMN',@level2name=N'DataUpload'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'處理狀態1.成功 2.失敗' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'UploadLog', @level2type=N'COLUMN',@level2name=N'Flag'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'失敗訊息' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'UploadLog', @level2type=N'COLUMN',@level2name=N'ExceptionMessage'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'資料總筆數' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'UploadLog', @level2type=N'COLUMN',@level2name=N'TotalRecords'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'區分庫存or盤點' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'UploadLog', @level2type=N'COLUMN',@level2name=N'ControllerName'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'摘要' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'UploadLog', @level2type=N'COLUMN',@level2name=N'Summary'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'PK,自動流水' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'UploadLogDetail', @level2type=N'COLUMN',@level2name=N'UploadLogDetailID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'FK,上傳日誌主鍵' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'UploadLogDetail', @level2type=N'COLUMN',@level2name=N'UploadLogID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Debug,Warm,Error' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'UploadLogDetail', @level2type=N'COLUMN',@level2name=N'LogType'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'訊息' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'UploadLogDetail', @level2type=N'COLUMN',@level2name=N'Message'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'新增時間' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'UploadLogDetail', @level2type=N'COLUMN',@level2name=N'DateCreated'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'帳號識別' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'UserRoles', @level2type=N'COLUMN',@level2name=N'Account'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'帳號識別' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Users', @level2type=N'COLUMN',@level2name=N'Account'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'信箱' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Users', @level2type=N'COLUMN',@level2name=N'Email'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'姓名' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Users', @level2type=N'COLUMN',@level2name=N'Name'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'密碼' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Users', @level2type=N'COLUMN',@level2name=N'PasswordHash'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[40] 4[20] 2[20] 3) )"
      End
      Begin PaneConfiguration = 1
         NumPanes = 3
         Configuration = "(H (1 [50] 4 [25] 3))"
      End
      Begin PaneConfiguration = 2
         NumPanes = 3
         Configuration = "(H (1 [50] 2 [25] 3))"
      End
      Begin PaneConfiguration = 3
         NumPanes = 3
         Configuration = "(H (4 [30] 2 [40] 3))"
      End
      Begin PaneConfiguration = 4
         NumPanes = 2
         Configuration = "(H (1 [56] 3))"
      End
      Begin PaneConfiguration = 5
         NumPanes = 2
         Configuration = "(H (2 [66] 3))"
      End
      Begin PaneConfiguration = 6
         NumPanes = 2
         Configuration = "(H (4 [50] 3))"
      End
      Begin PaneConfiguration = 7
         NumPanes = 1
         Configuration = "(V (3))"
      End
      Begin PaneConfiguration = 8
         NumPanes = 3
         Configuration = "(H (1[56] 4[18] 2) )"
      End
      Begin PaneConfiguration = 9
         NumPanes = 2
         Configuration = "(H (1 [75] 4))"
      End
      Begin PaneConfiguration = 10
         NumPanes = 2
         Configuration = "(H (1[66] 2) )"
      End
      Begin PaneConfiguration = 11
         NumPanes = 2
         Configuration = "(H (4 [60] 2))"
      End
      Begin PaneConfiguration = 12
         NumPanes = 1
         Configuration = "(H (1) )"
      End
      Begin PaneConfiguration = 13
         NumPanes = 1
         Configuration = "(V (4))"
      End
      Begin PaneConfiguration = 14
         NumPanes = 1
         Configuration = "(V (2))"
      End
      ActivePaneConfig = 0
   End
   Begin DiagramPane = 
      Begin Origin = 
         Top = 0
         Left = 0
      End
      Begin Tables = 
         Begin Table = "UploadLog"
            Begin Extent = 
               Top = 6
               Left = 38
               Bottom = 302
               Right = 204
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "UploadLogDetail"
            Begin Extent = 
               Top = 6
               Left = 242
               Bottom = 302
               Right = 414
            End
            DisplayFlags = 280
            TopColumn = 0
         End
      End
   End
   Begin SQLPane = 
   End
   Begin DataPane = 
      Begin ParameterDefaults = ""
      End
   End
   Begin CriteriaPane = 
      Begin ColumnWidths = 11
         Column = 1440
         Alias = 900
         Table = 1170
         Output = 720
         Append = 1400
         NewValue = 1170
         SortType = 1350
         SortOrder = 1410
         GroupBy = 1350
         Filter = 1350
         Or = 1350
         Or = 1350
         Or = 1350
      End
   End
End
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'LOG_TRACE'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=1 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'LOG_TRACE'
GO
USE [master]
GO
ALTER DATABASE [PFGWarehouse] SET  READ_WRITE 
GO
