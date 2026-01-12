using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Library.Enums
{
    public enum ApiGroupType
    {
        [Display(Name = "API首頁")]
        Home,
        [Display(Name = "公司相關")]
        Company,
        [Display(Name = "客戶資料")]
        Customer,
        [Display(Name = "庫存管理")]
        Inventory
    }
}
