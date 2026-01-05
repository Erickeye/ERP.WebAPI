CREATE TABLE [dbo].[t_4060Supplier]
(
	[Id] INT IDENTITY (1, 1) NOT NULL,
    [No] NVARCHAR(64) NULL, 
    [Name] NVARCHAR(128) NULL, 
    [TaxIdNumber] NVARCHAR(8) NULL, 
    [Owner] NVARCHAR(32) NULL, 
    [ContactPhone] NVARCHAR(32) NULL, 
    [FaxPhone] NVARCHAR(32) NULL, 
    [Salesperson] NVARCHAR(32) NULL, 
    [Address] NVARCHAR(128) NULL, 
    [BankName] NVARCHAR(64) NULL, 
    [SubBankName] NVARCHAR(64) NULL, 
    [CheckingAccount] NVARCHAR(64) NULL, 
    [RemittanceAccount] NVARCHAR(64) NULL, 
    [PayDays] DATETIME NULL, 
    [CreditLine] DECIMAL(12, 2) NULL, 
    [CreditBalance] DECIMAL(12, 2) NULL, 
    [LastTransDate] DATETIME NULL, 
    [TemporaryAmount] DECIMAL(12, 2) NULL,
    CONSTRAINT [PK_4060Supplier] PRIMARY KEY CLUSTERED ([Id] ASC),
)
GO
CREATE NONCLUSTERED INDEX IX_t_4060Supplier_No
ON dbo.t_4060Supplier (No);
