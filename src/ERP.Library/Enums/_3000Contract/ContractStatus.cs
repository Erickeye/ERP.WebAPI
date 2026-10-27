using System.ComponentModel.DataAnnotations;

namespace ERP.Library.Enums._3000Contract
{
    /// <summary>
    /// 合約單狀態（「生效中」「已到期」不是狀態，由已核准且日期計算）
    /// </summary>
    public enum ContractStatus
    {
        [Display(Name = "草稿")]
        草稿 = 0,

        [Display(Name = "簽核中")]
        簽核中 = 1,

        [Display(Name = "已核准")]
        已核准 = 2,

        /// <summary>
        /// 被拒絕或被撤銷後的狀態，可修改後重新送簽
        /// </summary>
        [Display(Name = "已退回")]
        已退回 = 3,

        /// <summary>
        /// 終態：已核准的合約單被終止，不可重送
        /// </summary>
        [Display(Name = "已作廢")]
        已作廢 = 4,
    }
}
