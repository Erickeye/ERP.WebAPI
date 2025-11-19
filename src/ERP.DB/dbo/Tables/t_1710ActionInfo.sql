CREATE TABLE [dbo].[t_1710ActionInfo] (
    [Id]         INT            IDENTITY (1, 1) NOT NULL,
    [Account]    NVARCHAR (32)  DEFAULT (N'') NOT NULL,
    [ActionType] INT            DEFAULT ((0)) NOT NULL,
    [CrateDate]  DATETIME2 (7)  DEFAULT ('0001-01-01T00:00:00.0000000') NOT NULL,
    [IpAddress]  NVARCHAR (64)  DEFAULT (N'') NOT NULL,
    [KeyId]      NVARCHAR (16)  NULL,
    [Location]   NVARCHAR (512) NULL,
    [Memo]       NVARCHAR (512) NULL,
    [UserId]     INT            DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_t_1710ActionInfo] PRIMARY KEY CLUSTERED ([Id] ASC)
);

