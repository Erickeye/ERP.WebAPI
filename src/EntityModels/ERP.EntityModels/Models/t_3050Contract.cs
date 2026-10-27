using System;
using System.Collections.Generic;

namespace ERP.EntityModels.Models;

public partial class t_3050Contract
{
    public int Id { get; set; }

    public string No { get; set; } = null!;

    public string ContractNo { get; set; } = null!;

    public int Type { get; set; }

    public int? SupplierId { get; set; }

    public int? CustomerId { get; set; }

    public int CompanyId { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public decimal Amount { get; set; }

    public string? Remark { get; set; }

    public string? AttachmentPath { get; set; }

    public int Status { get; set; }

    public DateTime CreateTime { get; set; }

    public int CreateUserId { get; set; }

    public DateTime UpdateTime { get; set; }

    public int UpdateUserId { get; set; }

    public virtual t_1080Company Company { get; set; } = null!;

    public virtual User CreateUser { get; set; } = null!;

    public virtual t_2000Customer? Customer { get; set; }

    public virtual t_4060Supplier? Supplier { get; set; }
}
