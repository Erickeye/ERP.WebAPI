using System;
using System.Collections.Generic;

namespace ERP.EntityModels.Models;

public partial class t_4031MaterialIssueDetail
{
    public int Id { get; set; }

    public int MaterialIssueId { get; set; }

    public int InventoryId { get; set; }

    public decimal Quantity { get; set; }

    public string? No { get; set; }

    public string Name { get; set; } = null!;

    public string? Unit { get; set; }

    public virtual t_4000Inventory Inventory { get; set; } = null!;

    public virtual t_4030MaterialIssue MaterialIssue { get; set; } = null!;
}
