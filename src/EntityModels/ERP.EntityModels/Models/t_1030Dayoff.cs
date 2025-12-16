using System;
using System.Collections.Generic;

namespace ERP.EntityModels.Models;

public partial class t_1030Dayoff
{
    public int Id { get; set; }

    public DateTime? ApplicationDate { get; set; }

    public int LeaveTaker { get; set; }

    public int? Applicant { get; set; }

    public int? Proxy { get; set; }

    public int LeaveType { get; set; }

    public string? Reason { get; set; }

    public string? ProxySignature { get; set; }

    public DateTime BeginDate { get; set; }

    public DateTime EndDate { get; set; }

    public int? ApprovalStatus { get; set; }

    public virtual t_1000Staff? ApplicantNavigation { get; set; }

    public virtual t_1000Staff LeaveTakerNavigation { get; set; } = null!;

    public virtual t_1000Staff? ProxyNavigation { get; set; }

    public virtual ICollection<t_1039DayoffProxy> t_1039DayoffProxy { get; set; } = new List<t_1039DayoffProxy>();
}
