using ERP.EntityModels.Models._1000Company;
using ERP.Library.Enums.Other;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.EntityModels.Models.Other
{
    public class ApprovalSettings
    {
        [Key]
        [Display(Name = "流水號")]
        public int Id { get; set; }

        [Display(Name = "表單類型")]
        public TableType TableType { get; set; }

        [StringLength(64, ErrorMessage = "長度不可超過 64 個字元")]
        [Display(Name = "流程名稱")]
        public string Name { get; set; } = string.Empty;

        [Display(Name = "是否啟用")]
        public bool IsActive { get; set; }

    }
}
