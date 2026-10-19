using System.ComponentModel.DataAnnotations;
using ERP.Library.Attributes;

namespace ERP.Library.ViewModels._4000Inventory
{
    /// <summary>
    /// 查詢-領料單搜尋
    /// </summary>
    public class MaterialIssueSearchVM : SearchModel, IDownloadFileModel
    {
        [Display(Name = "領料單號")]
        [SearchField("No", SearchCompare.Contains)]
        public string? No { get; set; }

        [Display(Name = "領料用途識別碼")]
        [SearchField("PurposeId", SearchCompare.Equal)]
        public int? PurposeId { get; set; }

        [Display(Name = "領料人姓名")]
        [SearchField("ReceiverStaff.ChineseName", SearchCompare.Contains)]
        public string? ReceiverName { get; set; }

        [Display(Name = "庫位識別碼")]
        [SearchField("LocationId", SearchCompare.Equal)]
        public int? LocationId { get; set; }

        [Display(Name = "客戶名稱")]
        [SearchField("Customer.Name", SearchCompare.Contains)]
        public string? CustomerName { get; set; }

        [Display(Name = "專案名稱")]
        [SearchField("ProjectName", SearchCompare.Contains)]
        public string? ProjectName { get; set; }

        [Display(Name = "領料單狀態(0草稿;1簽核中;2已核准;3已退回;4已作廢)")]
        [SearchField("Status", SearchCompare.Equal)]
        public int? Status { get; set; }

        [Display(Name = "領料日期開始")]
        [SearchField("IssueDate", SearchCompare.GreaterThanOrEqual)]
        public DateTime? IssueDateStart { get; set; }

        [Display(Name = "領料日期結束")]
        [SearchField("IssueDate", SearchCompare.LessThanOrEqual)]
        public DateTime? IssueDateEnd { get; set; }

        /// <summary>
        /// 下載格式
        /// </summary>
        [Display(Name = "下載格式(Pdf:13;Xlsx:6;預設為Xlsx)")]
        public int SaveFormat { get; set; } = 6;
    }

    /// <summary>
    /// 查詢-領料單列表／明細
    /// </summary>
    public class MaterialIssueVM
    {
        [Display(Name = "識別碼")]
        public int Id { get; set; }

        [Display(Name = "領料單號")]
        public string? No { get; set; }

        [Display(Name = "領料日期")]
        public DateTime IssueDate { get; set; }

        [Display(Name = "領料用途識別碼")]
        public int PurposeId { get; set; }

        [Display(Name = "領料用途")]
        public string? PurposeName { get; set; }

        [Display(Name = "庫位識別碼")]
        public int LocationId { get; set; }

        [Display(Name = "庫位")]
        public string? LocationName { get; set; }

        [Display(Name = "領料人識別碼")]
        public int ReceiverStaffId { get; set; }

        [Display(Name = "領料人")]
        public string? ReceiverName { get; set; }

        [Display(Name = "客戶識別碼")]
        public int? CustomerId { get; set; }

        [Display(Name = "客戶名稱")]
        public string? CustomerName { get; set; }

        [Display(Name = "專案名稱")]
        public string? ProjectName { get; set; }

        [Display(Name = "備註")]
        public string? Note { get; set; }

        [Display(Name = "狀態")]
        public int Status { get; set; }

        [Display(Name = "狀態顯示")]
        public string? StatusDisplay { get; set; }

        [Display(Name = "建單時間")]
        public DateTime CreateTime { get; set; }

        [Display(Name = "建單人識別碼")]
        public int CreateUserId { get; set; }

        [Display(Name = "建單人")]
        public string? CreateUserName { get; set; }

        public List<MaterialIssueItemVM> Items { get; set; }
            = new List<MaterialIssueItemVM>();
    }

    /// <summary>
    /// 領料單明細
    /// </summary>
    public class MaterialIssueItemVM
    {
        [Display(Name = "識別碼")]
        public int Id { get; set; }

        [Display(Name = "庫存項目識別碼")]
        public int InventoryId { get; set; }

        [Display(Name = "供應商名稱")]
        public string? SupplierName { get; set; }

        [Display(Name = "編號")]
        public string? No { get; set; }

        [Display(Name = "品名")]
        public string? Name { get; set; }

        [Display(Name = "單位")]
        public string? Unit { get; set; }

        [Display(Name = "領料數量")]
        public decimal Quantity { get; set; }

        [Display(Name = "目前庫存數量")]
        public decimal? InventoryQuantity { get; set; }
    }

    /// <summary>
    /// 新增／修改-領料單輸入
    /// </summary>
    public class MaterialIssueAddVM
    {
        [Display(Name = "識別碼")]
        public int Id { get; set; }

        [Display(Name = "領料日期")]
        [Required(ErrorMessage = "{0}-必填欄位")]
        public DateTime IssueDate { get; set; } = DateTime.Today;

        [Display(Name = "領料用途識別碼")]
        [Required(ErrorMessage = "{0}-必填欄位")]
        public int PurposeId { get; set; }

        [Display(Name = "庫位識別碼")]
        [Required(ErrorMessage = "{0}-必填欄位")]
        public int LocationId { get; set; }

        [Display(Name = "領料人識別碼")]
        [Required(ErrorMessage = "{0}-必填欄位")]
        public int ReceiverStaffId { get; set; }

        [Display(Name = "客戶識別碼")]
        public int? CustomerId { get; set; }

        [Display(Name = "專案名稱")]
        [StringLength(128, ErrorMessage = "{0}-長度不可超過 128 個字元")]
        public string? ProjectName { get; set; }

        [Display(Name = "備註")]
        [StringLength(1024, ErrorMessage = "{0}-長度不可超過 1024 個字元")]
        public string? Note { get; set; }

        public List<MaterialIssueAddItemVM> Items { get; set; }
            = new List<MaterialIssueAddItemVM>();
    }

    /// <summary>
    /// 新增／修改-領料單明細輸入
    /// </summary>
    public class MaterialIssueAddItemVM
    {
        [Display(Name = "庫存項目識別碼")]
        [Required(ErrorMessage = "{0}-必填欄位")]
        public int InventoryId { get; set; }

        [Display(Name = "領料數量")]
        [Required(ErrorMessage = "{0}-必填欄位")]
        public decimal Quantity { get; set; }
    }

    /// <summary>
    /// 新增／修改領料單的結果（含庫存不足警告）
    /// </summary>
    public class MaterialIssueSaveResultVM
    {
        [Display(Name = "識別碼")]
        public int Id { get; set; }

        [Display(Name = "領料單號")]
        public string No { get; set; } = null!;

        [Display(Name = "警告訊息(例如庫存不足，仍可儲存草稿)")]
        public List<string> Warnings { get; set; } = new List<string>();
    }

    /// <summary>
    /// 匯出-領料單列表
    /// </summary>
    public class MaterialIssueExportItemVM
    {
        [Display(Name = "領料單號")]
        public string? No { get; set; }

        [Display(Name = "領料日期")]
        public string? IssueDate { get; set; }

        [Display(Name = "領料用途")]
        public string? PurposeName { get; set; }

        [Display(Name = "庫位")]
        public string? LocationName { get; set; }

        [Display(Name = "領料人")]
        public string? ReceiverName { get; set; }

        [Display(Name = "客戶名稱")]
        public string? CustomerName { get; set; }

        [Display(Name = "專案名稱")]
        public string? ProjectName { get; set; }

        [Display(Name = "狀態")]
        public string? StatusDisplay { get; set; }

        [Display(Name = "備註")]
        public string? Note { get; set; }
    }
}
