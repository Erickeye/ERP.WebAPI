using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Library.Enums
{
    public enum OperationActionType
    {
        /// <summary>
        /// None
        /// </summary>    
        [Display(Name = "None", Description = "None")]
        None = 0,
        /// <summary>
        /// 檢視
        /// </summary>
        [Display(Name = "檢視", Description = "檢視")]
        View,
        /// <summary>
        /// 新增
        /// </summary>
        [Display(Name = "新增", Description = "新增")]
        Create,
        /// <summary>
        /// 編輯
        /// </summary>
        [Display(Name = "編輯", Description = "編輯")]
        Edit,
        /// <summary>
        /// 刪除
        /// </summary>
        [Display(Name = "刪除", Description = "刪除")]
        Delete,
        /// <summary>
        /// 匯出
        /// </summary>
        [Display(Name = "匯出", Description = "匯出")]
        Export,

        /// <summary>
        /// 匯入
        /// </summary>
        [Display(Name = "匯入", Description = "匯入")]
        Import,

        /// <summary>
        /// 簽核
        /// </summary>
        [Display(Name = "簽核", Description = "簽核")]
        Approval,

        /// <summary>
        /// 撤銷簽核
        /// </summary>
        [Display(Name = "撤銷簽核", Description = "撤銷簽核")]
        RevokeApproval,

        /// <summary>
        /// 重設密碼
        /// </summary>
        [Display(Name = "重設密碼", Description = "重設密碼")]
        ResetSw,
    }
}
