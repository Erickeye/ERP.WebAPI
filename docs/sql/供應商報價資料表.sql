-- 供應商報價資料表（t_4061SupplierPrice）：建立資料表與（供應商＋料號）唯一索引。
-- 既有資料庫用：可重複執行，資料表與索引已存在時不會重複建立。這是全新資料表，沒有既有資料需要遷移。
-- 前置條件：t_4060Supplier 已存在（供應商管理的部署腳本已執行）。
-- 新環境由 ERP.DB 專案發佈即可建立資料表與索引，不一定需要此腳本。
SET NOCOUNT ON;
SET QUOTED_IDENTIFIER ON;
GO

IF OBJECT_ID(N'dbo.t_4061SupplierPrice', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[t_4061SupplierPrice] (
        [Id]           INT            IDENTITY (1, 1) NOT NULL,
        [SupplierId]   INT            NOT NULL,
        [No]           NVARCHAR (64)  NOT NULL, -- 料號（與進貨單明細的料號文字比對）
        [Name]         NVARCHAR (128) NOT NULL,
        [Unit]         NVARCHAR (16)  NOT NULL,
        [Price]        DECIMAL (12, 2) NOT NULL, -- 新台幣未稅單價；只存現價
        [Note]         NVARCHAR (256) NULL,
        [CreateTime]   DATETIME       NOT NULL,
        [CreateUserId] INT            NOT NULL,
        [UpdateTime]   DATETIME       NOT NULL,
        [UpdateUserId] INT            NOT NULL,
        CONSTRAINT [PK_4061SupplierPrice] PRIMARY KEY CLUSTERED ([Id] ASC),
        -- 刪除供應商時連同其報價一起刪除
        CONSTRAINT [FK_4061SupplierPrice_SupplierId] FOREIGN KEY ([SupplierId])
            REFERENCES [dbo].[t_4060Supplier] ([Id]) ON DELETE CASCADE
    );
END
GO

-- 同一供應商的同一料號只有一筆報價
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_t_4061SupplierPrice_SupplierId_No' AND object_id = OBJECT_ID(N'dbo.t_4061SupplierPrice'))
    CREATE UNIQUE NONCLUSTERED INDEX [IX_t_4061SupplierPrice_SupplierId_No]
    ON [dbo].[t_4061SupplierPrice] ([SupplierId], [No]);
GO

-- 權限：沿用「供應商檢視」（4060，管列表與詳情）、「供應商編輯」（40600，管新增與編輯）、「供應商刪除」（40601，管刪除），
-- 不需要此腳本寫入 Permission，也沒有新增權限碼。
