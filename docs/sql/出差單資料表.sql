-- 出差單資料表（t_1060BusinessTrip）
-- 既有資料庫用：可重複執行，資料表與索引已存在時不會重複建立。
-- 新環境由 ERP.DB 專案發佈即可，不需要執行此腳本。
SET NOCOUNT ON;

IF OBJECT_ID(N'dbo.t_1060BusinessTrip', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[t_1060BusinessTrip]
    (
        [Id] INT IDENTITY (1, 1) NOT NULL,
        [No] VARCHAR(32) NOT NULL,
        [TravelerStaffId] INT NOT NULL,
        [CreateUserId] INT NOT NULL,
        [Destination] NVARCHAR(128) NOT NULL,
        [Reason] NVARCHAR(256) NOT NULL,
        [StartDate] DATE NOT NULL,
        [EndDate] DATE NOT NULL,
        [CustomerId] INT NULL,
        [Note] NVARCHAR(1024) NULL,
        [Status] INT NOT NULL CONSTRAINT [DF_t_1060BusinessTrip_Status] DEFAULT 0,
        [CreateTime] DATETIME NOT NULL,
        CONSTRAINT [PK_t_1060BusinessTrip] PRIMARY KEY CLUSTERED ([Id] DESC),
        CONSTRAINT [CK_t_1060BusinessTrip_Dates] CHECK ([EndDate] >= [StartDate]),
        CONSTRAINT [FK_t_1060BusinessTrip_Traveler]   FOREIGN KEY ([TravelerStaffId]) REFERENCES [dbo].[t_1000Staff] ([StaffId]),
        CONSTRAINT [FK_t_1060BusinessTrip_CreateUser] FOREIGN KEY ([CreateUserId])    REFERENCES [dbo].[User] ([Id]),
        CONSTRAINT [FK_t_1060BusinessTrip_Customer]   FOREIGN KEY ([CustomerId])      REFERENCES [dbo].[t_2000Customer] ([Id])
    );
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_t_1060BusinessTrip_No' AND object_id = OBJECT_ID(N'dbo.t_1060BusinessTrip'))
    CREATE UNIQUE NONCLUSTERED INDEX [IX_t_1060BusinessTrip_No]
    ON [dbo].[t_1060BusinessTrip] ([No]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_t_1060BusinessTrip_TravelerStaffId' AND object_id = OBJECT_ID(N'dbo.t_1060BusinessTrip'))
    CREATE NONCLUSTERED INDEX [IX_t_1060BusinessTrip_TravelerStaffId]
    ON [dbo].[t_1060BusinessTrip] ([TravelerStaffId], [StartDate], [EndDate]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_t_1060BusinessTrip_CreateUserId' AND object_id = OBJECT_ID(N'dbo.t_1060BusinessTrip'))
    CREATE NONCLUSTERED INDEX [IX_t_1060BusinessTrip_CreateUserId]
    ON [dbo].[t_1060BusinessTrip] ([CreateUserId]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_t_1060BusinessTrip_StartDate' AND object_id = OBJECT_ID(N'dbo.t_1060BusinessTrip'))
    CREATE NONCLUSTERED INDEX [IX_t_1060BusinessTrip_StartDate]
    ON [dbo].[t_1060BusinessTrip] ([StartDate]);
GO
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_t_1060BusinessTrip_Status' AND object_id = OBJECT_ID(N'dbo.t_1060BusinessTrip'))
    CREATE NONCLUSTERED INDEX [IX_t_1060BusinessTrip_Status]
    ON [dbo].[t_1060BusinessTrip] ([Status]);
GO

-- 功能權限「出差單」（PermissionType = 1060）：
-- 在角色權限設定（既有角色權限 API／畫面）勾選「出差單」即可，不需要此腳本寫入 Permission。
