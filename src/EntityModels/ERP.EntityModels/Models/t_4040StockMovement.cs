using System;
using System.Collections.Generic;

namespace ERP.EntityModels.Models;

public partial class t_4040StockMovement
{
    public int Id { get; set; }

    public int InventoryId { get; set; }

    public int MovementType { get; set; }

    public int SourceTableType { get; set; }

    public string SourceNo { get; set; } = null!;

    public decimal Quantity { get; set; }

    public decimal BeforeQuantity { get; set; }

    public decimal AfterQuantity { get; set; }

    public int OperatorUserId { get; set; }

    public DateTime MovementTime { get; set; }

    public virtual t_4000Inventory Inventory { get; set; } = null!;

    public virtual User OperatorUser { get; set; } = null!;
}
