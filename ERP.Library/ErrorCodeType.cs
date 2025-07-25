using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TLRIMOA.Library.Enums
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
        /// 使用者名稱或密碼或驗證碼錯誤
        /// </summary>
        [Display(Name = "使用者名稱或密碼或驗證碼錯誤", Description = "使用者名稱或密碼或驗證碼錯誤")]
        IncorrectUsernameOrPasswordOrCaptcha = 4001,

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
        /// 請選擇所屬上層
        /// </summary>
        [Display(Name = "請選擇所屬上層", Description = "請選擇所屬上層")]
        UnChooseParentId = 4006,
        /// <summary>
        /// 請輸入名稱
        /// </summary>
        [Display(Name = "請輸入名稱", Description = "請輸入名稱")]
        UnFillName = 4007,
        /// <summary>
        /// 請輸入代碼
        /// </summary>
        [Display(Name = "請輸入代碼", Description = "請輸入代碼")]
        UnFillCode = 4008,
        /// <summary>
        /// 請輸入排序
        /// </summary>
        [Display(Name = "請輸入排序", Description = "請輸入排序")]
        UnFillSort = 4009,
        /// <summary>
        /// 請先移除子部門與人員
        /// </summary>
        [Display(Name = "請先移除子部門與人員", Description = "請先移除子部門與人員")]
        RomoveSubItemsFirst = 4010,
        /// <summary>
        /// 單位名稱含有非法字元
        /// </summary>
        [Display(Name = "單位名稱含有非法字元", Description = "單位名稱含有非法字元")]
        UnitNameIsInvalid = 4011,
        /// <summary>
        /// 查無人員
        /// </summary>
        [Display(Name = "查無人員", Description = "查無人員")]
        MemberNotFound = 4012,
        /// <summary>
        /// 請輸入帳號
        /// </summary>
        [Display(Name = "請輸入帳號", Description = "請輸入帳號")]
        UnFillAC = 4013,
        /// <summary>
        /// 請輸入信箱
        /// </summary>
        [Display(Name = "請輸入信箱", Description = "請輸入信箱")]
        UnFillEmail = 4014,
        /// <summary>
        /// 帳號名稱已被使用
        /// </summary>
        [Display(Name = "帳號名稱已被使用", Description = "帳號名稱已被使用")]
        AcIsUsed = 4015,
        /// <summary>
        /// 身分字號無效
        /// </summary>
        [Display(Name = "身分字號無效", Description = "身分字號無效")]
        IdCardNumberInvalid = 4016,
        /// <summary>
        /// 已存在相同的身分證字號
        /// </summary>
        [Display(Name = "已存在相同的身分證字號", Description = "已存在相同的身分證字號")]
        SameIdCardNumber = 4017,

        /// <summary>
        /// 必須為單位才可刪除
        /// </summary>
        [Display(Name = "必須為單位才可刪除", Description = "必須為單位才可刪除")]
        MustBeOrganizationToDelete = 4018,

        /// <summary>
        /// 檔案為空
        /// </summary>
        [Display(Name = "檔案為空", Description = "檔案為空")]
        FileIsEmpty = 4019,

        /// <summary>
        /// 檔案無效
        /// </summary>
        [Display(Name = "檔案無效", Description = "檔案無效")]
        FileIsInvalid = 4020,

        /// <summary>
        /// 檔案壓縮失敗
        /// </summary>
        [Display(Name = "檔案壓縮失敗", Description = "檔案壓縮失敗")]
        CompressionFailed = 4021,

        /// <summary>
        /// 帳號不能用身分證字號
        /// </summary>
        [Display(Name = "帳號不能用身分證字號", Description = "帳號不能用身分證字號")]
        AcCannotUseIdCardNumber = 4022,

        /// <summary>
        /// 檔案類型錯誤
        /// </summary>
        [Display(Name = "檔案類型錯誤", Description = "檔案類型錯誤")]
        UnValidFileTypeError = 4023,

        /// <summary>
        /// 必填欄位
        /// </summary>
        [Display(Name = "必填欄位", Description = "必填欄位")]
        FieldMustBeFilled = 4024,

        /// <summary>
        /// 查無孳生物
        /// </summary>
        [Display(Name = "查無孳生物", Description = "查無孳生物")]
        AnimalNotFound = 4025,
        /// <summary>
        /// 查無資料
        /// </summary>
        [Display(Name = "查無資料", Description = "查無資料")]
        DataEmpty = 4026,
        /// <summary>
        /// 查無原料
        /// </summary>
        [Display(Name = "查無原料", Description = "查無原料")]
        UnFoundMaterialRaw = 4050,
        /// <summary>
        /// 已存在相同代碼
        /// </summary>
        [Display(Name = "已存在相同代碼", Description = "已存在相同代碼")]
        SameCode = 4051,
        /// <summary>
        /// 項目已被選擇過
        /// </summary>
        [Display(Name = "項目已被選擇過", Description = "項目已被選擇過")]
        ChoosedItem = 4052,
        /// <summary>
        /// 此帳號尚未驗證信箱，或填入之信箱與驗證信箱不同
        /// </summary>
        [Display(Name = "此帳號尚未驗證信箱，或填入之信箱與驗證信箱不同", Description = "此帳號尚未驗證信箱，或填入之信箱與驗證信箱不同")]
        ForgetSwEmailError = 4053,
        /// <summary>
        /// 尚未設定原物料定價設定
        /// </summary>
        [Display(Name = "尚未設定原物料定價設定", Description = "尚未設定原物料定價設定")]
        NotFoundRawPricing = 4054,
        /// <summary>
        /// 您選擇的資料中，部分數量不足
        /// </summary>
        [Display(Name = "您選擇的資料中，部分數量不足", Description = "您選擇的資料中，部分數量不足")]
        AmountNotEnough = 4101,
        /// <summary>
        /// 接收單位無該畜禽種類或品種
        /// </summary>
        [Display(Name = "接收單位無該畜禽種類或品種", Description = "接收單位無該畜禽種類或品種")]
        ReceivingUnitNotHaveVariety = 4102,
        /// <summary>
        /// 請選擇多租戶機關
        /// </summary>
        [Display(Name = "請選擇多租戶機關", Description = "請選擇多租戶機關")]
        UnChooseTenacyId = 5000,

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
