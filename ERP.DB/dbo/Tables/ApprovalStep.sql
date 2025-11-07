CREATE TABLE [dbo].[ApprovalStep] (
    [Id]                 INT IDENTITY (1, 1) NOT NULL,
    [ApprovalSettingsId] INT NOT NULL,
    [StepOrder]          INT NOT NULL,
    [RoleId]             INT NOT NULL,
    [Mode]               INT NOT NULL,
    [RequiredCounts]     INT NULL,
    CONSTRAINT [PK_ApprovalStep] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_ApprovalStep_ApprovalSettings_ApprovalSettingsId] FOREIGN KEY ([ApprovalSettingsId]) REFERENCES [dbo].[ApprovalSettings] ([Id]) ON DELETE CASCADE
);


GO
CREATE NONCLUSTERED INDEX [IX_ApprovalStep_ApprovalSettingsId]
    ON [dbo].[ApprovalStep]([ApprovalSettingsId] ASC);

