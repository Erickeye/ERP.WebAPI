CREATE TABLE [dbo].[t_4030MaterialIssue]
(
    [Id] INT IDENTITY (1, 1) NOT NULL,
    [No] VARCHAR(64) NOT NULL,
    [IssueDate] DATETIME NOT NULL,
    [PurposeId] INT NOT NULL,
    [LocationId] INT NOT NULL,
    [ReceiverStaffId] INT NOT NULL,
    [CustomerId] INT NULL,
    [ProjectName] NVARCHAR(128) NULL,
    [Note] NVARCHAR(1024) NULL,
    [Status] INT NOT NULL DEFAULT 0,
    [CreateTime] DATETIME NOT NULL,
    [CreateUserId] INT NOT NULL,
    -- Primary Key
    CONSTRAINT [PK_t_4030MaterialIssue] PRIMARY KEY CLUSTERED ([Id] DESC),
    -- Foreign Keys
    CONSTRAINT [FK_t_4030MaterialIssue_Purpose]  FOREIGN KEY ([PurposeId])       REFERENCES [dbo].[SystemConfig] ([Id]),
    CONSTRAINT [FK_t_4030MaterialIssue_Location] FOREIGN KEY ([LocationId])      REFERENCES [dbo].[SystemConfig] ([Id]),
    CONSTRAINT [FK_t_4030MaterialIssue_Receiver] FOREIGN KEY ([ReceiverStaffId]) REFERENCES [dbo].[t_1000Staff] ([StaffId]),
    CONSTRAINT [FK_t_4030MaterialIssue_Customer] FOREIGN KEY ([CustomerId])      REFERENCES [dbo].[t_2000Customer] ([Id]),
    CONSTRAINT [FK_t_4030MaterialIssue_CreateUser] FOREIGN KEY ([CreateUserId])  REFERENCES [dbo].[User] ([Id])
)
GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_t_4030MaterialIssue_No]
ON [dbo].[t_4030MaterialIssue] ([No]);
GO
CREATE NONCLUSTERED INDEX [IX_t_4030MaterialIssue_IssueDate]
ON [dbo].[t_4030MaterialIssue] ([IssueDate]);
GO
CREATE NONCLUSTERED INDEX [IX_t_4030MaterialIssue_Status]
ON [dbo].[t_4030MaterialIssue] ([Status]);
GO
CREATE NONCLUSTERED INDEX [IX_t_4030MaterialIssue_LocationId]
ON [dbo].[t_4030MaterialIssue] ([LocationId]);
GO
CREATE NONCLUSTERED INDEX [IX_t_4030MaterialIssue_ReceiverStaffId]
ON [dbo].[t_4030MaterialIssue] ([ReceiverStaffId]);
GO
