using ERP.Approval.Abstractions;
using ERP.Approval.Services;
using ERP.EntityModels.Models;
using ERP.Library.Enums.Other;
using ERP.Library.ViewModels;

namespace ERP.Tests.Support;

/// <summary>
/// 可切換的簽核操作者：測試中以 <see cref="ApprovalEngineHarness.ActAs"/> 切換目前登入者。
/// </summary>
public class FakeApprovalUserContext : IApprovalUserContext
{
    public int UserId { get; set; }
    public int RoleId { get; set; }
}

/// <summary>
/// 假的簽核目標處理器：任何單據都視為存在，讓真實簽核引擎可以送簽。
/// </summary>
public class FakeApprovalTargetHandlerResolver : IApprovalTargetHandlerResolver
{
    public IApprovalTargetHandler? Resolve(TableType tableType) => new AlwaysExistsHandler(tableType);

    private sealed class AlwaysExistsHandler : IApprovalTargetHandler
    {
        public AlwaysExistsHandler(TableType tableType) => TableType = tableType;

        public TableType TableType { get; }

        public Task<bool> ExistsAsync(string tableId) => Task.FromResult(true);
    }
}

/// <summary>
/// 真實的 <see cref="ApprovalWorkflowService"/> 配 SQLite 記憶體資料庫，
/// 用來測試簽核引擎本身（不使用假引擎）。
/// </summary>
public sealed class ApprovalEngineHarness : IDisposable
{
    public const TableType DocumentType = TableType.進貨單;
    public const string DocumentId = "DOC001";

    private readonly TestDb _testDb = new();
    private readonly FakeApprovalUserContext _userContext = new();

    public ApprovalEngineHarness()
    {
        Service = new ApprovalWorkflowService(
            _testDb.CreateContext(),
            _userContext,
            new FakeApprovalTargetHandlerResolver());
    }

    public ApprovalWorkflowService Service { get; }

    public static ApprovalVM Document() => new() { TableType = DocumentType, TableId = DocumentId };

    /// <summary>切換目前操作者（使用者與其角色）。</summary>
    public void ActAs(int userId, int roleId)
    {
        _userContext.UserId = userId;
        _userContext.RoleId = roleId;
    }

    /// <summary>建立一組啟用中的簽核設定，關卡依序為 StepOrder 1、2、...。</summary>
    public void SeedSetting(params StepSeed[] steps)
    {
        using var db = _testDb.CreateContext();
        var setting = new ApprovalSettings { TableType = (int)DocumentType, Name = "測試簽核", IsActive = true };
        for (var index = 0; index < steps.Length; index++)
        {
            var seed = steps[index];
            var step = new ApprovalStep
            {
                StepOrder = index + 1,
                Mode = (int)seed.Mode,
                RoleId = seed.RoleId,
                RequiredCounts = seed.Mode == ApprovalMode.Role ? seed.RequiredCounts : null
            };
            foreach (var userId in seed.UserIds)
            {
                step.ApprovalStepNumber.Add(new ApprovalStepNumber { UserId = userId });
            }
            setting.ApprovalStep.Add(step);
        }
        db.ApprovalSettings.Add(setting);
        db.SaveChanges();
    }

    public record StepSeed(ApprovalMode Mode, int RoleId, int[] UserIds, int RequiredCounts = 1)
    {
        public static StepSeed Specify(params int[] userIds) => new(ApprovalMode.Specify, 0, userIds);

        public static StepSeed Role(int roleId, int requiredCounts = 1) => new(ApprovalMode.Role, roleId, Array.Empty<int>(), requiredCounts);
    }

    public void Dispose() => _testDb.Dispose();
}
