using System;
using System.Collections.Generic;

namespace ERP.EntityModels.Models;

public partial class SystemConfig
{
    public int Id { get; set; }

    public string ConfigType { get; set; } = null!;

    public string? Code { get; set; }

    public string Name { get; set; } = null!;

    public int Sort { get; set; }

    public bool IsActive { get; set; }

    public virtual ICollection<t_4010Purchase> t_4010PurchaseLocation { get; set; } = new List<t_4010Purchase>();

    public virtual ICollection<t_4010Purchase> t_4010PurchasePaymentMethod { get; set; } = new List<t_4010Purchase>();
}
