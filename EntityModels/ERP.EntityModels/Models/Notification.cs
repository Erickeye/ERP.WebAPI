using System;
using System.Collections.Generic;

namespace ERP.EntityModels.Models;

public partial class Notification
{
    public int Id { get; set; }

    public DateTime DateTime { get; set; }

    public string? Message { get; set; }

    public int Type { get; set; }

    public string? TargetId { get; set; }

    public int UserId { get; set; }

    public bool IsRead { get; set; }

    public bool IsShow { get; set; }
}
