-- 出差單加「單據公司」：t_1060BusinessTrip 新增 CompanyId（依 ADR-0001 只存公司 Id，不存名稱快照）。
-- 既有資料庫用：可重複執行，欄位、索引與外鍵已存在時不會重複建立。
-- 前置：請先執行「公司資料表.sql」（需要 t_1080Company 與預設公司）。
-- 順序：加欄位（允許空值）→ 用「預設公司」回填既有出差單 → 改成必填 → 加索引與外鍵。
-- 新環境由 ERP.DB 專案發佈即可，資料表已含 CompanyId；沒有既有單據，不需要回填。
SET NOCOUNT ON;

IF OBJECT_ID(N'dbo.t_1080Company', N'U') IS NULL
BEGIN
    RAISERROR(N'找不到 t_1080Company，請先執行「公司資料表.sql」。', 16, 1);
    RETURN;
END
GO

-- 1. 加欄位（先允許空值，才能回填）
IF COL_LENGTH(N'dbo.t_1060BusinessTrip', N'CompanyId') IS NULL
    ALTER TABLE [dbo].[t_1060BusinessTrip] ADD [CompanyId] INT NULL;
GO

-- 2. 回填：把沒有公司的出差單歸到預設公司（簡稱「預設公司」）。
--    找不到「預設公司」時，退而使用 Id 最小的公司；完全沒有公司則停下，不改欄位為必填。
DECLARE @DefaultCompanyId INT =
    (SELECT TOP (1) [Id] FROM [dbo].[t_1080Company] WHERE [AttribName] = N'預設公司' ORDER BY [Id]);
IF @DefaultCompanyId IS NULL
    SET @DefaultCompanyId = (SELECT TOP (1) [Id] FROM [dbo].[t_1080Company] ORDER BY [Id]);

IF @DefaultCompanyId IS NULL
BEGIN
    RAISERROR(N't_1080Company 沒有任何公司，無法回填出差單；請先執行「公司資料表.sql」新增預設公司。', 16, 1);
END
ELSE
BEGIN
    EXEC sp_executesql
        N'UPDATE [dbo].[t_1060BusinessTrip] SET [CompanyId] = @id WHERE [CompanyId] IS NULL;',
        N'@id INT', @id = @DefaultCompanyId;
END
GO

-- 3. 改成必填（仍有空值時略過，避免失敗；回填成功後重跑即可）
IF NOT EXISTS (SELECT 1 FROM [dbo].[t_1060BusinessTrip] WHERE [CompanyId] IS NULL)
   AND EXISTS (SELECT 1 FROM sys.columns
               WHERE object_id = OBJECT_ID(N'dbo.t_1060BusinessTrip') AND name = N'CompanyId' AND is_nullable = 1)
    ALTER TABLE [dbo].[t_1060BusinessTrip] ALTER COLUMN [CompanyId] INT NOT NULL;
GO

-- 4. 索引與外鍵
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_t_1060BusinessTrip_CompanyId' AND object_id = OBJECT_ID(N'dbo.t_1060BusinessTrip'))
    CREATE NONCLUSTERED INDEX [IX_t_1060BusinessTrip_CompanyId]
    ON [dbo].[t_1060BusinessTrip] ([CompanyId]);
GO

IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_t_1060BusinessTrip_Company')
    ALTER TABLE [dbo].[t_1060BusinessTrip]
        ADD CONSTRAINT [FK_t_1060BusinessTrip_Company]
        FOREIGN KEY ([CompanyId]) REFERENCES [dbo].[t_1080Company] ([Id]);
GO
