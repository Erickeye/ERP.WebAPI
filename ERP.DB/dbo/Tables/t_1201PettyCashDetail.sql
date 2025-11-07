CREATE TABLE [dbo].[t_1201PettyCashDetail] (
    [Id]            INT             IDENTITY (1, 1) NOT NULL,
    [Name]          NVARCHAR (MAX)  NOT NULL,
    [Project]       NVARCHAR (MAX)  NULL,
    [InvoiceNumber] NVARCHAR (MAX)  NULL,
    [PurchaseId]    NVARCHAR (MAX)  NULL,
    [Vehicle]       NVARCHAR (MAX)  NULL,
    [Supplier]      NVARCHAR (MAX)  NULL,
    [Content]       NVARCHAR (MAX)  NULL,
    [InvoiceDate]   DATETIME2 (7)   NULL,
    [Amount]        DECIMAL (12, 2) NULL,
    [Tax]           DECIMAL (12, 2) NULL,
    [Total]         DECIMAL (12, 2) NULL,
    [Sort]          INT             NULL,
    [PettyCashId]   NVARCHAR (32)   DEFAULT (N'') NOT NULL,
    CONSTRAINT [PK_t_1201PettyCashDetail] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_t_1201PettyCashDetail_t_1200PettyCash_PettyCashId] FOREIGN KEY ([PettyCashId]) REFERENCES [dbo].[t_1200PettyCash] ([Id]) ON DELETE CASCADE
);


GO
CREATE NONCLUSTERED INDEX [IX_t_1201PettyCashDetail_PettyCashId]
    ON [dbo].[t_1201PettyCashDetail]([PettyCashId] ASC);

