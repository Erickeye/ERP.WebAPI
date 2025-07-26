using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.EntityModels.Models._1000Company
{
    public class t_1710ActionInfo
    {
        [Key]
        [Display(Name = "流水號")]
        public int f_ActionInfo_UID { get; set; }

        [Display(Name = "使用者帳號")]
        public string? f_ActionInfo_Account { get; set; }

        [Display(Name = "登入IP")]
        public string? f_ActionInfo_IP { get; set; }

        [Display(Name = "時間")]
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd HH:mm:ss}")]
        public System.DateTime? f_ActionInfo_Date { get; set; }

        [Display(Name = "資料表")]
        public string? f_ActionInfo_Controller { get; set; }

        [Display(Name = "動作")]
        public string? f_ActionInfo_Function { get; set; }

        [Display(Name = "關鍵詞")]
        public string? f_ActionInfo_ControllerID { get; set; }

        [Display(Name = "說明")]
        public string? f_ActionInfo_describe { get; set; }
    }
}
