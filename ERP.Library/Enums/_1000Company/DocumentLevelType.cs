using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Library.Enums._1000Company
{
    public enum DocumentLevelType
    {
        [Display(Name = "普通件", Description = "普通件")]
        普通件 = 0,
        [Display(Name = "速件", Description = "速件")]
        速件 = 1,
        [Display(Name = "最速件", Description = "最速件")]
        最速件 = 2,
    }
}
