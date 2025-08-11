using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.EntityModels.Models._1000Company
{
    public class t_1700LoginLog
    {
        [Key]
        [Display(Name = "流水號")]
        public int Id { get; set; }

        [Display(Name = "時間")]
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd HH:mm:ss}")]
        public System.DateTime CrateDate { get; set; }

        [Display(Name = "使用者Id")]
        public int UserId { get; set; }

        [Display(Name = "使用者帳號")]
        public string? Account { get; set; }

        [Display(Name = "操作類型")]
        public int Method { get; set; }

        [Display(Name = "登入IP")]
        public string IpAddress { get; set; } = null!;
    }
}
