CREATE TABLE [dbo].[t_1101DepartmentUnit] (
    [Id]           INT           IDENTITY (1, 1) NOT NULL,
    [StaffId]      INT           NOT NULL,
    [DepartmentId] NVARCHAR (16) NOT NULL,
    [IsManager]    BIT           NOT NULL,
    [JobTitle]     NVARCHAR (16) NOT NULL,
    CONSTRAINT [PK_t_1101DepartmentUnit] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_t_1101DepartmentUnit_t_1000Staff_StaffId] FOREIGN KEY ([StaffId]) REFERENCES [dbo].[t_1000Staff] ([StaffId]) ON DELETE CASCADE,
    CONSTRAINT [FK_t_1101DepartmentUnit_t_1100Department_DepartmentId] FOREIGN KEY ([DepartmentId]) REFERENCES [dbo].[t_1100Department] ([Id]) ON DELETE CASCADE
);


GO
CREATE NONCLUSTERED INDEX [IX_t_1101DepartmentUnit_StaffId]
    ON [dbo].[t_1101DepartmentUnit]([StaffId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_t_1101DepartmentUnit_DepartmentId]
    ON [dbo].[t_1101DepartmentUnit]([DepartmentId] ASC);

