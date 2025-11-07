CREATE TABLE [dbo].[t_1700LoginLog] (
    [Id]        INT            IDENTITY (1, 1) NOT NULL,
    [Account]   NVARCHAR (MAX) NULL,
    [CrateDate] DATETIME2 (7)  DEFAULT ('0001-01-01T00:00:00.0000000') NOT NULL,
    [IpAddress] NVARCHAR (MAX) DEFAULT (N'') NOT NULL,
    [Method]    INT            DEFAULT ((0)) NOT NULL,
    [UserId]    INT            DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_t_1700LoginLog] PRIMARY KEY CLUSTERED ([Id] ASC)
);

