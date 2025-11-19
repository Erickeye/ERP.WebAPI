using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Library.ViewModels.Approval
{
    public class ApprovalStepNumberInputVM
    {
        public int Id { get; set; }
        public int ApprovalStepId { get; set; }

        [Display(Name = "使用者Id")]
        public int UserId { get; set; }
    }
}
