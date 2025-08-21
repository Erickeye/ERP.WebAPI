using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Library.Enums
{
    public enum ErrorCodeType
    {
        [Display(Name = "無錯誤", Description = "無錯誤")]
        None = 0,

        [Display(Name = "未認證", Description = "未認證")]
        Unauthorized = 1001,

        [Display(Name = "權限不足", Description = "權限不足")]
        PermissionDenied = 1003,

        [Display(Name = "無效的請求", Description = "無效的請求")]
        InvalidRequest = 1007,

        [Display(Name = "無效的權杖", Description = "無效的權杖")]
        InvalidToken = 1008,

        [Display(Name = "不支援的驗證方法", Description = "不支援的驗證方法")]
        UnsupportedAuthenticateType = 1009,

        [Display(Name = "權杖已到期", Description = "權杖已到期")]
        TokenExpiredOrInvalid = 1010,

        [Display(Name = "密碼未設定", Description = "密碼未設定")]
        PasswordUnassigned = 1021,

        [Display(Name = "密碼強度不足", Description = "密碼強度不足")]
        PasswordStrengthInsufficient = 1022,

        [Display(Name = "密碼與前面幾次相同", Description = "密碼與前面幾次相同")]
        PasswordConflicted = 1023,

        [Display(Name = "密碼已過期", Description = "密碼已過期")]
        PasswordExpired = 1024,

        [Display(Name = "密碼已達變更上限次數", Description = "密碼已達變更上限次數")]
        PasswordChangeLimitReached = 1025,

        [Display(Name = "使用者名稱或密碼錯誤", Description = "使用者名稱或密碼錯誤")]
        IncorrectUsernameOrPassword = 1026,

        [Display(Name = "找不到使用者", Description = "找不到使用者")]
        UserNotFound = 1031,

        [Display(Name = "使用者已鎖定", Description = "使用者已鎖定")]
        UserLocked = 1032,

        [Display(Name = "使用者已停用", Description = "使用者已停用")]
        UserDisabled = 1033,

        [Display(Name = "使用者已存在", Description = "使用者已存在")]
        UserAlreadyExists = 1034,

        [Display(Name = "使用者權限範圍已存在", Description = "使用者權限範圍已存在")]
        UserScopeAlreadyExists = 1035,

        [Display(Name = "使用者群組已存在", Description = "使用者群組已存在")]
        UserGroupAlreadyExists = 1036,

        [Display(Name = "使用者角色已存在", Description = "使用者角色已存在")]
        UserRoleAlreadyExists = 1037,

        [Display(Name = "使用者政策不存在", Description = "使用者政策不存在")]
        UserPolicyNotFound = 1038,

        [Display(Name = "使用者政策已存在", Description = "使用者政策已存在")]
        UserPolicyAlreadyExists = 1039,

        [Display(Name = "使用者郵箱已驗證", Description = "使用者郵箱已驗證")]
        UserEmailAlreadyConfirmed = 1040,

        [Display(Name = "找不到使用者名稱或郵箱", Description = "找不到使用者名稱或郵箱")]
        UserNameOrEmailNotFound = 1041,

        [Display(Name = "群組不存在", Description = "群組不存在")]
        GroupNotFound = 1051,

        [Display(Name = "群組已存在", Description = "群組已存在")]
        GroupAlreadyExists = 1052,

        [Display(Name = "群組權限範圍不存在", Description = "群組權限範圍不存在")]
        GroupScopeNotFound = 1053,

        [Display(Name = "群組權限範圍已存在", Description = "群組權限範圍已存在")]
        GroupScopeAlreadyExists = 1054,

        [Display(Name = "群組類型不存在", Description = "群組類型不存在")]
        GroupTypeNotFound = 1055,

        [Display(Name = "群組類型已存在", Description = "群組類型已存在")]
        GroupTypeAlreadyExists = 1056,

        [Display(Name = "群組政策不存在", Description = "群組政策不存在")]
        GroupPolicyNotFound = 1057,

        [Display(Name = "群組政策已存在", Description = "群組政策已存在")]
        GroupPolicyAlreadyExists = 1058,

        [Display(Name = "群組含關聯資料", Description = "群組含關聯資料")]
        GroupContainRelatedData = 1059,

        [Display(Name = "角色不存在", Description = "角色不存在")]
        RoleNotFound = 1061,

        [Display(Name = "角色已存在", Description = "角色已存在")]
        RoleAlreadyExists = 1062,

        [Display(Name = "角色政策不存在", Description = "角色政策不存在")]
        RolePolicyNotFound = 1063,

        [Display(Name = "角色政策已存在", Description = "角色政策已存在")]
        RolePolicyAlreadyExists = 1064,

        [Display(Name = "角色權限範圍已存在", Description = "角色權限範圍已存在")]
        RoleScopeAlreadyExists = 1065,

        [Display(Name = "屬性不存在", Description = "屬性不存在")]
        AttributeNotFound = 1071,

        [Display(Name = "權限範圍不存在", Description = "權限範圍不存在")]
        ScopeNotFound = 1081,

        [Display(Name = "權限範圍已存在", Description = "權限範圍已存在")]
        ScopeAlreadyExists = 1082,

        [Display(Name = "權限範圍政策已存在", Description = "權限範圍政策已存在")]
        ScopePolicyAlreadyExists = 1083,

        [Display(Name = "操作不存在", Description = "操作不存在")]
        OperationNotFound = 1091,

        [Display(Name = "操作權限範圍已存在", Description = "操作權限範圍已存在")]
        OperationScopeAlreadyExists = 1092,

        [Display(Name = "政策不存在", Description = "政策不存在")]
        PolicyNotFound = 1101,

        [Display(Name = "稽核紀錄不存在", Description = "稽核紀錄不存在")]
        AuditLogNotFound = 1201,

        /// <summary>
        /// 例外錯誤
        /// </summary>
        [Display(Name = "例外錯誤", Description = "例外錯誤")]
        Exception = 3000,
        /// <summary>
        /// 查無欲編修的項目
        /// </summary>
        [Display(Name = "查無欲編修的項目", Description = "查無欲編修的項目")]
        ModifyItemNotFound = 3001,
        /// <summary>
        /// 權杖到期
        /// </summary>
        [Display(Name = "權杖到期", Description = "權杖到期")]
        TokenExpire = 3002,

        /// <summary>
        /// 欄位值無效
        /// </summary>
        [Display(Name = "欄位值無效", Description = "欄位值無效")]
        FieldValueIsInvalid = 4000,

        /// <summary>
        /// 找不到該資料
        /// </summary>
        [Display(Name = "找不到該資料", Description = "找不到該資料")]
        NotFoundData = 4001,

        /// <summary>
        /// 必須為有效單位
        /// </summary>
        [Display(Name = "必須為有效單位", Description = "必須為有效單位")]
        HasToBeValidOrganization = 4002,

        /// <summary>
        /// 此單位的資料已存在
        /// </summary>
        [Display(Name = "此單位的資料已存在", Description = "此單位的資料已存在")]
        OrganizationDataExists = 4003,
        /// <summary>
        /// 查無單位
        /// </summary>
        [Display(Name = "查無單位", Description = "查無單位")]
        UnitNotFound = 4005,

        /// <summary>
        /// 必填欄位
        /// </summary>
        [Display(Name = "必填欄位", Description = "必填欄位")]
        FieldMustBeFilled = 4006,

        /// <summary>
        /// 查無資料
        /// </summary>
        [Display(Name = "查無資料", Description = "查無資料")]
        DataEmpty = 4007,

        /// <summary>
        /// 沒有上傳圖片或圖片無效
        /// </summary>
        [Display(Name = "沒有上傳圖片或圖片無效", Description = "沒有上傳圖片或圖片無效")]
        ImgNotFound = 4008,

        /// <summary>
        /// 圖片大小不能超過 2MB
        /// </summary>
        [Display(Name = "圖片大小不能超過 2MB", Description = "圖片大小不能超過 2MB")]
        ImgOver2MB = 4009,

        /// <summary>
        /// 圖片格式不支援，只接受 JPEG、PNG 和 GIF 格式
        /// </summary>
        [Display(Name = "圖片格式不支援，只接受 JPEG、PNG 和 GIF 格式", Description = "圖片格式不支援，只接受 JPEG、PNG 和 GIF 格式")]
        ImgNotSupport = 4010,

        [Display(Name = "資料不完整", Description = "資料不完整")]
        IncompleteInfo= 4011,

        [Display(Name = "不支援的簽核資料類型", Description = "不支援的簽核資料類型")]
        InvalidApproval = 4012,

        [Display(Name = "該作業已簽核", Description = "該作業已簽核")]
        IsAlreadyApproval = 4013,

        [Display(Name = "非該使用者作業", Description = "非該使用者作業")]
        InvalidUserOperation = 4014,

        [Display(Name = "該簽核作業已存在", Description = "該簽核作業已存在")]
        ApprovalExists = 4015,

        [Display(Name = "指定的紀錄已異動", Description = "指定的紀錄已異動")]
        SpecificRecordChanged = 9501,

        [Display(Name = "動態規約中包含語法錯誤", Description = "動態規約中包含語法錯誤")]
        SyntaxErrorInDynamicSpecification = 9502,

        [Display(Name = "Redis 連線失敗", Description = "Redis 連線失敗")]
        RedisConnectionFailed = 9801,

        [Display(Name = "Redis 逾時", Description = "Redis 逾時")]
        RedisTimeout = 9802,

        [Display(Name = "服務配額不足", Description = "服務配額不足")]
        ServiceQuotaExceeded = 9810,

        [Display(Name = "服務不可用", Description = "服務不可用")]
        ServiceUnavailable = 9811,

        [Display(Name = "端點不可用", Description = "端點不可用")]
        EndpointUnavailable = 9812,

        [Display(Name = "不支援的參數", Description = "不支援的參數")]
        UnsupportedParameter = 9813,

        [Display(Name = "訊息寄送失敗", Description = "訊息寄送失敗")]
        MessageDeliveryFailed = 9814,

        [Display(Name = "伺服器內部系統錯誤", Description = "伺服器內部系統錯誤")]
        InternalServerError = 9995,

        [Display(Name = "跟隨 HTTP 狀態代碼", Description = "跟隨 HTTP 狀態代碼")]
        ReferToHttpStatusCode = 9996,

        [Display(Name = "開發時期的例外錯誤", Description = "開發時期的例外錯誤")]
        DevelopmentException = 9997,

        [Display(Name = "未實作或未定義", Description = "未實作或未定義")]
        UnimplementedOrUndefined = 9998,
        [Display(Name = "簽核引擎錯誤", Description = "簽核引擎錯誤")]
        FlowingEngineError = 10000,


    }
}
