using System;
using System.Collections.Generic;

namespace ERP.EntityModels.Models;

public partial class t_1039DayoffProxy
{
    public int Id { get; set; }

    public int DayoffId { get; set; }

    public int ProxyId { get; set; }

    public bool? ProxyAgree { get; set; }

    public DateTime? DateTime { get; set; }

    public int SelfId { get; set; }

    public bool? IsConfirm { get; set; }

    public virtual t_1030Dayoff Dayoff { get; set; } = null!;
}
