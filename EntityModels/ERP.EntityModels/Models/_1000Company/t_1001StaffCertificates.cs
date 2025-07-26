using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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

        [Display(Name = "員工Id")]
        public int? StaffId { get; set; }

        [Display(Name = "證書圖片")]
        public byte[]? Certificate { get; set; }

        [Display(Name = "證書名稱")]
        [StringLength(128)]
        public string? CertificateName { get; set; }

        [Display(Name = "證書取得日")]
        [DataType(DataType.Date)]
        public DateTime? CertificateDate { get; set; }

        [Display(Name = "有效期限（月）")]
        public int? EffectiveDate { get; set; }

        [Display(Name = "是否提醒")]
        public bool? IsNotify { get; set; }

        [Display(Name = "提醒日期")]
        [DataType(DataType.Date)]
        public DateTime? NotifyDate { get; set; }
    }
}
