using System.ComponentModel.DataAnnotations;

namespace ERP.Library.Enums._3000Contract
{
    /// <summary>
    /// 合約類型：決定合約對象是供應商還是客戶
    /// </summary>
    public enum ContractType
    {
        [Display(Name = "供應商合約")]
        供應商合約 = 0,

        [Display(Name = "客戶合約")]
        客戶合約 = 1,
    }
}
