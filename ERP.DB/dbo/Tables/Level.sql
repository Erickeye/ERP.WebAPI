CREATE TABLE [dbo].[Level] (
    [PermissionId]    INT IDENTITY (1, 1) NOT NULL,
    [PermissionLevel] INT NOT NULL,
    [LevelAmount]     INT NOT NULL,
    CONSTRAINT [PK_Level] PRIMARY KEY CLUSTERED ([PermissionId] ASC)
);

