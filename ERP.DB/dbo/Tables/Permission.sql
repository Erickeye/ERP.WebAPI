CREATE TABLE [dbo].[Permission] (
    [RoleId] INT NOT NULL,
    [PageId] INT NOT NULL,
    CONSTRAINT [PK_Permission] PRIMARY KEY CLUSTERED ([RoleId] ASC, [PageId] ASC),
    CONSTRAINT [FK_Permission_Role_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [dbo].[Role] ([Id]) ON DELETE CASCADE
);

