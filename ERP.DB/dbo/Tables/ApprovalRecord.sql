CREATE TABLE [dbo].[ApprovalRecord] (
    [Id]             INT            IDENTITY (1, 1) NOT NULL,
    [ApprovalStepId] INT            NOT NULL,
    [RoleId]         INT            NOT NULL,
    [TableId]        NVARCHAR (32)  NULL,
    [StepOrder]      INT            NOT NULL,
    [UserId]         INT            NULL,
    [Status]         INT            NOT NULL,
    [Date]           DATETIME2 (7)  NULL,
    [Memo]           NVARCHAR (256) NULL,
    [TableType]      INT            DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_ApprovalRecord] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_ApprovalRecord_ApprovalStep_ApprovalStepId] FOREIGN KEY ([ApprovalStepId]) REFERENCES [dbo].[ApprovalStep] ([Id]) ON DELETE CASCADE
);


GO
CREATE NONCLUSTERED INDEX [IX_ApprovalRecord_ApprovalStepId]
    ON [dbo].[ApprovalRecord]([ApprovalStepId] ASC);

