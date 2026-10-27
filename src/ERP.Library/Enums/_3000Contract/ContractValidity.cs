using System.ComponentModel.DataAnnotations;

namespace ERP.Library.Enums._3000Contract
{
    /// <summary>
    /// 已核准合約單依日期計算的有效性（不儲存）：今天落在起訖日內（含兩天）為生效中，晚於迄日為已到期；
    /// 尚未到起日、或不是已核准的合約單沒有有效性。
    /// </summary>
    public enum ContractValidity
    {
        [Display(Name = "生效中")]
        生效中 = 1,

        [Display(Name = "已到期")]
        已到期 = 2,
    }
}
