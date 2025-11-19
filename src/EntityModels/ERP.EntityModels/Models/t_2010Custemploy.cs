using System;
using System.Collections.Generic;

namespace ERP.EntityModels.Models;

public partial class t_2010Custemploy
{
    public int Id { get; set; }

    public int? MarriageStatus { get; set; }

    public string? Account { get; set; }

    public int CustomerId { get; set; }

    public string? Department { get; set; }

    public string? Email { get; set; }

    public string? ExtNum { get; set; }

    public string? Job { get; set; }

    public int? JobStatus { get; set; }

    public string? JobTitle { get; set; }

    public string? MobilePhone { get; set; }

    public string? Momo { get; set; }

    public string Name { get; set; } = null!;

    public virtual t_2000Customer Customer { get; set; } = null!;
}
