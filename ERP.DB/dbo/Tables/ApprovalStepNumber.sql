CREATE TABLE [dbo].[ApprovalStepNumber] (
    [Id]             INT IDENTITY (1, 1) NOT NULL,
    [ApprovalStepId] INT NOT NULL,
    [UserId]         INT NOT NULL,
    CONSTRAINT [PK_ApprovalStepNumber] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_ApprovalStepNumber_ApprovalStep_ApprovalStepId] FOREIGN KEY ([ApprovalStepId]) REFERENCES [dbo].[ApprovalStep] ([Id]) ON DELETE CASCADE
);


GO
CREATE NONCLUSTERED INDEX [IX_ApprovalStepNumber_ApprovalStepId]
    ON [dbo].[ApprovalStepNumber]([ApprovalStepId] ASC);

