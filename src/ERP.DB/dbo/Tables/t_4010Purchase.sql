CREATE TABLE [dbo].[t_4010Purchase]
(
	[Id] INT IDENTITY (1, 1) NOT NULL,
    [No] VARCHAR(64) NULL, 
    [SupplierId] INT NOT NULL, 
    [CustomerId] INT NULL, 
    [LocationId] INT NOT NULL, 
    [ProjectName] NVARCHAR(128) NOT NULL, 
    [PurchaseDate] DATETIME NOT NULL, 
    [IsPurchase] BIT NOT NULL DEFAULT 1, 
    [PaymentMethodId] INT NOT NULL, 
    [Amount] DECIMAL(12, 2) NOT NULL, 
    [Tax] DECIMAL(12, 2) NOT NULL,
    [Payer] NVARCHAR(32) NOT NULL, 
    [InvoiceNumber] NCHAR(10) NULL,
    [Note] NVARCHAR(1024) NULL, 
    [Authorizator] NVARCHAR(32) NULL, 
    [IsApproval] BIT NOT NULL DEFAULT 0, 
    [CreateTime] DATETIME NOT NULL, 
    [CreateUserId] INT NOT NULL,
    -- Primary Key
CONSTRAINT [PK_t_4010Purchase] PRIMARY KEY CLUSTERED ([Id] DESC),
-- Foreign Keys
CONSTRAINT [FK_t_4010Purchase_Supplier]      FOREIGN KEY ([SupplierId])    REFERENCES [dbo].[t_4060Supplier] ([Id]),
CONSTRAINT [FK_t_4010Purchase_Customer]      FOREIGN KEY ([CustomerId])    REFERENCES [dbo].[t_2000Customer] ([Id]),
CONSTRAINT [FK_t_4010Purchase_Location]      FOREIGN KEY ([LocationId])    REFERENCES [dbo].[SystemConfig] ([Id]),
CONSTRAINT [FK_t_4010Purchase_PaymentMethod] FOREIGN KEY ([PaymentMethodId]) REFERENCES [dbo].[SystemConfig] ([Id]),
CONSTRAINT [FK_t_4010Purchase_CreateUser]    FOREIGN KEY ([CreateUserId])  REFERENCES [dbo].[User] ([Id])
)
GO
CREATE NONCLUSTERED INDEX [IX_t_4010Purchase_SupplierId]
ON [dbo].[t_4010Purchase] ([SupplierId]);
GO
CREATE NONCLUSTERED INDEX [IX_t_4010Purchase_PurchaseDate]
ON [dbo].[t_4010Purchase] ([PurchaseDate]);
GO
CREATE NONCLUSTERED INDEX [IX_t_4010Purchase_LocationId]
ON [dbo].[t_4010Purchase] ([LocationId]);
GO
CREATE NONCLUSTERED INDEX [IX_t_4010Purchase_IsApproval]
ON [dbo].[t_4010Purchase] ([IsApproval]);
GO
CREATE NONCLUSTERED INDEX [IX_t_4010Purchase_No]
ON [dbo].[t_4010Purchase] ([No]);
GO
CREATE NONCLUSTERED INDEX [IX_t_4010Purchase_Supplier_PurchaseDate]
ON [dbo].[t_4010Purchase] ([SupplierId], [PurchaseDate]);
GO

