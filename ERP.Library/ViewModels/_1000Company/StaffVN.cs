using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Library.ViewModels._1000Company
{
    public class StaffListVM
    {
        public string? StaffUid { get; set; }
        public string? Name { get; set; }
        public string? IdCard { get; set; }
        public string? Gender { get; set; }
        public System.DateTime? Bitrthday { get; set; }
        public string? ContactPhone { get; set; }
        public string? LineId { get; set; }
        public string? Email { get; set; }
        public string? ContactAddress { get; set; }
    }

    public class UploadImg
    {
        public int StaffId { get; set; }
        public IFormFile? image { get; set; }
    }
}
