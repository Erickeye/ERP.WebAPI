CREATE TABLE [dbo].[t_1001StaffCertificates] (
    [Id]              INT             IDENTITY (1, 1) NOT NULL,
    [StaffId]         INT             NOT NULL,
    [Certificate]     VARBINARY (MAX) NULL,
    [CertificateName] NVARCHAR (128)  NULL,
    [CertificateDate] DATETIME2 (7)   NULL,
    [EffectiveDate]   INT             NULL,
    [IsNotify]        BIT             NULL,
    [NotifyDate]      DATETIME2 (7)   NULL,
    CONSTRAINT [PK_t_1001StaffCertificates] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_t_1001StaffCertificates_t_1000Staff_StaffId] FOREIGN KEY ([StaffId]) REFERENCES [dbo].[t_1000Staff] ([StaffId]) ON DELETE CASCADE
);


GO
CREATE NONCLUSTERED INDEX [IX_t_1001StaffCertificates_StaffId]
    ON [dbo].[t_1001StaffCertificates]([StaffId] ASC);

