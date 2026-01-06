using System;
using System.Collections.Generic;

namespace ERP.EntityModels.Models;

public partial class t_4060Supplier
{
    public int Id { get; set; }

    public string? No { get; set; }

    public string? Name { get; set; }

    public string? TaxIdNumber { get; set; }

    public string? Owner { get; set; }

    public string? ContactPhone { get; set; }

    public string? FaxPhone { get; set; }

    public string? Salesperson { get; set; }

    public string? Address { get; set; }

    public string? BankName { get; set; }

    public string? SubBankName { get; set; }

    public string? CheckingAccount { get; set; }

    public string? RemittanceAccount { get; set; }

    public DateTime? PayDays { get; set; }

    public decimal? CreditLine { get; set; }

    public decimal? CreditBalance { get; set; }

    public DateTime? LastTransDate { get; set; }

    public decimal? TemporaryAmount { get; set; }

    public virtual ICollection<t_4000Inventory> t_4000Inventory { get; set; } = new List<t_4000Inventory>();

    public virtual ICollection<t_4010Purchase> t_4010Purchase { get; set; } = new List<t_4010Purchase>();
}
