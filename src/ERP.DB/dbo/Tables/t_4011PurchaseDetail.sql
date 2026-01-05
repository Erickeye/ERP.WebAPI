CREATE TABLE [dbo].[_4011PurchaseDetail]
(
	[Id] INT IDENTITY (1, 1) NOT NULL, 
    [PurchaseId] INT NOT NULL, 
    [Category] NVARCHAR(16) NULL, 
    [No] NVARCHAR(64) NULL, 
    [Name] NVARCHAR(128) NOT NULL, 
    [Unit] NVARCHAR(16) NULL, 
    [Quantity] DECIMAL(12, 2) NOT NULL, 
    [Price] DECIMAL(12, 2) NULL, 
    [Total] DECIMAL(12, 2) NULL,
    CONSTRAINT [PK_4011PurchaseDetail] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_4011PurchaseDetail_PurchaseId] FOREIGN KEY ([PurchaseId]) REFERENCES [dbo].[t_4010Purchase] ([Id]),
)
