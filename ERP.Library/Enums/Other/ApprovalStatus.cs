using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Library.Enums.Other
{
    public enum ApprovalStatus
    {
        [Display(Name = "等待中")]
        Pending = 0,
        [Display(Name = "核准")]
        Approved = 1,
        [Display(Name = "拒絕")]
        Rejected = 2,
    }
}
