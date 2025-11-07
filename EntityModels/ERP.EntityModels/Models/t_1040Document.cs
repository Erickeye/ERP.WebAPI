using System;
using System.Collections.Generic;

namespace ERP.EntityModels.Models;

public partial class t_1040Document
{
    public int Id { get; set; }

    public string Company { get; set; } = null!;

    public string ContactPerson { get; set; } = null!;

    public string Recipient { get; set; } = null!;

    public DateTime DocumentDate { get; set; }

    public int Level { get; set; }

    public string? Attachment { get; set; }

    public string? Subject { get; set; }

    public string? Original { get; set; }

    public string? Remark1 { get; set; }

    public string? Remark2 { get; set; }

    public string? File { get; set; }

    public string? Contract { get; set; }

    public string? Authorizator { get; set; }

    public bool? Approval { get; set; }
}
