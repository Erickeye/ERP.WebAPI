using System;
using System.Collections.Generic;

namespace ERP.EntityModels.Models;

public partial class ApprovalStep
{
    public int Id { get; set; }

    public int ApprovalSettingsId { get; set; }

    public int StepOrder { get; set; }

    public int RoleId { get; set; }

    public int Mode { get; set; }

    public int? RequiredCounts { get; set; }

    public virtual ICollection<ApprovalRecord> ApprovalRecord { get; set; } = new List<ApprovalRecord>();

    public virtual ApprovalSettings ApprovalSettings { get; set; } = null!;

    public virtual ICollection<ApprovalStepNumber> ApprovalStepNumber { get; set; } = new List<ApprovalStepNumber>();
}
