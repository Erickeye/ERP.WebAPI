CREATE TABLE [dbo].[t_4060Supplier]
(
	[Id] INT IDENTITY (1, 1) NOT NULL,
    [No] NVARCHAR(64) NULL, 
    [Name] NVARCHAR(128) NOT NULL, 
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
    [PayDays] INT NULL, -- 付款天數
    [CreditLine] DECIMAL(12, 2) NULL, 
    [CreditBalance] DECIMAL(12, 2) NULL, 
    [LastTransDate] DATETIME NULL, 
    [TemporaryAmount] DECIMAL(12, 2) NULL,
    [IsActive] BIT NOT NULL CONSTRAINT [DF_t_4060Supplier_IsActive] DEFAULT 1, -- 啟用中；停用後不再出現在新進貨單的選項
    CONSTRAINT [PK_4060Supplier] PRIMARY KEY CLUSTERED ([Id] ASC),
)
GO
-- 供應商編號有值時不可重複（既有資料可能為空，所以只限制有值的列）
CREATE UNIQUE NONCLUSTERED INDEX IX_t_4060Supplier_No
ON dbo.t_4060Supplier (No)
WHERE No IS NOT NULL;
GO
-- 統一編號有值時不可重複
CREATE UNIQUE NONCLUSTERED INDEX IX_t_4060Supplier_TaxIdNumber
ON dbo.t_4060Supplier (TaxIdNumber)
WHERE TaxIdNumber IS NOT NULL;
