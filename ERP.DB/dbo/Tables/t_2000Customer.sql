CREATE TABLE [dbo].[t_2000Customer] (
    [Id]                INT             IDENTITY (1, 1) NOT NULL,
    [TaxInvoiceNumber]  NVARCHAR (8)    NOT NULL,
    [PayDays]           NVARCHAR (8)    NULL,
    [CreditLine]        DECIMAL (12, 2) NULL,
    [CreditBalance]     DECIMAL (12, 2) NULL,
    [LastDeliveryDate]  DATETIME2 (7)   NULL,
    [Advance]           DECIMAL (12, 2) NULL,
    [AttribName]        NVARCHAR (32)   NULL,
    [BankName]          NVARCHAR (32)   NULL,
    [CheckingAccount]   NVARCHAR (32)   NULL,
    [ContactPhone]      NVARCHAR (32)   NULL,
    [DeliveryAddress]   NVARCHAR (64)   NULL,
    [FaxPhone]          NVARCHAR (32)   NULL,
    [InvoiceForm]       INT             NULL,
    [Name]              NVARCHAR (64)   DEFAULT (N'') NOT NULL,
    [Owner]             NVARCHAR (32)   DEFAULT (N'') NOT NULL,
    [RegisteredAddress] NVARCHAR (64)   DEFAULT (N'') NOT NULL,
    [RemittanceAccount] NVARCHAR (32)   NULL,
    [StaffChineseName]  NVARCHAR (32)   NULL,
    [TaxInvoiceAddress] NVARCHAR (64)   NULL,
    CONSTRAINT [PK_t_2000Customer] PRIMARY KEY CLUSTERED ([Id] ASC)
);

