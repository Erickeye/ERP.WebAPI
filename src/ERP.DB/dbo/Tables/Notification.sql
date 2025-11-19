CREATE TABLE [dbo].[Notification] (
    [Id]       INT            IDENTITY (1, 1) NOT NULL,
    [DateTime] DATETIME2 (7)  NOT NULL,
    [Message]  NVARCHAR (256) NULL,
    [Type]     INT            NOT NULL,
    [TargetId] NVARCHAR (32)  NULL,
    [UserId]   INT            NOT NULL,
    [IsRead]   BIT            DEFAULT (CONVERT([bit],(0))) NOT NULL,
    [IsShow]   BIT            DEFAULT (CONVERT([bit],(0))) NOT NULL,
    CONSTRAINT [PK_Notification] PRIMARY KEY CLUSTERED ([Id] ASC)
);

