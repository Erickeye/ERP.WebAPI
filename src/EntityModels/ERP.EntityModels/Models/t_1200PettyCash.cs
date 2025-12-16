using System;
using System.Collections.Generic;

namespace ERP.EntityModels.Models;

public partial class t_1200PettyCash
{
    public DateTime RequestDate { get; set; }

    public decimal? TotalAmount { get; set; }

    public DateTime? PaymentDate { get; set; }

    public bool? Approval { get; set; }

    public bool? Accounting { get; set; }

    public string Id { get; set; } = null!;

    public string? Authorizator { get; set; }

    public string? Company { get; set; }

    public string Filler { get; set; } = null!;

    public string Payee { get; set; } = null!;

    public string? Reason { get; set; }

    public string? Supervisor { get; set; }

    public virtual ICollection<t_1201PettyCashDetail> t_1201PettyCashDetail { get; set; } = new List<t_1201PettyCashDetail>();
}
