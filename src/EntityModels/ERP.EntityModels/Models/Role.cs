using System;
using System.Collections.Generic;

namespace ERP.EntityModels.Models;

public partial class Role
{
    public int Id { get; set; }

    public string RoleName { get; set; } = null!;

    public int? PermissionLevel { get; set; }

    public int? ApprovalLevel { get; set; }

    public int? QuotationLevel { get; set; }

    public int? ProcurementLevel { get; set; }

    public virtual ICollection<Permission> Permissions { get; set; } = new List<Permission>();

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
