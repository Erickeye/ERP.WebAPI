CREATE TABLE [dbo].[t_1030Dayoff] (
    [Id]              INT            IDENTITY (1, 1) NOT NULL,
    [ApplicationDate] DATETIME2 (7)  NULL,
    [LeaveTaker]      INT            NOT NULL,
    [Applicant]       INT            NULL,
    [Proxy]           INT            NULL,
    [LeaveType]       INT            NOT NULL,
    [Reason]          NVARCHAR (64)  NULL,
    [ProxySignature]  NVARCHAR (128) NULL,
    [BeginDate]       DATETIME2 (7)  NOT NULL,
    [EndDate]         DATETIME2 (7)  NOT NULL,
    [ApprovalStatus]  INT            NULL,
    CONSTRAINT [PK_t_1030Dayoff] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_t_1030Dayoff_t_1000Staff_Applicant] FOREIGN KEY ([Applicant]) REFERENCES [dbo].[t_1000Staff] ([StaffId]),
    CONSTRAINT [FK_t_1030Dayoff_t_1000Staff_LeaveTaker] FOREIGN KEY ([LeaveTaker]) REFERENCES [dbo].[t_1000Staff] ([StaffId]) ON DELETE CASCADE,
    CONSTRAINT [FK_t_1030Dayoff_t_1000Staff_Proxy] FOREIGN KEY ([Proxy]) REFERENCES [dbo].[t_1000Staff] ([StaffId])
);


GO
CREATE NONCLUSTERED INDEX [IX_t_1030Dayoff_Proxy]
    ON [dbo].[t_1030Dayoff]([Proxy] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_t_1030Dayoff_Applicant]
    ON [dbo].[t_1030Dayoff]([Applicant] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_t_1030Dayoff_LeaveTaker]
    ON [dbo].[t_1030Dayoff]([LeaveTaker] ASC);

