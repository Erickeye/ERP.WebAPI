using System;
using System.Collections.Generic;

namespace ERP.EntityModels.Models;

public partial class t_1700LoginLog
{
    public int Id { get; set; }

    public string? Account { get; set; }

    public DateTime CrateDate { get; set; }

    public string IpAddress { get; set; } = null!;

    public int Method { get; set; }

    public int UserId { get; set; }
}
