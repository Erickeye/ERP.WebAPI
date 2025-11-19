using System;
using System.Collections.Generic;

namespace ERP.EntityModels.Models;

public partial class ApprovalStepNumber
{
    public int Id { get; set; }

    public int ApprovalStepId { get; set; }

    public int UserId { get; set; }

    public virtual ApprovalStep ApprovalStep { get; set; } = null!;
}
