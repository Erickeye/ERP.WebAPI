using ERP.Library.Enums;
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
        public int Id { get; set; }

        [Display(Name = "使用者Id")]
        public int UserId { get; set; }

        [StringLength(32)]
        [Display(Name = "使用者帳號")]
        public string Account { get; set; } = null!;

        [StringLength(64)]
        [Display(Name = "登入IP")]
        public string IpAddress { get; set; } = null!;

        [Display(Name = "時間")]
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd HH:mm:ss}")]
        public System.DateTime CrateDate { get; set; }

        [StringLength(512)]
        [Display(Name = "操作位置")]
        public string? Location { get; set; }

        [Display(Name = "操作類型")]
        public OperationActionType ActionType { get; set; }

        [StringLength(16)]
        [Display(Name = "關鍵詞")]
        public string? KeyId { get; set; }

        [StringLength(512)]
        [Display(Name = "說明")]
        public string? Memo { get; set; }
    }
}
