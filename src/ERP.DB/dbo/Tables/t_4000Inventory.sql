CREATE TABLE [dbo].[t_4000Inventory]
(
	[Id] INT IDENTITY (1, 1) NOT NULL, 
    [SupplierId] INT NOT NULL,
    [Name] NVARCHAR(128) NOT NULL, 
    [LocationId] INT NOT NULL, 
    [Category] NVARCHAR(64) NULL, 
    [LastPurchaseDate] DATETIME NULL, 
    [Number] NVARCHAR(64) NULL, 
    [Unit] NVARCHAR(16) NULL, 
    [Quantity] DECIMAL(12, 2) NULL, 
    [Amount] DECIMAL(12, 2) NULL, 
    [Total] DECIMAL(12, 2) NULL,
    CONSTRAINT [PK_4000Inventory] PRIMARY KEY CLUSTERED ([Id] ASC),
)
-- 供應商（JOIN / 報表）
GO
CREATE NONCLUSTERED INDEX IX_t_4000Inventory_SupplierId
ON dbo.t_4000Inventory (SupplierId);
-- 分類（列表 / 盤點）
GO
CREATE NONCLUSTERED INDEX IX_t_4000Inventory_Category
ON dbo.t_4000Inventory (Category);
-- 料號（精準查詢）
GO
CREATE NONCLUSTERED INDEX IX_t_4000Inventory_Number
ON dbo.t_4000Inventory (Number);
-- 品名（搜尋）
GO
CREATE NONCLUSTERED INDEX IX_t_4000Inventory_Name
ON dbo.t_4000Inventory (Name);
-- 倉位（盤點）
GO
CREATE NONCLUSTERED INDEX IX_t_4000Inventory_Location
ON dbo.t_4000Inventory ([LocationId]);
