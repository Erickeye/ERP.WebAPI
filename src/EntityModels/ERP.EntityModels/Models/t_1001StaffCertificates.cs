using System;
using System.Collections.Generic;

namespace ERP.EntityModels.Models;

public partial class t_1001StaffCertificates
{
    public int Id { get; set; }

    public int StaffId { get; set; }

    public byte[]? Certificate { get; set; }

    public string? CertificateName { get; set; }

    public DateTime? CertificateDate { get; set; }

    public int? EffectiveDate { get; set; }

    public bool? IsNotify { get; set; }

    public DateTime? NotifyDate { get; set; }

    public virtual t_1000Staff Staff { get; set; } = null!;
}
