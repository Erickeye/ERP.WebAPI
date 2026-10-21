CREATE TABLE [dbo].[t_4061SupplierPrice]
(
	[Id] INT IDENTITY (1, 1) NOT NULL,
    [SupplierId] INT NOT NULL,
    [No] NVARCHAR(64) NOT NULL, -- 料號（與進貨單明細的料號文字比對）
    [Name] NVARCHAR(128) NOT NULL,
    [Unit] NVARCHAR(16) NOT NULL,
    [Price] DECIMAL(12, 2) NOT NULL, -- 新台幣未稅單價；只存現價
    [Note] NVARCHAR(256) NULL,
    [CreateTime] DATETIME NOT NULL,
    [CreateUserId] INT NOT NULL,
    [UpdateTime] DATETIME NOT NULL,
    [UpdateUserId] INT NOT NULL,
    CONSTRAINT [PK_4061SupplierPrice] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_4061SupplierPrice_SupplierId] FOREIGN KEY ([SupplierId]) REFERENCES [dbo].[t_4060Supplier] ([Id]) ON DELETE CASCADE
)
GO
-- 同一供應商的同一料號只有一筆報價
CREATE UNIQUE NONCLUSTERED INDEX IX_t_4061SupplierPrice_SupplierId_No
ON dbo.t_4061SupplierPrice (SupplierId, No);
GO
