using System;
using System.Collections.Generic;

namespace ERP.EntityModels.Models;

public partial class _4011PurchaseDetail
{
    public int Id { get; set; }

    public int PurchaseId { get; set; }

    public string? Category { get; set; }

    public string? No { get; set; }

    public string Name { get; set; } = null!;

    public string? Unit { get; set; }

    public decimal Quantity { get; set; }

    public decimal? Price { get; set; }

    public decimal? Total { get; set; }

    public virtual t_4010Purchase Purchase { get; set; } = null!;
}
