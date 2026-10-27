-- 合約單資料表（t_3050Contract）：建立資料表與索引。
-- 既有資料庫用：可重複執行，資料表與索引已存在時不會重複建立。這是全新資料表，沒有既有資料需要遷移。
-- 前置條件：t_4060Supplier、t_2000Customer、t_1080Company、User 已存在（供應商與公司管理的部署腳本已執行）。
-- 新環境由 ERP.DB 專案發佈即可建立資料表與索引，不一定需要此腳本。
SET NOCOUNT ON;
SET QUOTED_IDENTIFIER ON;
GO

IF OBJECT_ID(N'dbo.t_3050Contract', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[t_3050Contract]
    (
        [Id] INT IDENTITY (1, 1) NOT NULL,
        [No] VARCHAR(32) NOT NULL,                      -- 單號（系統產生）
        [ContractNo] NVARCHAR(64) NOT NULL,             -- 合約編號（紙本合約編號；非作廢單之間不可重複，由服務層檢查）
        [Type] INT NOT NULL,                            -- 0供應商合約;1客戶合約
        [SupplierId] INT NULL,
        [CustomerId] INT NULL,
        [CompanyId] INT NOT NULL,                       -- 簽約公司
        [StartDate] DATE NOT NULL,
        [EndDate] DATE NOT NULL,
        [Amount] DECIMAL(12, 2) NOT NULL,               -- 合約總金額（新台幣）
        [Remark] NVARCHAR(1024) NULL,
        [AttachmentPath] NVARCHAR(256) NULL,            -- 合約掃描檔的 SFTP 遠端路徑
        [Status] INT NOT NULL CONSTRAINT [DF_t_3050Contract_Status] DEFAULT 0, -- 0草稿;1簽核中;2已核准;3已退回;4已作廢
        [CreateTime] DATETIME NOT NULL,
        [CreateUserId] INT NOT NULL,
        [UpdateTime] DATETIME NOT NULL,
        [UpdateUserId] INT NOT NULL,
        CONSTRAINT [PK_t_3050Contract] PRIMARY KEY CLUSTERED ([Id] DESC),
        CONSTRAINT [CK_t_3050Contract_Dates] CHECK ([EndDate] >= [StartDate]),
        CONSTRAINT [CK_t_3050Contract_Amount] CHECK ([Amount] > 0),
        CONSTRAINT [CK_t_3050Contract_Party] CHECK (
            ([Type] = 0 AND [SupplierId] IS NOT NULL AND [CustomerId] IS NULL) OR
            ([Type] = 1 AND [CustomerId] IS NOT NULL AND [SupplierId] IS NULL)),
        -- 供應商、客戶、公司外鍵都限制刪除：被合約單引用就不能刪
        CONSTRAINT [FK_t_3050Contract_Supplier]   FOREIGN KEY ([SupplierId])   REFERENCES [dbo].[t_4060Supplier] ([Id]),
        CONSTRAINT [FK_t_3050Contract_Customer]   FOREIGN KEY ([CustomerId])   REFERENCES [dbo].[t_2000Customer] ([Id]),
        CONSTRAINT [FK_t_3050Contract_Company]    FOREIGN KEY ([CompanyId])    REFERENCES [dbo].[t_1080Company] ([Id]),
        CONSTRAINT [FK_t_3050Contract_CreateUser] FOREIGN KEY ([CreateUserId]) REFERENCES [dbo].[User] ([Id])
    );
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_t_3050Contract_No' AND object_id = OBJECT_ID(N'dbo.t_3050Contract'))
    CREATE UNIQUE NONCLUSTERED INDEX [IX_t_3050Contract_No]
    ON [dbo].[t_3050Contract] ([No]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_t_3050Contract_ContractNo' AND object_id = OBJECT_ID(N'dbo.t_3050Contract'))
    CREATE NONCLUSTERED INDEX [IX_t_3050Contract_ContractNo]
    ON [dbo].[t_3050Contract] ([ContractNo]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_t_3050Contract_StartDate' AND object_id = OBJECT_ID(N'dbo.t_3050Contract'))
    CREATE NONCLUSTERED INDEX [IX_t_3050Contract_StartDate]
    ON [dbo].[t_3050Contract] ([StartDate], [EndDate]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_t_3050Contract_Status' AND object_id = OBJECT_ID(N'dbo.t_3050Contract'))
    CREATE NONCLUSTERED INDEX [IX_t_3050Contract_Status]
    ON [dbo].[t_3050Contract] ([Status]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_t_3050Contract_SupplierId' AND object_id = OBJECT_ID(N'dbo.t_3050Contract'))
    CREATE NONCLUSTERED INDEX [IX_t_3050Contract_SupplierId]
    ON [dbo].[t_3050Contract] ([SupplierId]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_t_3050Contract_CustomerId' AND object_id = OBJECT_ID(N'dbo.t_3050Contract'))
    CREATE NONCLUSTERED INDEX [IX_t_3050Contract_CustomerId]
    ON [dbo].[t_3050Contract] ([CustomerId]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_t_3050Contract_CompanyId' AND object_id = OBJECT_ID(N'dbo.t_3050Contract'))
    CREATE NONCLUSTERED INDEX [IX_t_3050Contract_CompanyId]
    ON [dbo].[t_3050Contract] ([CompanyId]);
GO

-- 權限：沿用「合約單檢視」（3050）、「合約單編輯」（30500）、「合約單刪除」（30501），
-- 不需要此腳本寫入 Permission，也沒有新增權限碼。
-- 簽核設定：上線後需由管理者以現有簽核設定 API 建立一組啟用的「合約單」（TableType = 11）簽核設定，見 ticket 03、07。
