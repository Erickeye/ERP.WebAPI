CREATE TABLE [dbo].[User] (
    [Id]                     INT            IDENTITY (1, 1) NOT NULL,
    [RoleId]                 INT            NOT NULL,
    [CreateDate]             DATETIME2 (7)  NOT NULL,
    [Name]                   NVARCHAR (32)  NOT NULL,
    [Account]                NVARCHAR (32)  NOT NULL,
    [Pwd]                    NVARCHAR (128) NOT NULL,
    [IsLock]                 BIT            NOT NULL,
    [RefreshToken]           NVARCHAR (MAX) NULL,
    [RefreshTokenExpiryTime] DATETIME2 (7)  NULL,
    CONSTRAINT [PK_User] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_User_Role_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [dbo].[Role] ([Id]) ON DELETE CASCADE
);


GO
CREATE NONCLUSTERED INDEX [IX_User_RoleId]
    ON [dbo].[User]([RoleId] ASC);

