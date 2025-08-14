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
    public class ApprovalStep
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey(nameof(ApprovalSettings))]
        [Display(Name = "關聯Id")]
        public int ApprovalSettingsId { get; set; }

        [Display(Name = "簽核順序")]
        public int StepOrder { get; set; }

        [Display(Name = "簽核角色")]
        public int RoleId { get; set; }

        [Display(Name = "簽核模式")]
        public ApprovalMode Mode { get; set; } // 指定人員 / 單人通過 / 自訂人數


        [Display(Name = "自訂人數")]
        public int? RequiredCounts { get; set; }

        public virtual ApprovalSettings ApprovalSettings { get; set; } = null!;
    }
}
