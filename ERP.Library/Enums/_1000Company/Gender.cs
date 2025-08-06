using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Library.Enums._1000Company
{
    public enum Gender
    {
        [Display(Name = "未知", Description = "未知")]
        未知 = 0,
        [Display(Name = "男", Description = "男")]
        男 = 1,
        [Display(Name = "女", Description = "女")]
        女 = 2
    }
}
