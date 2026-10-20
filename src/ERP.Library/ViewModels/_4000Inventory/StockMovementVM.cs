using System.ComponentModel.DataAnnotations;
using ERP.Library.Attributes;

namespace ERP.Library.ViewModels._4000Inventory
{
    /// <summary>
    /// 查詢-庫存異動紀錄搜尋
    /// </summary>
    public class StockMovementSearchVM : SearchModel
    {
        [Display(Name = "庫存項目識別碼")]
        [SearchField("InventoryId", SearchCompare.Equal)]
        public int? InventoryId { get; set; }

        [Display(Name = "異動類型(1進貨入庫;2進貨作廢;3領料出庫;4領料作廢)")]
        [SearchField("MovementType", SearchCompare.Equal)]
        public int? MovementType { get; set; }

        [Display(Name = "來源單號")]
        [SearchField("SourceNo", SearchCompare.Contains)]
        public string? SourceNo { get; set; }

        [Display(Name = "異動日期開始")]
        [SearchField("MovementTime", SearchCompare.GreaterThanOrEqual)]
        public DateTime? MovementTimeStart { get; set; }

        [Display(Name = "異動日期結束")]
        [SearchField("MovementTime", SearchCompare.LessThanOrEqual)]
        public DateTime? MovementTimeEnd { get; set; }
    }

    /// <summary>
    /// 查詢-庫存異動紀錄列表
    /// </summary>
    public class StockMovementVM
    {
        [Display(Name = "識別碼")]
        public int Id { get; set; }

        [Display(Name = "庫存項目識別碼")]
        public int InventoryId { get; set; }

        [Display(Name = "編號")]
        public string? InventoryNo { get; set; }

        [Display(Name = "品名")]
        public string? InventoryName { get; set; }

        [Display(Name = "庫位")]
        public string? LocationName { get; set; }

        [Display(Name = "異動類型")]
        public int MovementType { get; set; }

        [Display(Name = "異動類型顯示")]
        public string? MovementTypeDisplay { get; set; }

        [Display(Name = "來源單據類型")]
        public int SourceTableType { get; set; }

        [Display(Name = "來源單據類型顯示")]
        public string? SourceTableName { get; set; }

        [Display(Name = "來源單號")]
        public string? SourceNo { get; set; }

        [Display(Name = "異動數量(增加為正、減少為負)")]
        public decimal Quantity { get; set; }

        [Display(Name = "異動前數量")]
        public decimal BeforeQuantity { get; set; }

        [Display(Name = "異動後數量")]
        public decimal AfterQuantity { get; set; }

        [Display(Name = "操作人識別碼")]
        public int OperatorUserId { get; set; }

        [Display(Name = "操作人")]
        public string? OperatorName { get; set; }

        [Display(Name = "異動時間")]
        public DateTime MovementTime { get; set; }
    }
}
