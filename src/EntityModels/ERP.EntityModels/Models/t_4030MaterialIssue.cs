using System;
using System.Collections.Generic;

namespace ERP.EntityModels.Models;

public partial class t_4030MaterialIssue
{
    public int Id { get; set; }

    public string No { get; set; } = null!;

    public DateTime IssueDate { get; set; }

    public int PurposeId { get; set; }

    public int LocationId { get; set; }

    public int ReceiverStaffId { get; set; }

    public int? CustomerId { get; set; }

    public string? ProjectName { get; set; }

    public string? Note { get; set; }

    public int Status { get; set; }

    public DateTime CreateTime { get; set; }

    public int CreateUserId { get; set; }

    public virtual User CreateUser { get; set; } = null!;

    public virtual t_2000Customer? Customer { get; set; }

    public virtual SystemConfig Location { get; set; } = null!;

    public virtual SystemConfig Purpose { get; set; } = null!;

    public virtual t_1000Staff ReceiverStaff { get; set; } = null!;

    public virtual ICollection<t_4031MaterialIssueDetail> t_4031MaterialIssueDetail { get; set; } = new List<t_4031MaterialIssueDetail>();
}
