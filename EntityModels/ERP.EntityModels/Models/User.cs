using System;
using System.Collections.Generic;

namespace ERP.EntityModels.Models;

public partial class User
{
    public int Id { get; set; }

    public int RoleId { get; set; }

    public DateTime CreateDate { get; set; }

    public string Name { get; set; } = null!;

    public string Account { get; set; } = null!;

    public string Pwd { get; set; } = null!;

    public bool IsLock { get; set; }

    public string? RefreshToken { get; set; }

    public DateTime? RefreshTokenExpiryTime { get; set; }

    public virtual Role Role { get; set; } = null!;
}
