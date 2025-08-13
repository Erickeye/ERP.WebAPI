using System.ComponentModel.DataAnnotations;

namespace ERP.Models.AMS
{
    public class Role
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "必填欄位")]
        [StringLength(32, ErrorMessage = "長度不可超過 32 個字元")]
        public string RoleName { get; set; } = null!;
        public int? PermissionLevel { get; set; } = 0;
        public int? ApprovalLevel { get; set; } = 30;
        public int? QuotationLevel { get; set; } = 30;
        public int? ProcurementLevel { get; set; } = 30;
    }
}
