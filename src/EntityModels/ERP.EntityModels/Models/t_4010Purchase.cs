using System;
using System.Collections.Generic;

namespace ERP.EntityModels.Models;

public partial class t_4010Purchase
{
    public int Id { get; set; }

    public string? No { get; set; }

    public int SupplierId { get; set; }

    public int? CustomerId { get; set; }

    public int LocationId { get; set; }

    public string ProjectName { get; set; } = null!;

    public DateTime PurchaseDate { get; set; }

    public bool IsPurchase { get; set; }

    public int PaymentMethodId { get; set; }

    public decimal Amount { get; set; }

    public decimal Tax { get; set; }

    public string Payer { get; set; } = null!;

    public string? InvoiceNumber { get; set; }

    public string? Note { get; set; }

    public string? Authorizator { get; set; }

    public bool IsApproval { get; set; }

    public DateTime CreateTime { get; set; }

    public int CreateUserId { get; set; }

    public virtual User CreateUser { get; set; } = null!;

    public virtual t_2000Customer? Customer { get; set; }

    public virtual SystemConfig Location { get; set; } = null!;

    public virtual SystemConfig PaymentMethod { get; set; } = null!;

    public virtual t_4060Supplier Supplier { get; set; } = null!;

    public virtual ICollection<t_4011PurchaseDetail> t_4011PurchaseDetail { get; set; } = new List<t_4011PurchaseDetail>();
}
