using System;
using System.Collections.Generic;

namespace ERP.EntityModels.Models;

public partial class t_1201PettyCashDetail
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Project { get; set; }

    public string? InvoiceNumber { get; set; }

    public string? PurchaseId { get; set; }

    public string? Vehicle { get; set; }

    public string? Supplier { get; set; }

    public string? Content { get; set; }

    public DateTime? InvoiceDate { get; set; }

    public decimal? Amount { get; set; }

    public decimal? Tax { get; set; }

    public decimal? Total { get; set; }

    public int? Sort { get; set; }

    public string PettyCashId { get; set; } = null!;

    public virtual t_1200PettyCash PettyCash { get; set; } = null!;
}
