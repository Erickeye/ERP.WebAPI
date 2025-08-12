using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Library.ViewModels._1000Company
{
    public class UploadCertificate
    {
        public int StaffId { get; set; }
        public string? CertificateName { get; set; }
        public DateTime? CertificateDate { get; set; }
        public int EffectiveDate { get; set; }
        public IFormFile? CertificateFile { get; set; }
    }
    public class EditCertificate
    {
        public int Id { get; set; }
        public string? CertificateName {  get; set; }
        public DateTime? CertificateDate { get; set; }
        public int EffectiveDate { get; set; }
    }
}
