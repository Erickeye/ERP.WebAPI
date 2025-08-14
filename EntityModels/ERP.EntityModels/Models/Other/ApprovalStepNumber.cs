using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.EntityModels.Models.Other
{
    public class ApprovalStepNumber
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey(nameof(ApprovalStep))]
        public int ApprovalStepId { get; set; }

        [Display(Name = "使用者Id")]
        public int UserId { get; set; }

        public virtual ApprovalStep ApprovalStep { get; set; } = null!;
    }
}
