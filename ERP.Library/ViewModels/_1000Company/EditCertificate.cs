using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Library.ViewModels._1000Company
{
    public class EditCertificate
    {
        public int Id { get; set; }
        public string? CertificateName {  get; set; }
        public DateTime? CertificateDate { get; set; }
        public int EffectiveDate { get; set; }
    }
}
