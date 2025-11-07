using System;
using System.Collections.Generic;

namespace ERP.EntityModels.Models;

public partial class t_1101DepartmentUnit
{
    public int Id { get; set; }

    public int StaffId { get; set; }

    public string DepartmentId { get; set; } = null!;

    public bool IsManager { get; set; }

    public string JobTitle { get; set; } = null!;

    public virtual t_1100Department Department { get; set; } = null!;

    public virtual t_1000Staff Staff { get; set; } = null!;
}
