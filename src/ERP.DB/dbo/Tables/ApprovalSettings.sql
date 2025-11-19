CREATE TABLE [dbo].[ApprovalSettings] (
    [Id]        INT           IDENTITY (1, 1) NOT NULL,
    [TableType] INT           NOT NULL,
    [Name]      NVARCHAR (64) NOT NULL,
    [IsActive]  BIT           NOT NULL,
    CONSTRAINT [PK_ApprovalSettings] PRIMARY KEY CLUSTERED ([Id] ASC)
);

