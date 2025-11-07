using System;
using System.Collections.Generic;

namespace ERP.EntityModels.Models;

public partial class t_1080Company
{
    public int Id { get; set; }

    public string AttribName { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string TaxInvoiceNumber { get; set; } = null!;

    public string TaxSerialNumber { get; set; } = null!;

    public string Owner { get; set; } = null!;

    public string? ContactPhone { get; set; }

    public string? FaxPhone { get; set; }

    public string RegisteredAddress { get; set; } = null!;

    public string? DeliveryAddress { get; set; }

    public string? TaxInvoiceAddress { get; set; }

    public string? BankName { get; set; }

    public string? CheckingAccount { get; set; }

    public string? RemittanceAccount { get; set; }

    public string? PayDays { get; set; }

    public DateTime? FoundedDate { get; set; }

    public string? InvoiceForm { get; set; }
}
