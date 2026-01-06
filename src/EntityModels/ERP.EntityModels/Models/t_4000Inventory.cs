using System;
using System.Collections.Generic;

namespace ERP.EntityModels.Models;

public partial class t_4000Inventory
{
    public int Id { get; set; }

    public int SupplierId { get; set; }

    public string Name { get; set; } = null!;

    public int LocationId { get; set; }

    public string? Category { get; set; }

    public DateTime? LastPurchaseDate { get; set; }

    public string? Number { get; set; }

    public string? Unit { get; set; }

    public decimal? Quantity { get; set; }

    public decimal? Amount { get; set; }

    public decimal? Total { get; set; }

    public virtual SystemConfig Location { get; set; } = null!;

    public virtual t_4060Supplier Supplier { get; set; } = null!;
}
