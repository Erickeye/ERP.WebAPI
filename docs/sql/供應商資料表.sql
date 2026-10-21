-- 供應商資料表（t_4060Supplier）：補啟用旗標、名稱必填、付款天數改整數，並在資料無衝突時建立唯一索引。
-- 既有資料庫用：可重複執行，欄位、型別與索引已調整過時不會重複處理。
-- 新環境由 ERP.DB 專案發佈即可建立最終結構，不需要執行此腳本。
SET NOCOUNT ON;
GO

-- 1. 啟用旗標：既有的供應商一律視為啟用中
IF COL_LENGTH(N'dbo.t_4060Supplier', N'IsActive') IS NULL
    ALTER TABLE [dbo].[t_4060Supplier]
        ADD [IsActive] BIT NOT NULL CONSTRAINT [DF_t_4060Supplier_IsActive] DEFAULT 1;
GO

-- 2. 名稱改為必填：空名稱先補成佔位名稱，之後請到供應商管理頁更正
IF EXISTS (SELECT 1 FROM sys.columns
           WHERE object_id = OBJECT_ID(N'dbo.t_4060Supplier') AND name = N'Name' AND is_nullable = 1)
BEGIN
    UPDATE [dbo].[t_4060Supplier]
    SET [Name] = N'（未命名供應商 ' + CAST([Id] AS NVARCHAR(20)) + N'）'
    WHERE [Name] IS NULL OR LTRIM(RTRIM([Name])) = N'';

    ALTER TABLE [dbo].[t_4060Supplier] ALTER COLUMN [Name] NVARCHAR(128) NOT NULL;
END
GO

-- 3. 付款天數由日期型別改為整數天數：日期內容無法可靠轉成天數，先清空再改型別
IF EXISTS (SELECT 1 FROM sys.columns c
           JOIN sys.types t ON t.user_type_id = c.user_type_id
           WHERE c.object_id = OBJECT_ID(N'dbo.t_4060Supplier') AND c.name = N'PayDays' AND t.name IN (N'datetime', N'datetime2', N'date'))
BEGIN
    UPDATE [dbo].[t_4060Supplier] SET [PayDays] = NULL;
    ALTER TABLE [dbo].[t_4060Supplier] ALTER COLUMN [PayDays] INT NULL;
END
GO

-- 4. 列出會讓唯一索引失敗的資料。腳本不會自動修改這些資料，請到供應商管理頁人工處理後再重新執行本腳本。
--    供應商編號（有值時不可重複；空白字串也算有值）
SELECT N'供應商編號重複' AS [問題], [No] AS [值], COUNT(*) AS [筆數], MIN([Id]) AS [最小Id]
FROM [dbo].[t_4060Supplier]
WHERE [No] IS NOT NULL
GROUP BY [No]
HAVING COUNT(*) > 1;

--    統一編號（有值時不可重複）
SELECT N'統一編號重複' AS [問題], [TaxIdNumber] AS [值], COUNT(*) AS [筆數], MIN([Id]) AS [最小Id]
FROM [dbo].[t_4060Supplier]
WHERE [TaxIdNumber] IS NOT NULL
GROUP BY [TaxIdNumber]
HAVING COUNT(*) > 1;

--    供應商編號為空的資料只是提醒：新增與編輯時編號必填，空編號的舊資料編輯前需先補上
SELECT N'供應商編號為空（提醒）' AS [問題], COUNT(*) AS [筆數]
FROM [dbo].[t_4060Supplier]
WHERE [No] IS NULL OR LTRIM(RTRIM([No])) = N'';
GO

-- 5. 唯一索引：只在沒有重複資料時才建立；原本的供應商編號索引不是唯一的，無衝突時才換掉
IF NOT EXISTS (SELECT 1 FROM [dbo].[t_4060Supplier] WHERE [No] IS NOT NULL GROUP BY [No] HAVING COUNT(*) > 1)
   AND NOT EXISTS (SELECT 1 FROM sys.indexes
                   WHERE name = N'IX_t_4060Supplier_No' AND object_id = OBJECT_ID(N'dbo.t_4060Supplier') AND is_unique = 1)
BEGIN
    IF EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_t_4060Supplier_No' AND object_id = OBJECT_ID(N'dbo.t_4060Supplier'))
        DROP INDEX [IX_t_4060Supplier_No] ON [dbo].[t_4060Supplier];

    CREATE UNIQUE NONCLUSTERED INDEX [IX_t_4060Supplier_No]
    ON [dbo].[t_4060Supplier] ([No])
    WHERE [No] IS NOT NULL;
END
ELSE IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_t_4060Supplier_No' AND object_id = OBJECT_ID(N'dbo.t_4060Supplier'))
    PRINT N'供應商編號有重複，尚未建立唯一索引 IX_t_4060Supplier_No，請先處理上面列出的資料。';
GO

IF NOT EXISTS (SELECT 1 FROM [dbo].[t_4060Supplier] WHERE [TaxIdNumber] IS NOT NULL GROUP BY [TaxIdNumber] HAVING COUNT(*) > 1)
   AND NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_t_4060Supplier_TaxIdNumber' AND object_id = OBJECT_ID(N'dbo.t_4060Supplier'))
    CREATE UNIQUE NONCLUSTERED INDEX [IX_t_4060Supplier_TaxIdNumber]
    ON [dbo].[t_4060Supplier] ([TaxIdNumber])
    WHERE [TaxIdNumber] IS NOT NULL;
ELSE IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_t_4060Supplier_TaxIdNumber' AND object_id = OBJECT_ID(N'dbo.t_4060Supplier'))
    PRINT N'統一編號有重複，尚未建立唯一索引 IX_t_4060Supplier_TaxIdNumber，請先處理上面列出的資料。';
GO

-- 功能權限「供應商檢視」（PermissionType = 4060）、「供應商編輯」（40600，含停用與啟用）、「供應商刪除」（40601）：
-- 在角色權限設定勾選即可，不需要此腳本寫入 Permission。
