using System;
using System.Collections.Generic;

namespace ERP.EntityModels.Models;

public partial class t_1060BusinessTrip
{
    public int Id { get; set; }

    public string No { get; set; } = null!;

    public int TravelerStaffId { get; set; }

    public int CreateUserId { get; set; }

    public string Destination { get; set; } = null!;

    public string Reason { get; set; } = null!;

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public int? CustomerId { get; set; }

    public string? Note { get; set; }

    public int Status { get; set; }

    public DateTime CreateTime { get; set; }

    public virtual User CreateUser { get; set; } = null!;

    public virtual t_2000Customer? Customer { get; set; }

    public virtual t_1000Staff TravelerStaff { get; set; } = null!;
}
