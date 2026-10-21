using System;
using System.Collections.Generic;

namespace ERP.EntityModels.Models;

public partial class t_4061SupplierPrice
{
    public int Id { get; set; }

    public int SupplierId { get; set; }

    public string No { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string Unit { get; set; } = null!;

    public decimal Price { get; set; }

    public string? Note { get; set; }

    public DateTime CreateTime { get; set; }

    public int CreateUserId { get; set; }

    public DateTime UpdateTime { get; set; }

    public int UpdateUserId { get; set; }

    public virtual t_4060Supplier Supplier { get; set; } = null!;
}
