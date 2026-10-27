using System.ComponentModel.DataAnnotations;
using ERP.Library.Attributes;
using ERP.Library.Enums._3000Contract;
using Microsoft.AspNetCore.Http;

namespace ERP.Library.ViewModels._3000Contract
{
    /// <summary>
    /// 查詢-合約單搜尋（分頁與排序；篩選條件可任意組合，且都只在可見範圍內再縮小）
    /// </summary>
    public class ContractSearchVM : SearchModel
    {
        /// <summary>
        /// 同時比對單號與合約編號，所以不用 SearchField 逐欄比對，由 Service 組條件
        /// </summary>
        [Display(Name = "單號或合約編號關鍵字")]
        public string? Keyword { get; set; }

        [Display(Name = "合約類型(0供應商合約;1客戶合約)")]
        [SearchField("Type", SearchCompare.Equal)]
        public int? Type { get; set; }

        /// <summary>
        /// 資料表的供應商、客戶外鍵可為空，SearchField 的 Equal 不支援可空欄位，所以由 Service 組條件
        /// </summary>
        [Display(Name = "供應商識別碼")]
        public int? SupplierId { get; set; }

        [Display(Name = "客戶識別碼")]
        public int? CustomerId { get; set; }

        [Display(Name = "簽約公司識別碼")]
        [SearchField("CompanyId", SearchCompare.Equal)]
        public int? CompanyId { get; set; }

        [Display(Name = "合約單狀態(0草稿;1簽核中;2已核准;3已退回;4已作廢)")]
        [SearchField("Status", SearchCompare.Equal)]
        public int? Status { get; set; }

        /// <summary>
        /// 只看已核准且生效中或已到期的合約單，由 Service 依今天的日期組條件
        /// </summary>
        [Display(Name = "有效性(1生效中;2已到期)")]
        public ContractValidity? Validity { get; set; }

        /// <summary>
        /// 日期區間以「與合約起訖有交集」為準（邊界日算有交集），所以不用 SearchField 逐欄比對，由 Service 組條件
        /// </summary>
        [Display(Name = "合約期間開始(與合約起訖有交集)")]
        public DateTime? PeriodStart { get; set; }

        [Display(Name = "合約期間結束(與合約起訖有交集)")]
        public DateTime? PeriodEnd { get; set; }

        [Display(Name = "只看待我簽核")]
        public bool PendingMyApproval { get; set; }
    }

    /// <summary>
    /// 查詢-合約單列表／單筆
    /// </summary>
    public class ContractVM
    {
        [Display(Name = "識別碼")]
        public int Id { get; set; }

        [Display(Name = "單號")]
        public string? No { get; set; }

        [Display(Name = "合約編號")]
        public string? ContractNo { get; set; }

        [Display(Name = "合約類型(0供應商合約;1客戶合約)")]
        public int Type { get; set; }

        [Display(Name = "合約類型顯示")]
        public string? TypeDisplay { get; set; }

        [Display(Name = "供應商識別碼(供應商合約才有)")]
        public int? SupplierId { get; set; }

        [Display(Name = "客戶識別碼(客戶合約才有)")]
        public int? CustomerId { get; set; }

        /// <summary>
        /// 合約對象名稱：供應商合約為供應商名稱，客戶合約為客戶名稱
        /// </summary>
        [Display(Name = "合約對象")]
        public string? PartyName { get; set; }

        [Display(Name = "對象供應商是否啟用中(客戶合約為 null；已停用的供應商，畫面標示已停用)")]
        public bool? SupplierIsActive { get; set; }

        [Display(Name = "簽約公司識別碼")]
        public int CompanyId { get; set; }

        [Display(Name = "簽約公司(公司簡稱)")]
        public string? CompanyName { get; set; }

        [Display(Name = "簽約公司是否啟用中(已停用的公司，畫面標示已停用)")]
        public bool CompanyIsActive { get; set; }

        [Display(Name = "合約起日")]
        public DateTime StartDate { get; set; }

        [Display(Name = "合約迄日")]
        public DateTime EndDate { get; set; }

        [Display(Name = "合約總金額")]
        public decimal Amount { get; set; }

        [Display(Name = "備註")]
        public string? Remark { get; set; }

        [Display(Name = "是否已上傳合約掃描檔")]
        public bool HasAttachment { get; set; }

        [Display(Name = "合約單狀態(0草稿;1簽核中;2已核准;3已退回;4已作廢)")]
        public int Status { get; set; }

        [Display(Name = "合約單狀態顯示")]
        public string? StatusDisplay { get; set; }

        /// <summary>
        /// 已核准且今天介於起訖日（含兩天）為 1 生效中，晚於迄日為 2 已到期，其餘為 null；只在輸出時計算，不存資料庫
        /// </summary>
        [Display(Name = "有效性(1生效中;2已到期;null表示不適用)")]
        public int? Validity { get; set; }

        [Display(Name = "有效性顯示(生效中、已到期)")]
        public string? ValidityDisplay { get; set; }

        /// <summary>
        /// 是否輪到目前使用者簽核（簽核中且在引擎待簽核查詢內），由後端判斷，前端據此顯示核准與拒絕
        /// </summary>
        [Display(Name = "輪到目前使用者簽核")]
        public bool CanApprove { get; set; }

        /// <summary>
        /// 目前使用者能否編輯、上傳掃描檔與送簽：草稿或已退回，且有「合約單編輯」權限
        /// </summary>
        [Display(Name = "目前使用者可編輯")]
        public bool CanEdit { get; set; }

        /// <summary>
        /// 目前使用者能否刪除：草稿，且有「合約單刪除」權限
        /// </summary>
        [Display(Name = "目前使用者可刪除")]
        public bool CanDelete { get; set; }

        /// <summary>
        /// 目前使用者能否撤銷：簽核中、有「合約單編輯」權限，且是最近一輪的送簽人（引擎只允許送簽人撤銷）
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

        [Display(Name = "最後修改時間")]
        public DateTime UpdateTime { get; set; }
    }

    /// <summary>
    /// 新增／修改-合約單輸入
    /// </summary>
    public class ContractAddVM
    {
        [Display(Name = "識別碼(新增時不需傳)")]
        public int Id { get; set; }

        [Display(Name = "合約類型(0供應商合約;1客戶合約)")]
        [Required(ErrorMessage = "{0}-必填欄位")]
        public ContractType? Type { get; set; }

        [Display(Name = "供應商識別碼(供應商合約必填，客戶合約不可填)")]
        public int? SupplierId { get; set; }

        [Display(Name = "客戶識別碼(客戶合約必填，供應商合約不可填)")]
        public int? CustomerId { get; set; }

        [Display(Name = "簽約公司識別碼")]
        [Required(ErrorMessage = "{0}-必填欄位")]
        public int? CompanyId { get; set; }

        [Display(Name = "合約編號")]
        [Required(ErrorMessage = "{0}-必填欄位")]
        [StringLength(64, ErrorMessage = "{0}-長度不可超過 64 個字元")]
        public string? ContractNo { get; set; }

        [Display(Name = "合約起日")]
        [Required(ErrorMessage = "{0}-必填欄位")]
        public DateTime? StartDate { get; set; }

        [Display(Name = "合約迄日")]
        [Required(ErrorMessage = "{0}-必填欄位")]
        public DateTime? EndDate { get; set; }

        [Display(Name = "合約總金額(大於 0，最多兩位小數)")]
        [Required(ErrorMessage = "{0}-必填欄位")]
        public decimal? Amount { get; set; }

        [Display(Name = "備註")]
        [StringLength(1024, ErrorMessage = "{0}-長度不可超過 1024 個字元")]
        public string? Remark { get; set; }
    }

    /// <summary>
    /// 上傳／更換-合約掃描檔輸入（multipart/form-data）
    /// </summary>
    public class ContractAttachmentVM
    {
        [Display(Name = "合約單流水號")]
        [Required(ErrorMessage = "{0}-必填欄位")]
        public int? Id { get; set; }

        [Display(Name = "合約掃描檔(僅限 PDF，不超過 10 MB)")]
        [Required(ErrorMessage = "{0}-必填欄位")]
        public IFormFile? File { get; set; }
    }

    /// <summary>
    /// 下載-合約掃描檔內容（由 Controller 轉成檔案回應）
    /// </summary>
    public class ContractAttachmentFileVM
    {
        public Stream Content { get; set; } = null!;

        public string FileName { get; set; } = null!;
    }

    /// <summary>
    /// 新增／修改合約單的結果
    /// </summary>
    public class ContractSaveResultVM
    {
        [Display(Name = "識別碼")]
        public int Id { get; set; }

        [Display(Name = "單號")]
        public string No { get; set; } = null!;
    }
}
