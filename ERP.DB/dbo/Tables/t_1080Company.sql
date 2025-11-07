CREATE TABLE [dbo].[t_1080Company] (
    [Id]                INT           IDENTITY (1, 1) NOT NULL,
    [AttribName]        NVARCHAR (32) NOT NULL,
    [Name]              NVARCHAR (32) NOT NULL,
    [TaxInvoiceNumber]  NVARCHAR (8)  NOT NULL,
    [TaxSerialNumber]   NVARCHAR (9)  NOT NULL,
    [Owner]             NVARCHAR (32) NOT NULL,
    [ContactPhone]      NVARCHAR (32) NULL,
    [FaxPhone]          NVARCHAR (32) NULL,
    [RegisteredAddress] NVARCHAR (64) NOT NULL,
    [DeliveryAddress]   NVARCHAR (64) NULL,
    [TaxInvoiceAddress] NVARCHAR (64) NULL,
    [BankName]          NVARCHAR (32) NULL,
    [CheckingAccount]   NVARCHAR (32) NULL,
    [RemittanceAccount] NVARCHAR (32) NULL,
    [PayDays]           NVARCHAR (8)  NULL,
    [FoundedDate]       DATETIME2 (7) NULL,
    [InvoiceForm]       NVARCHAR (8)  NULL,
    CONSTRAINT [PK_t_1080Company] PRIMARY KEY CLUSTERED ([Id] ASC)
);

