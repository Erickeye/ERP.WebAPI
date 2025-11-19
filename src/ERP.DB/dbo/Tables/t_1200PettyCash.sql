CREATE TABLE [dbo].[t_1200PettyCash] (
    [RequestDate]  DATETIME2 (7)   NOT NULL,
    [TotalAmount]  DECIMAL (12, 2) NULL,
    [PaymentDate]  DATETIME2 (7)   NULL,
    [Approval]     BIT             NULL,
    [Accounting]   BIT             NULL,
    [Id]           NVARCHAR (32)   DEFAULT (N'') NOT NULL,
    [Authorizator] NVARCHAR (32)   NULL,
    [Company]      NVARCHAR (64)   NULL,
    [Filler]       NVARCHAR (32)   DEFAULT (N'') NOT NULL,
    [Payee]        NVARCHAR (32)   DEFAULT (N'') NOT NULL,
    [Reason]       NVARCHAR (64)   NULL,
    [Supervisor]   NVARCHAR (32)   NULL,
    CONSTRAINT [PK_t_1200PettyCash] PRIMARY KEY CLUSTERED ([Id] ASC)
);

