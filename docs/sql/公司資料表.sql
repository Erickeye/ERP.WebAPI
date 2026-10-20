-- 公司資料表（t_1080Company）：補啟用旗標與唯一索引，並在沒有任何公司時新增一間占位的預設公司。
-- 既有資料庫用：可重複執行，欄位、索引與預設公司已存在時不會重複建立。
-- 新環境由 ERP.DB 專案發佈即可建立資料表與索引，但「預設公司」仍需執行此腳本最後一段（或自行在公司管理頁新增公司）。
SET NOCOUNT ON;

IF OBJECT_ID(N'dbo.t_1080Company', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[t_1080Company] (
        [Id]                INT           IDENTITY (1, 1) NOT NULL,
        [AttribName]        NVARCHAR (32) NOT NULL,
        [Name]              NVARCHAR (32) NOT NULL,
        [TaxInvoiceNumber]  NVARCHAR (8)  NOT NULL,
        [TaxSerialNumber]   NVARCHAR (9)  NOT NULL,
        [Owner]             NVARCHAR (32) NOT NULL,
        [ContactPhone]      NVARCHAR (32) NULL,
        [FaxPhone]          NVARCHAR (32) NULL,
        [RegisteredAddress] NVARCHAR (64) NOT NULL,
        [DeliveryAddress]   NVARCHAR (64) NULL,
        [TaxInvoiceAddress] NVARCHAR (64) NULL,
        [BankName]          NVARCHAR (32) NULL,
        [CheckingAccount]   NVARCHAR (32) NULL,
        [RemittanceAccount] NVARCHAR (32) NULL,
        [PayDays]           NVARCHAR (8)  NULL,
        [FoundedDate]       DATETIME2 (7) NULL,
        [InvoiceForm]       NVARCHAR (8)  NULL,
        CONSTRAINT [PK_t_1080Company] PRIMARY KEY CLUSTERED ([Id] ASC)
    );
END
GO

-- 啟用旗標：既有的公司（若有）一律視為啟用中
IF COL_LENGTH(N'dbo.t_1080Company', N'IsActive') IS NULL
    ALTER TABLE [dbo].[t_1080Company]
        ADD [IsActive] BIT NOT NULL CONSTRAINT [DF_t_1080Company_IsActive] DEFAULT 1;
GO

-- 公司簡稱與統一編號在所有公司之間不可重複（停用的公司也算）。
-- 若既有資料有重複，建立索引會失敗，請先到公司管理頁或直接修正重複的資料後再執行。
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_t_1080Company_AttribName' AND object_id = OBJECT_ID(N'dbo.t_1080Company'))
    CREATE UNIQUE NONCLUSTERED INDEX [IX_t_1080Company_AttribName]
    ON [dbo].[t_1080Company] ([AttribName]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_t_1080Company_TaxInvoiceNumber' AND object_id = OBJECT_ID(N'dbo.t_1080Company'))
    CREATE UNIQUE NONCLUSTERED INDEX [IX_t_1080Company_TaxInvoiceNumber]
    ON [dbo].[t_1080Company] ([TaxInvoiceNumber]);
GO

-- 預設公司：資料表沒有任何公司時才新增。
-- 注意：以下統一編號、稅籍編號、負責人、登記地址都是【占位值，待更正】，
-- 上線後請立刻到「公司管理」頁改成真實資料；之後領料單與出差單（ticket 02、03）的既有單據會回填到這間公司。
IF NOT EXISTS (SELECT 1 FROM [dbo].[t_1080Company])
    INSERT INTO [dbo].[t_1080Company]
        ([AttribName], [Name], [TaxInvoiceNumber], [TaxSerialNumber], [Owner], [RegisteredAddress], [IsActive])
    VALUES
        (N'預設公司', N'預設公司（待更正）', N'00000000', N'000000000', N'待更正', N'待更正', 1);
GO

-- 功能權限「公司檢視」（PermissionType = 1080）、「公司編輯」（10800，含停用與啟用）、「公司刪除」（10801）：
-- 在角色權限設定（既有角色權限 API／畫面）勾選即可，不需要此腳本寫入 Permission。
-- 填單用的公司選單不需要上述權限：有「領料單編輯」或「出差單」權限者即可取得。
