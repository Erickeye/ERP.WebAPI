CREATE TABLE [dbo].[t_1020PerformanceTarget] (
    [f_PTarget_ID]        INT             IDENTITY (1, 1) NOT NULL,
    [f_staff_UID]         NVARCHAR (MAX)  NOT NULL,
    [f_staff_ChineseName] NVARCHAR (MAX)  NOT NULL,
    [f_PTarget_Annyal]    DATETIME2 (7)   NOT NULL,
    [f_PTarget_Achieve]   DECIMAL (12, 2) NOT NULL,
    CONSTRAINT [PK_t_1020PerformanceTarget] PRIMARY KEY CLUSTERED ([f_PTarget_ID] ASC)
);

