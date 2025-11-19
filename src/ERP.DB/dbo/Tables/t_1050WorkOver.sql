CREATE TABLE [dbo].[t_1050WorkOver] (
    [Id]            INT             IDENTITY (1, 1) NOT NULL,
    [Applicant]     NVARCHAR (32)   NOT NULL,
    [Department]    NVARCHAR (32)   NULL,
    [JobTitle]      NVARCHAR (32)   NULL,
    [OvertimeDate]  DATETIME2 (7)   NULL,
    [OvertimeType]  INT             NOT NULL,
    [StartTime]     TIME (7)        NOT NULL,
    [EndTime]       TIME (7)        NOT NULL,
    [OverTimeHours] DECIMAL (12, 2) NULL,
    [Reason]        NVARCHAR (64)   NULL,
    [SignaturePath] NVARCHAR (64)   NULL,
    [Authorizator]  NVARCHAR (32)   NULL,
    [IsApproved]    BIT             NULL,
    CONSTRAINT [PK_t_1050WorkOver] PRIMARY KEY CLUSTERED ([Id] ASC)
);

