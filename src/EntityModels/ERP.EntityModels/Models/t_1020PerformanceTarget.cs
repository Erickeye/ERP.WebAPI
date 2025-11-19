using System;
using System.Collections.Generic;

namespace ERP.EntityModels.Models;

public partial class t_1020PerformanceTarget
{
    public int f_PTarget_ID { get; set; }

    public string f_staff_UID { get; set; } = null!;

    public string f_staff_ChineseName { get; set; } = null!;

    public DateTime f_PTarget_Annyal { get; set; }

    public decimal f_PTarget_Achieve { get; set; }
}
