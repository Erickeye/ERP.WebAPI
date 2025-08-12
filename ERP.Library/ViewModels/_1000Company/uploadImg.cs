using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Library.ViewModels._1000Company
{
    public class UploadImg
    {
        public int StaffId {  get; set; }
        public IFormFile? image { get; set; }
    }
}
