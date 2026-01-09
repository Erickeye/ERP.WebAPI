using System;
using System.Collections.Generic;

namespace ERP.EntityModels.Models;

public partial class SerialNumber
{
    public int Id { get; set; }

    public string Prefix { get; set; } = null!;

    public string DateKey { get; set; } = null!;

    public int CurrentNo { get; set; }
}
