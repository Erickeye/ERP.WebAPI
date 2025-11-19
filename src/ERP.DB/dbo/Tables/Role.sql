CREATE TABLE [dbo].[Role] (
    [Id]               INT           IDENTITY (1, 1) NOT NULL,
    [RoleName]         NVARCHAR (32) NOT NULL,
    [PermissionLevel]  INT           NULL,
    [ApprovalLevel]    INT           NULL,
    [QuotationLevel]   INT           NULL,
    [ProcurementLevel] INT           NULL,
    CONSTRAINT [PK_Role] PRIMARY KEY CLUSTERED ([Id] ASC)
);

