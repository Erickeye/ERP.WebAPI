using System;
using System.Collections.Generic;

namespace ERP.EntityModels.Models;

public partial class ApprovalSettings
{
    public int Id { get; set; }

    public int TableType { get; set; }

    public string Name { get; set; } = null!;

    public bool IsActive { get; set; }

    public virtual ICollection<ApprovalStep> ApprovalStep { get; set; } = new List<ApprovalStep>();
}
