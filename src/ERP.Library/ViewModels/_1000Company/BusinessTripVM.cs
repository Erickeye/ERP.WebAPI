using System.ComponentModel.DataAnnotations;
using ERP.Library.Attributes;

namespace ERP.Library.ViewModels._1000Company
{
    /// <summary>
    /// 查詢-出差單搜尋（分頁與排序；篩選條件可任意組合，且都只在可見範圍內再縮小）
    /// </summary>
    public class BusinessTripSearchVM : SearchModel
    {
        [Display(Name = "出差單號關鍵字")]
        [SearchField("No", SearchCompare.Contains)]
        public string? No { get; set; }

        [Display(Name = "出差人識別碼")]
        [SearchField("TravelerStaffId", SearchCompare.Equal)]
        public int? TravelerStaffId { get; set; }

        [Display(Name = "單據公司識別碼")]
        [SearchField("CompanyId", SearchCompare.Equal)]
        public int? CompanyId { get; set; }

        [Display(Name = "出差單狀態(0草稿;1簽核中;2已核准;3已退回;4已作廢)")]
        [SearchField("Status", SearchCompare.Equal)]
        public int? Status { get; set; }

        /// <summary>
        /// 日期區間以「與出差起訖有交集」為準（邊界日算有交集），所以不用 SearchField 逐欄比對，由 Service 組條件
        /// </summary>
        [Display(Name = "出差期間開始(與出差起訖有交集)")]
        public DateTime? PeriodStart { get; set; }

        [Display(Name = "出差期間結束(與出差起訖有交集)")]
        public DateTime? PeriodEnd { get; set; }

        [Display(Name = "只看待我簽核")]
        public bool PendingMyApproval { get; set; }
    }

    /// <summary>
    /// 查詢-出差單列表／單筆
    /// </summary>
    public class BusinessTripVM
    {
        [Display(Name = "識別碼")]
        public int Id { get; set; }

        [Display(Name = "出差單號")]
        public string? No { get; set; }

        [Display(Name = "出差人識別碼")]
        public int TravelerStaffId { get; set; }

        [Display(Name = "出差人")]
        public string? TravelerName { get; set; }

        [Display(Name = "單據公司識別碼")]
        public int CompanyId { get; set; }

        [Display(Name = "單據公司(公司簡稱)")]
        public string? CompanyName { get; set; }

        [Display(Name = "單據公司統一編號")]
        public string? CompanyTaxInvoiceNumber { get; set; }

        [Display(Name = "單據公司是否啟用中(已停用的公司，畫面標示已停用)")]
        public bool CompanyIsActive { get; set; }

        [Display(Name = "出差地點")]
        public string? Destination { get; set; }

        [Display(Name = "出差事由")]
        public string? Reason { get; set; }

        [Display(Name = "出差起日")]
        public DateTime StartDate { get; set; }

        [Display(Name = "出差迄日")]
        public DateTime EndDate { get; set; }

        /// <summary>
        /// 出差天數：日曆天、含起訖兩天、不扣假日；只在輸出時計算，不存資料庫
        /// </summary>
        [Display(Name = "出差天數")]
        public int Days => (EndDate.Date - StartDate.Date).Days + 1;

        [Display(Name = "客戶識別碼")]
        public int? CustomerId { get; set; }

        [Display(Name = "客戶名稱")]
        public string? CustomerName { get; set; }

        [Display(Name = "備註")]
        public string? Note { get; set; }

        [Display(Name = "出差單狀態(0草稿;1簽核中;2已核准;3已退回;4已作廢)")]
        public int Status { get; set; }

        [Display(Name = "出差單狀態顯示")]
        public string? StatusDisplay { get; set; }

        /// <summary>
        /// 是否輪到目前使用者簽核（簽核中且在引擎待簽核查詢內），由後端判斷，前端據此顯示核准與拒絕
        /// </summary>
        [Display(Name = "輪到目前使用者簽核")]
        public bool CanApprove { get; set; }

        /// <summary>
        /// 目前使用者能否編輯與送簽：草稿或已退回，且是建單人或出差人。簽核人檢視他人的已退回單時為 false
        /// </summary>
        [Display(Name = "目前使用者可編輯")]
        public bool CanEdit { get; set; }

        /// <summary>
        /// 目前使用者能否撤銷：簽核中、是建單人或出差人，且是最近一輪的送簽人（引擎只允許送簽人撤銷）
        /// </summary>
        [Display(Name = "目前使用者可撤銷")]
        public bool CanRevoke { get; set; }

        /// <summary>
        /// 目前使用者能否作廢：已核准，且是最近一輪最後一關的簽核人
        /// </summary>
        [Display(Name = "目前使用者可作廢")]
        public bool CanVoid { get; set; }

        [Display(Name = "建單時間")]
        public DateTime CreateTime { get; set; }

        [Display(Name = "建單人識別碼")]
        public int CreateUserId { get; set; }

        [Display(Name = "建單人")]
        public string? CreateUserName { get; set; }
    }

    /// <summary>
    /// 新增／修改-出差單輸入
    /// </summary>
    public class BusinessTripAddVM
    {
        [Display(Name = "識別碼(新增時不需傳)")]
        public int Id { get; set; }

        [Display(Name = "出差人識別碼")]
        [Required(ErrorMessage = "{0}-必填欄位")]
        public int TravelerStaffId { get; set; }

        [Display(Name = "單據公司識別碼")]
        [Required(ErrorMessage = "{0}-必填欄位")]
        public int CompanyId { get; set; }

        [Display(Name = "出差地點")]
        [Required(ErrorMessage = "{0}-必填欄位")]
        [StringLength(128, ErrorMessage = "{0}-長度不可超過 128 個字元")]
        public string? Destination { get; set; }

        [Display(Name = "出差事由")]
        [Required(ErrorMessage = "{0}-必填欄位")]
        [StringLength(256, ErrorMessage = "{0}-長度不可超過 256 個字元")]
        public string? Reason { get; set; }

        [Display(Name = "出差起日")]
        [Required(ErrorMessage = "{0}-必填欄位")]
        public DateTime? StartDate { get; set; }

        [Display(Name = "出差迄日")]
        [Required(ErrorMessage = "{0}-必填欄位")]
        public DateTime? EndDate { get; set; }

        [Display(Name = "客戶識別碼")]
        public int? CustomerId { get; set; }

        [Display(Name = "備註")]
        [StringLength(1024, ErrorMessage = "{0}-長度不可超過 1024 個字元")]
        public string? Note { get; set; }
    }

    /// <summary>
    /// 新增／修改出差單的結果
    /// </summary>
    public class BusinessTripSaveResultVM
    {
        [Display(Name = "識別碼")]
        public int Id { get; set; }

        [Display(Name = "出差單號")]
        public string No { get; set; } = null!;
    }
}
