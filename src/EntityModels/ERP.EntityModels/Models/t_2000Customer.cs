using System;
using System.Collections.Generic;

namespace ERP.EntityModels.Models;

public partial class t_2000Customer
{
    public int Id { get; set; }

    public string TaxInvoiceNumber { get; set; } = null!;

    public string? PayDays { get; set; }

    public decimal? CreditLine { get; set; }

    public decimal? CreditBalance { get; set; }

    public DateTime? LastDeliveryDate { get; set; }

    public decimal? Advance { get; set; }

    public string? AttribName { get; set; }

    public string? BankName { get; set; }

    public string? CheckingAccount { get; set; }

    public string? ContactPhone { get; set; }

    public string? DeliveryAddress { get; set; }

    public string? FaxPhone { get; set; }

    public int? InvoiceForm { get; set; }

    public string Name { get; set; } = null!;

    public string Owner { get; set; } = null!;

    public string RegisteredAddress { get; set; } = null!;

    public string? RemittanceAccount { get; set; }

    public string? StaffChineseName { get; set; }

    public string? TaxInvoiceAddress { get; set; }

    public virtual ICollection<t_2010Custemploy> t_2010Custemploy { get; set; } = new List<t_2010Custemploy>();

    public virtual ICollection<t_4010Purchase> t_4010Purchase { get; set; } = new List<t_4010Purchase>();
}
