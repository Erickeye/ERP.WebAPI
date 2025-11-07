using System;
using System.Collections.Generic;

namespace ERP.EntityModels.Models;

public partial class Permission
{
    public int RoleId { get; set; }

    public int PageId { get; set; }

    public virtual Role Role { get; set; } = null!;
}
