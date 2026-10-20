CREATE TABLE [dbo].[t_4040StockMovement]
(
    [Id] INT IDENTITY (1, 1) NOT NULL,
    [InventoryId] INT NOT NULL,
    -- 異動類型：1 進貨入庫、2 進貨作廢、3 領料出庫、4 領料作廢
    [MovementType] INT NOT NULL,
    -- 來源單據類型（TableType）與單號
    [SourceTableType] INT NOT NULL,
    [SourceNo] NVARCHAR(64) NOT NULL,
    -- 異動數量：增加為正、減少為負
    [Quantity] DECIMAL(12, 2) NOT NULL,
    [BeforeQuantity] DECIMAL(12, 2) NOT NULL,
    [AfterQuantity] DECIMAL(12, 2) NOT NULL,
    [OperatorUserId] INT NOT NULL,
    [MovementTime] DATETIME NOT NULL,
    CONSTRAINT [PK_t_4040StockMovement] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_t_4040StockMovement_Inventory] FOREIGN KEY ([InventoryId])    REFERENCES [dbo].[t_4000Inventory] ([Id]),
    CONSTRAINT [FK_t_4040StockMovement_Operator]  FOREIGN KEY ([OperatorUserId]) REFERENCES [dbo].[User] ([Id])
)
GO
CREATE NONCLUSTERED INDEX [IX_t_4040StockMovement_Inventory_Time]
ON [dbo].[t_4040StockMovement] ([InventoryId], [MovementTime]);
GO
CREATE NONCLUSTERED INDEX [IX_t_4040StockMovement_MovementType]
ON [dbo].[t_4040StockMovement] ([MovementType]);
GO
CREATE NONCLUSTERED INDEX [IX_t_4040StockMovement_Source]
ON [dbo].[t_4040StockMovement] ([SourceTableType], [SourceNo]);
GO
