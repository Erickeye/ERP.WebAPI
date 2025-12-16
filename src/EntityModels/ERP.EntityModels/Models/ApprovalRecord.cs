using System;
using System.Collections.Generic;

namespace ERP.EntityModels.Models;

public partial class ApprovalRecord
{
    public int Id { get; set; }

    public int ApprovalStepId { get; set; }

    public int? RoleId { get; set; }

    public string? TableId { get; set; }

    public int StepOrder { get; set; }

    public int? UserId { get; set; }

    public int Status { get; set; }

    public DateTime? Date { get; set; }

    public string? Memo { get; set; }

    public int TableType { get; set; }

    public virtual ApprovalStep ApprovalStep { get; set; } = null!;
}
