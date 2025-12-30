using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Library.Enums._1000Company
{
    public enum LeaveType
    {
        [Display(Name = "特休", Description = "特休")]
        特休 = 1,
        [Display(Name = "事假", Description = "事假")]
        事假 = 2,
        [Display(Name = "病假", Description = "病假")]
        病假 = 3,
        [Display(Name = "公假", Description = "公假")]
        公假 = 4,
        [Display(Name = "公傷病假", Description = "公傷病假")]
        公傷病假 = 5,
        [Display(Name = "生理假", Description = "生理假")]
        生理假 = 6,
        [Display(Name = "陪產假", Description = "陪產假")]
        陪產假 = 7,
        [Display(Name = "婚假", Description = "婚假")]
        婚假 = 8,
        [Display(Name = "產假", Description = "產假")]
        產假 = 9,
        [Display(Name = "喪假", Description = "喪假")]
        喪假 = 10,
        [Display(Name = "補休", Description = "補休")]
        補休 = 11,
        [Display(Name = "其他", Description = "其他")]
        其他 = 12
    }
}
