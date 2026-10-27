CREATE TABLE [dbo].[t_3050Contract]
(
    [Id] INT IDENTITY (1, 1) NOT NULL,
    [No] VARCHAR(32) NOT NULL,
    [ContractNo] NVARCHAR(64) NOT NULL,
    [Type] INT NOT NULL,
    [SupplierId] INT NULL,
    [CustomerId] INT NULL,
    [CompanyId] INT NOT NULL,
    [StartDate] DATE NOT NULL,
    [EndDate] DATE NOT NULL,
    [Amount] DECIMAL(12, 2) NOT NULL,
    [Remark] NVARCHAR(1024) NULL,
    [AttachmentPath] NVARCHAR(256) NULL,
    [Status] INT NOT NULL DEFAULT 0,
    [CreateTime] DATETIME NOT NULL,
    [CreateUserId] INT NOT NULL,
    [UpdateTime] DATETIME NOT NULL,
    [UpdateUserId] INT NOT NULL,
    -- Primary Key
    CONSTRAINT [PK_t_3050Contract] PRIMARY KEY CLUSTERED ([Id] DESC),
    -- Check：迄日不得早於起日；合約總金額大於 0；類型為供應商合約(0)時只有供應商，客戶合約(1)時只有客戶
    CONSTRAINT [CK_t_3050Contract_Dates] CHECK ([EndDate] >= [StartDate]),
    CONSTRAINT [CK_t_3050Contract_Amount] CHECK ([Amount] > 0),
    CONSTRAINT [CK_t_3050Contract_Party] CHECK (
        ([Type] = 0 AND [SupplierId] IS NOT NULL AND [CustomerId] IS NULL) OR
        ([Type] = 1 AND [CustomerId] IS NOT NULL AND [SupplierId] IS NULL)),
    -- Foreign Keys（一律限制刪除，被合約單引用的供應商、客戶、公司不能刪）
    CONSTRAINT [FK_t_3050Contract_Supplier]   FOREIGN KEY ([SupplierId])   REFERENCES [dbo].[t_4060Supplier] ([Id]),
    CONSTRAINT [FK_t_3050Contract_Customer]   FOREIGN KEY ([CustomerId])   REFERENCES [dbo].[t_2000Customer] ([Id]),
    CONSTRAINT [FK_t_3050Contract_Company]    FOREIGN KEY ([CompanyId])    REFERENCES [dbo].[t_1080Company] ([Id]),
    CONSTRAINT [FK_t_3050Contract_CreateUser] FOREIGN KEY ([CreateUserId]) REFERENCES [dbo].[User] ([Id])
)
GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_t_3050Contract_No]
ON [dbo].[t_3050Contract] ([No]);
GO
CREATE NONCLUSTERED INDEX [IX_t_3050Contract_ContractNo]
ON [dbo].[t_3050Contract] ([ContractNo]);
GO
CREATE NONCLUSTERED INDEX [IX_t_3050Contract_StartDate]
ON [dbo].[t_3050Contract] ([StartDate], [EndDate]);
GO
CREATE NONCLUSTERED INDEX [IX_t_3050Contract_Status]
ON [dbo].[t_3050Contract] ([Status]);
GO
CREATE NONCLUSTERED INDEX [IX_t_3050Contract_SupplierId]
ON [dbo].[t_3050Contract] ([SupplierId]);
GO
CREATE NONCLUSTERED INDEX [IX_t_3050Contract_CustomerId]
ON [dbo].[t_3050Contract] ([CustomerId]);
GO
CREATE NONCLUSTERED INDEX [IX_t_3050Contract_CompanyId]
ON [dbo].[t_3050Contract] ([CompanyId]);
GO
