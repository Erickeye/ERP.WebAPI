CREATE TABLE [dbo].[t_1039DayoffProxy] (
    [Id]         INT           IDENTITY (1, 1) NOT NULL,
    [DayoffId]   INT           NOT NULL,
    [ProxyId]    INT           NOT NULL,
    [ProxyAgree] BIT           NULL,
    [DateTime]   DATETIME2 (7) NULL,
    [SelfId]     INT           NOT NULL,
    [IsConfirm]  BIT           NULL,
    CONSTRAINT [PK_t_1039DayoffProxy] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_t_1039DayoffProxy_t_1030Dayoff_DayoffId] FOREIGN KEY ([DayoffId]) REFERENCES [dbo].[t_1030Dayoff] ([Id]) ON DELETE CASCADE
);


GO
CREATE NONCLUSTERED INDEX [IX_t_1039DayoffProxy_DayoffId]
    ON [dbo].[t_1039DayoffProxy]([DayoffId] ASC);

