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
        [Display(Name ="單人")]
        Single = 0,
        [Display(Name = "全部")]
        All = 1,
        [Display(Name = "自訂人數")]
        Customized = 2
    }
}
