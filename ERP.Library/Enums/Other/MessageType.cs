using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Library.Enums.Other
{
    public enum MessageType
    {
        [Display(Name = "一般訊息")]
        General = 0,

        [Display(Name = "請假訊息")]
        Leave = 1,

        [Display(Name = "進貨簽核")]
        PurchaseSign = 2
    }
}
