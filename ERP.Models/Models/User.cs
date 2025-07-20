using System;
using System.Collections.Generic;

namespace ERP.EntityModels.Models;

public partial class User
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public DateOnly? CreateDate { get; set; }
}
