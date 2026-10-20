CREATE TABLE [dbo].[t_1060BusinessTrip]
(
    [Id] INT IDENTITY (1, 1) NOT NULL,
    [No] VARCHAR(32) NOT NULL,
    [TravelerStaffId] INT NOT NULL,
    [CreateUserId] INT NOT NULL,
    [Destination] NVARCHAR(128) NOT NULL,
    [Reason] NVARCHAR(256) NOT NULL,
    [StartDate] DATE NOT NULL,
    [EndDate] DATE NOT NULL,
    [CustomerId] INT NULL,
    [Note] NVARCHAR(1024) NULL,
    [Status] INT NOT NULL DEFAULT 0,
    [CreateTime] DATETIME NOT NULL,
    [CompanyId] INT NOT NULL,
    -- Primary Key
    CONSTRAINT [PK_t_1060BusinessTrip] PRIMARY KEY CLUSTERED ([Id] DESC),
    -- Check：迄日不得早於起日
    CONSTRAINT [CK_t_1060BusinessTrip_Dates] CHECK ([EndDate] >= [StartDate]),
    -- Foreign Keys
    CONSTRAINT [FK_t_1060BusinessTrip_Traveler]   FOREIGN KEY ([TravelerStaffId]) REFERENCES [dbo].[t_1000Staff] ([StaffId]),
    CONSTRAINT [FK_t_1060BusinessTrip_CreateUser] FOREIGN KEY ([CreateUserId])    REFERENCES [dbo].[User] ([Id]),
    CONSTRAINT [FK_t_1060BusinessTrip_Customer]   FOREIGN KEY ([CustomerId])      REFERENCES [dbo].[t_2000Customer] ([Id]),
    CONSTRAINT [FK_t_1060BusinessTrip_Company]    FOREIGN KEY ([CompanyId])       REFERENCES [dbo].[t_1080Company] ([Id])
)
GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_t_1060BusinessTrip_No]
ON [dbo].[t_1060BusinessTrip] ([No]);
GO
CREATE NONCLUSTERED INDEX [IX_t_1060BusinessTrip_TravelerStaffId]
ON [dbo].[t_1060BusinessTrip] ([TravelerStaffId], [StartDate], [EndDate]);
GO
CREATE NONCLUSTERED INDEX [IX_t_1060BusinessTrip_CreateUserId]
ON [dbo].[t_1060BusinessTrip] ([CreateUserId]);
GO
CREATE NONCLUSTERED INDEX [IX_t_1060BusinessTrip_StartDate]
ON [dbo].[t_1060BusinessTrip] ([StartDate]);
GO
CREATE NONCLUSTERED INDEX [IX_t_1060BusinessTrip_Status]
ON [dbo].[t_1060BusinessTrip] ([Status]);
GO
CREATE NONCLUSTERED INDEX [IX_t_1060BusinessTrip_CompanyId]
ON [dbo].[t_1060BusinessTrip] ([CompanyId]);
GO
