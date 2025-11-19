using System;
using System.Collections.Generic;

namespace ERP.EntityModels.Models;

public partial class Level
{
    public int PermissionId { get; set; }

    public int PermissionLevel { get; set; }

    public int LevelAmount { get; set; }
}
