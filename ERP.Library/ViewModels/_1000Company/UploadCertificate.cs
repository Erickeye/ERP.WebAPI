using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace ERP.Library.ViewModels._1000Company
{
    public class UploadCertificate
    {
        public int StaffId {  get; set; }
        public string? CertificateName { get; set; }
        public DateTime? CertificateDate { get; set; }
        public int EffectiveDate { get; set; }
        public IFormFile? CertificateFile { get; set; }
    }
}
