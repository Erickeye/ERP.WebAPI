using System;
using System.Collections.Generic;

namespace ERP.EntityModels.Models;

public partial class t_1710ActionInfo
{
    public int Id { get; set; }

    public string Account { get; set; } = null!;

    public int ActionType { get; set; }

    public DateTime CrateDate { get; set; }

    public string IpAddress { get; set; } = null!;

    public string? KeyId { get; set; }

    public string? Location { get; set; }

    public string? Memo { get; set; }

    public int UserId { get; set; }
}
