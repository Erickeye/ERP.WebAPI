using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.EntityModels.Models._1000Company
{
    public class t_1001StaffCertificates
    {
        [Key]
        [Display(Name = "流水號")]
        public int Id { get; set; }

        [Required(ErrorMessage = "必填欄位")]
        [Display(Name = "員工Id")]
        public int StaffId { get; set; }

        [Display(Name = "證書圖片")]
        public byte[]? Certificate { get; set; }

        [Display(Name = "證書名稱")]
        [StringLength(128, ErrorMessage = "長度不可超過 128 個字元")]
        public string? CertificateName { get; set; }

        [Display(Name = "證書取得日")]
        [DataType(DataType.Date)]
        public DateTime? CertificateDate { get; set; }

        [Display(Name = "有效期限（月）")]
        [Range(0, 1200, ErrorMessage = "有效期限需為 0~1200 月之間")]
        public int? EffectiveDate { get; set; }

        [Display(Name = "是否提醒")]
        public bool? IsNotify { get; set; }

        [Display(Name = "提醒日期")]
        [DataType(DataType.Date)]
        public DateTime? NotifyDate { get; set; }

        [ForeignKey(nameof(StaffId))]
        public virtual t_1000Staff? Staff { get; set; }
    }
}
