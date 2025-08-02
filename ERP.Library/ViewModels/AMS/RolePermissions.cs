using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Library.ViewModels.AMS
{
    public class RolePermissions
    {
        [Display(Name = "角色Id")]
        public int RoleId { get; set; }

        [Display(Name = "角色權限內容")]
        public string? RoleContent { get; set; }

        [Display(Name = "權限總等級")]
        public int? Permission { get; set; }

        [Display(Name = "???")]
        public int? Approval { get; set; }

        [Display(Name = "報價單權限")]
        public int? Quotation { get; set; }

        [Display(Name = "採購單權限")]
        public int? Procurement { get; set; }
    }
}
