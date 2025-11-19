CREATE TABLE [dbo].[t_1040Document] (
    [Id]            INT            IDENTITY (1, 1) NOT NULL,
    [Company]       NVARCHAR (64)  NOT NULL,
    [ContactPerson] NVARCHAR (32)  NOT NULL,
    [Recipient]     NVARCHAR (64)  NOT NULL,
    [DocumentDate]  DATETIME2 (7)  NOT NULL,
    [Level]         INT            NOT NULL,
    [Attachment]    NVARCHAR (32)  NULL,
    [Subject]       NVARCHAR (256) NULL,
    [Original]      NVARCHAR (64)  NULL,
    [Remark1]       NVARCHAR (MAX) NULL,
    [Remark2]       NVARCHAR (64)  NULL,
    [File]          NVARCHAR (MAX) NULL,
    [Contract]      NVARCHAR (32)  NULL,
    [Authorizator]  NVARCHAR (32)  NULL,
    [Approval]      BIT            NULL,
    CONSTRAINT [PK_t_1040Document] PRIMARY KEY CLUSTERED ([Id] ASC)
);

