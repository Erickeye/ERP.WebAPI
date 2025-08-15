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
    public class ApprovalRecord
    {
        [Key]
        [Display(Name = "流水號")]
        public int Id { get; set; }

        [Display(Name = "關聯Id")]
        [ForeignKey(nameof(ApprovalStep))]
        public int ApprovalStepId { get; set; }

        [StringLength(32, ErrorMessage = "長度不可超過 32 個字元")]
        [Display(Name = "表單Id")]
        public string? TableId { get; set; }

        [Display(Name = "簽核順序")]
        public int StepOrder { get; set; }

        [Display(Name = "簽核角色")]
        public int RoleId { get; set; }

        [Display(Name = "簽核者")]
        public int? UserId { get; set; }

        [Display(Name = "簽核狀態")]
        public ApprovalStatus Status { get; set; }

        [Display(Name = "簽核日期")]
        public System.DateTime? Date { get; set; }

        [StringLength(256, ErrorMessage = "長度不可超過 256 個字元")]
        [Display(Name = "備註")]
        public string? Memo { get; set; }

        public virtual ApprovalStep ApprovalStep { get; set; } = null!;
    }
}
