using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Library.Enums.Other
{
    public enum ApprovalMode
    {
        [Display(Name = "指定人員(可複數)")]
        Specify = 0,
        [Display(Name = "單人")]
        Single = 1,
        [Display(Name = "指定角色(設定數量[RequiredCounts]")]
        Role = 2,
    }
}
