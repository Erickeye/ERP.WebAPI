using System;
using System.Collections.Generic;

namespace ERP.EntityModels.Models;

public partial class t_1050WorkOver
{
    public int Id { get; set; }

    public string Applicant { get; set; } = null!;

    public string? Department { get; set; }

    public string? JobTitle { get; set; }

    public DateTime? OvertimeDate { get; set; }

    public int OvertimeType { get; set; }

    public TimeOnly StartTime { get; set; }

    public TimeOnly EndTime { get; set; }

    public decimal? OverTimeHours { get; set; }

    public string? Reason { get; set; }

    public string? SignaturePath { get; set; }

    public string? Authorizator { get; set; }

    public bool? IsApproved { get; set; }
}
