using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Library.Enums._1000Company
{
    public enum BloodType
    {
        [Display(Name = "未知", Description = "未知")]
        未知 = 0,
        [Display(Name = "A型", Description = "A型")]
        A = 1,
        [Display(Name = "B型", Description = "B型")]
        B = 2,
        [Display(Name = "AB型", Description = "AB型")]
        AB = 3,
        [Display(Name = "O型", Description = "O型")]
        O = 4
    }
}
