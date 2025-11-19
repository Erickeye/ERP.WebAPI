CREATE TABLE [dbo].[t_2010Custemploy] (
    [Id]             INT            IDENTITY (1, 1) NOT NULL,
    [MarriageStatus] INT            NULL,
    [Account]        NVARCHAR (32)  NULL,
    [CustomerId]     INT            DEFAULT ((0)) NOT NULL,
    [Department]     NVARCHAR (32)  NULL,
    [Email]          NVARCHAR (64)  NULL,
    [ExtNum]         NVARCHAR (32)  NULL,
    [Job]            NVARCHAR (32)  NULL,
    [JobStatus]      INT            NULL,
    [JobTitle]       NVARCHAR (32)  NULL,
    [MobilePhone]    NVARCHAR (32)  NULL,
    [Momo]           NVARCHAR (256) NULL,
    [Name]           NVARCHAR (32)  DEFAULT (N'') NOT NULL,
    CONSTRAINT [PK_t_2010Custemploy] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_t_2010Custemploy_t_2000Customer_CustomerId] FOREIGN KEY ([CustomerId]) REFERENCES [dbo].[t_2000Customer] ([Id]) ON DELETE CASCADE
);


GO
CREATE NONCLUSTERED INDEX [IX_t_2010Custemploy_CustomerId]
    ON [dbo].[t_2010Custemploy]([CustomerId] ASC);

