using System.ComponentModel.DataAnnotations;

namespace ERP.Library.Enums._4000Inventory
{
    /// <summary>
    /// 庫存異動類型
    /// </summary>
    public enum StockMovementType
    {
        [Display(Name = "進貨入庫")]
        進貨入庫 = 1,

        [Display(Name = "進貨作廢")]
        進貨作廢 = 2,

        [Display(Name = "領料出庫")]
        領料出庫 = 3,

        [Display(Name = "領料作廢")]
        領料作廢 = 4,
    }
}
