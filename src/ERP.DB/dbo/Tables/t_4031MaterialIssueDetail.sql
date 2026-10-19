CREATE TABLE [dbo].[t_4031MaterialIssueDetail]
(
    [Id] INT IDENTITY (1, 1) NOT NULL,
    [MaterialIssueId] INT NOT NULL,
    [InventoryId] INT NOT NULL,
    [Quantity] DECIMAL(12, 2) NOT NULL,
    -- 料號、品名、單位快照（單據歷史不隨庫存資料變動）
    [No] NVARCHAR(64) NULL,
    [Name] NVARCHAR(128) NOT NULL,
    [Unit] NVARCHAR(16) NULL,
    CONSTRAINT [PK_t_4031MaterialIssueDetail] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_t_4031MaterialIssueDetail_MaterialIssue] FOREIGN KEY ([MaterialIssueId]) REFERENCES [dbo].[t_4030MaterialIssue] ([Id]),
    CONSTRAINT [FK_t_4031MaterialIssueDetail_Inventory]     FOREIGN KEY ([InventoryId])     REFERENCES [dbo].[t_4000Inventory] ([Id])
)
GO
-- 同一張領料單內，同一庫存項目只能出現一次
CREATE UNIQUE NONCLUSTERED INDEX [IX_t_4031MaterialIssueDetail_Issue_Inventory]
ON [dbo].[t_4031MaterialIssueDetail] ([MaterialIssueId], [InventoryId]);
GO
CREATE NONCLUSTERED INDEX [IX_t_4031MaterialIssueDetail_InventoryId]
ON [dbo].[t_4031MaterialIssueDetail] ([InventoryId]);
GO
