using System;
using System.Collections.Generic;

namespace ERP.EntityModels.Models;

public partial class t_1100Department
{
    public string Id { get; set; } = null!;

    public string Name { get; set; } = null!;

    public virtual ICollection<t_1101DepartmentUnit> t_1101DepartmentUnits { get; set; } = new List<t_1101DepartmentUnit>();
}
