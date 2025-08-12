﻿using ERP.Library.Enums._1000Company;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Library.ViewModels._1000Company
{
    public class DayOffListVM
    {
        [Display(Name = "請假人")]
        public string LeaveTaker { get; set; } = null!;

        [Display(Name = "申請日期")]
        public DateTime? ApplicationDate { get; set; }

        [Display(Name = "代理人")]
        public string? Proxy { get; set; }

        [Display(Name = "假別")]
        public LeaveType LeaveType { get; set; }

        [Display(Name = "事由")]
        public string? Reason { get; set; }

        [Display(Name = "開始日期")]
        public DateTime BeginDate { get; set; }

        [Display(Name = "結束日期")]
        public DateTime EndDate { get; set; }

        [Display(Name = "簽核主管")]
        public string? Authorizer { get; set; }
    }
    public class DayOffInputVM
    {
        public int? Id { get; set; }

        [Display(Name = "申請日期")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? ApplicationDate { get; set; }

        [Required(ErrorMessage = "必填欄位")]
        [Display(Name = "請假人")]
        public int? LeaveTaker { get; set; }

        [Display(Name = "申請者")]
        public int? Applicant { get; set; }

        [Display(Name = "代理人")]
        public int? Proxy { get; set; }

        [Required(ErrorMessage = "必填欄位")]
        [Display(Name = "假別")]
        public LeaveType LeaveType { get; set; }

        [Display(Name = "事由")]
        [StringLength(64, ErrorMessage = "長度不可超過 64 個字元")]
        public string? Reason { get; set; }

        [Display(Name = "代理人簽章")]
        [StringLength(128, ErrorMessage = "長度不可超過 128 個字元")]
        public string? ProxySignature { get; set; }

        [Required(ErrorMessage = "必填欄位")]
        [Display(Name = "開始日期")]
        [DataType(DataType.DateTime)]
        public DateTime? BeginDate { get; set; }

        [Required(ErrorMessage = "必填欄位")]
        [Display(Name = "結束日期")]
        [DataType(DataType.DateTime)]
        public DateTime? EndDate { get; set; }

    }
}
