using ERP.Library.Enums;
using ERP.Library.Enums.Other;
using ERP.Library.ViewModels;
using ERP.Tests.Support;
using Xunit;
using static ERP.Tests.Support.ApprovalEngineHarness;

namespace ERP.Tests;

/// <summary>
/// 簽核引擎（真實 <see cref="ApprovalWorkflowService"/> + 記憶體資料庫）的撤銷與作廢行為，
/// 同時覆蓋「指定人員」與「指定角色」兩種簽核模式。
/// 角色：送簽人 1（角色 10）、第一關簽核人 2（角色 20）、最後一關簽核人 3（角色 30）、無關人員 9（角色 90）。
/// </summary>
public class ApprovalWorkflowServiceTests : IDisposable
{
    private const int Sender = 1, SenderRole = 10;
    private const int FirstApprover = 2, FirstRole = 20;
    private const int LastApprover = 3, LastRole = 30;
    private const int Outsider = 9, OutsiderRole = 90;

    private readonly ApprovalEngineHarness _engine = new();

    public void Dispose() => _engine.Dispose();

    private void SeedSpecifyFlow() => _engine.SeedSetting(StepSeed.Specify(FirstApprover), StepSeed.Specify(LastApprover));

    private void SeedRoleFlow() => _engine.SeedSetting(StepSeed.Role(FirstRole), StepSeed.Role(LastRole));

    private async Task Send()
    {
        _engine.ActAs(Sender, SenderRole);
        AssertOk(await _engine.Service.SendApprovalProcess(Document()));
    }

    private async Task ApproveAs(int userId, int roleId)
    {
        _engine.ActAs(userId, roleId);
        AssertOk(await _engine.Service.Approval(Document()));
    }

    private async Task SendAndApproveAll()
    {
        await Send();
        await ApproveAs(FirstApprover, FirstRole);
        await ApproveAs(LastApprover, LastRole);
    }

    private static void AssertOk(ResultModel<string> result) => Assert.True(result.IsSuccess, result.ErrorMessage);

    private async Task<List<GetApprovalProgressVM>> Progress()
    {
        var result = await _engine.Service.GetApprovalProgress(Document());
        Assert.True(result.IsSuccess);
        return result.Data!.Items.ToList();
    }

    private async Task<List<ApprovalStatus>> Statuses() =>
        (await Progress()).Select(x => (ApprovalStatus)x.Status).ToList();

    // ---------- 撤銷 ----------

    [Fact]
    public async Task 指定人員_送簽人可撤銷仍有待簽的簽核()
    {
        SeedSpecifyFlow();
        await Send();

        var result = await _engine.Service.RevokeApproval(Document());

        AssertOk(result);
        Assert.Equal(
            new[] { ApprovalStatus.Approved, ApprovalStatus.GetRevoked, ApprovalStatus.GetRevoked },
            await Statuses());
    }

    [Fact]
    public async Task 指定角色_送簽人可撤銷仍有待簽的簽核()
    {
        SeedRoleFlow();
        await Send();

        var result = await _engine.Service.RevokeApproval(Document());

        AssertOk(result);
        Assert.Equal(
            new[] { ApprovalStatus.Approved, ApprovalStatus.GetRevoked, ApprovalStatus.GetRevoked },
            await Statuses());
    }

    [Fact]
    public async Task 指定人員_第一關已核准後送簽人仍可撤銷剩下的待簽()
    {
        SeedSpecifyFlow();
        await Send();
        await ApproveAs(FirstApprover, FirstRole);

        _engine.ActAs(Sender, SenderRole);
        AssertOk(await _engine.Service.RevokeApproval(Document()));

        Assert.Equal(
            new[] { ApprovalStatus.Approved, ApprovalStatus.Approved, ApprovalStatus.GetRevoked },
            await Statuses());
    }

    [Fact]
    public async Task 指定人員_非送簽人不可撤銷_即使是待簽的簽核人()
    {
        SeedSpecifyFlow();
        await Send();
        await ApproveAs(FirstApprover, FirstRole);

        _engine.ActAs(LastApprover, LastRole);
        var result = await _engine.Service.RevokeApproval(Document());

        Assert.False(result.IsSuccess);
        Assert.Equal(
            new[] { ApprovalStatus.Approved, ApprovalStatus.Approved, ApprovalStatus.Pending },
            await Statuses());
    }

    [Fact]
    public async Task 指定角色_非送簽人不可撤銷_即使角色符合待簽關卡()
    {
        SeedRoleFlow();
        await Send();

        _engine.ActAs(FirstApprover, FirstRole);
        var result = await _engine.Service.RevokeApproval(Document());

        Assert.False(result.IsSuccess);
        Assert.Equal(
            new[] { ApprovalStatus.Approved, ApprovalStatus.Pending, ApprovalStatus.Pending },
            await Statuses());
    }

    [Fact]
    public async Task 沒有待簽紀錄時送簽人也不可撤銷()
    {
        SeedSpecifyFlow();
        await SendAndApproveAll();

        _engine.ActAs(Sender, SenderRole);
        var result = await _engine.Service.RevokeApproval(Document());

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorCodeType.InvalidUserOperation, result.ErrorCode);
        Assert.All(await Statuses(), status => Assert.Equal(ApprovalStatus.Approved, status));
    }

    [Fact]
    public async Task 找不到簽核紀錄時不可撤銷()
    {
        SeedSpecifyFlow();

        _engine.ActAs(Sender, SenderRole);
        var result = await _engine.Service.RevokeApproval(Document());

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorCodeType.NotFoundData, result.ErrorCode);
    }

    [Fact]
    public async Task 撤銷後重送_新的送簽人可撤銷新一輪的簽核()
    {
        SeedRoleFlow();
        await Send();
        AssertOk(await _engine.Service.RevokeApproval(Document()));

        // 換另一個人重新送簽
        _engine.ActAs(Outsider, OutsiderRole);
        AssertOk(await _engine.Service.SendApprovalProcess(Document()));

        _engine.ActAs(Sender, SenderRole);
        Assert.False((await _engine.Service.RevokeApproval(Document())).IsSuccess);

        _engine.ActAs(Outsider, OutsiderRole);
        AssertOk(await _engine.Service.RevokeApproval(Document()));
    }

    [Fact]
    public async Task 拒絕後可重新送簽_舊紀錄保留且新一輪進入待簽()
    {
        SeedSpecifyFlow();
        await Send();
        await ApproveAs(FirstApprover, FirstRole);
        _engine.ActAs(LastApprover, LastRole);
        AssertOk(await _engine.Service.RejectApproval(new() { TableType = ApprovalEngineHarness.DocumentType, TableId = DocumentId, Memo = "資料有誤" }));

        _engine.ActAs(Sender, SenderRole);
        AssertOk(await _engine.Service.SendApprovalProcess(Document()));

        // 新一輪：第一關、第二關重新待簽；送簽人可撤銷新一輪
        AssertOk(await _engine.Service.RevokeApproval(Document()));
        Assert.Equal(2, (await Statuses()).Count(x => x == ApprovalStatus.GetRevoked));
    }

    // ---------- 核准記錄簽核人 ----------

    [Fact]
    public async Task 指定角色_核准時記錄實際簽核人()
    {
        SeedRoleFlow();
        await Send();

        await ApproveAs(FirstApprover, FirstRole);
        await ApproveAs(LastApprover, LastRole);

        var records = await Progress();
        Assert.Equal(FirstApprover, records.Single(x => x.StepOrder == 1).UserId);
        Assert.Equal(LastApprover, records.Single(x => x.StepOrder == 2).UserId);
    }

    [Fact]
    public async Task 指定人員_核准時簽核人仍是設定的人員()
    {
        SeedSpecifyFlow();
        await Send();

        await ApproveAs(FirstApprover, FirstRole);
        await ApproveAs(LastApprover, LastRole);

        var records = await Progress();
        Assert.Equal(FirstApprover, records.Single(x => x.StepOrder == 1).UserId);
        Assert.Equal(LastApprover, records.Single(x => x.StepOrder == 2).UserId);
    }

    // ---------- 作廢 ----------

    [Fact]
    public async Task 指定人員_最後一關簽核人可作廢全部通過的簽核()
    {
        SeedSpecifyFlow();
        await SendAndApproveAll();

        _engine.ActAs(LastApprover, LastRole);
        AssertOk(await _engine.Service.VoidApproval(Document()));

        Assert.All(await Statuses(), status => Assert.Equal(ApprovalStatus.GetRevoked, status));
    }

    [Fact]
    public async Task 指定角色_最後一關簽核人可作廢全部通過的簽核()
    {
        SeedRoleFlow();
        await SendAndApproveAll();

        _engine.ActAs(LastApprover, LastRole);
        AssertOk(await _engine.Service.VoidApproval(Document()));

        Assert.All(await Statuses(), status => Assert.Equal(ApprovalStatus.GetRevoked, status));
    }

    [Fact]
    public async Task 指定人員_同一關多位簽核人時任一位核准過的人可作廢()
    {
        _engine.SeedSetting(StepSeed.Specify(FirstApprover), StepSeed.Specify(LastApprover, Outsider));
        await Send();
        await ApproveAs(FirstApprover, FirstRole);
        await ApproveAs(LastApprover, LastRole);
        await ApproveAs(Outsider, OutsiderRole);

        _engine.ActAs(Outsider, OutsiderRole);
        AssertOk(await _engine.Service.VoidApproval(Document()));

        Assert.All(await Statuses(), status => Assert.Equal(ApprovalStatus.GetRevoked, status));
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task 非最後一關簽核人不可作廢(bool roleMode)
    {
        if (roleMode) SeedRoleFlow(); else SeedSpecifyFlow();
        await SendAndApproveAll();

        foreach (var (userId, roleId) in new[] { (Sender, SenderRole), (FirstApprover, FirstRole), (Outsider, OutsiderRole) })
        {
            _engine.ActAs(userId, roleId);
            Assert.False((await _engine.Service.VoidApproval(Document())).IsSuccess);
        }

        Assert.All(await Statuses(), status => Assert.Equal(ApprovalStatus.Approved, status));
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task 流程未全部通過時不可作廢(bool roleMode)
    {
        if (roleMode) SeedRoleFlow(); else SeedSpecifyFlow();
        await Send();
        await ApproveAs(FirstApprover, FirstRole);

        _engine.ActAs(LastApprover, LastRole);
        var result = await _engine.Service.VoidApproval(Document());

        Assert.False(result.IsSuccess);
        Assert.Equal(
            new[] { ApprovalStatus.Approved, ApprovalStatus.Approved, ApprovalStatus.Pending },
            await Statuses());
    }

    // ---------- 重送後的新一輪（舊一輪的紀錄仍保留在流程中） ----------

    private async Task RevokeAsSender()
    {
        _engine.ActAs(Sender, SenderRole);
        AssertOk(await _engine.Service.RevokeApproval(Document()));
    }

    private async Task RejectAs(int userId, int roleId)
    {
        _engine.ActAs(userId, roleId);
        AssertOk(await _engine.Service.RejectApproval(
            new() { TableType = DocumentType, TableId = DocumentId, Memo = "資料有誤" }));
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task 撤銷後重送再全部核准_最後一關簽核人可作廢(bool roleMode)
    {
        if (roleMode) SeedRoleFlow(); else SeedSpecifyFlow();
        await Send();
        await ApproveAs(FirstApprover, FirstRole);
        await RevokeAsSender();

        await SendAndApproveAll();

        _engine.ActAs(LastApprover, LastRole);
        AssertOk(await _engine.Service.VoidApproval(Document()));
    }

    [Fact]
    public async Task 指定人員_拒絕後重送再全部核准_最後一關簽核人可作廢()
    {
        SeedSpecifyFlow();
        await Send();
        await ApproveAs(FirstApprover, FirstRole);
        await RejectAs(LastApprover, LastRole);

        await Send();
        await ApproveAs(FirstApprover, FirstRole);
        await ApproveAs(LastApprover, LastRole);

        _engine.ActAs(LastApprover, LastRole);
        AssertOk(await _engine.Service.VoidApproval(Document()));
    }

    [Fact]
    public async Task 指定人員_拒絕後重送再作廢_只作廢新一輪_舊一輪的拒絕紀錄保留()
    {
        SeedSpecifyFlow();
        await Send();
        await RejectAs(FirstApprover, FirstRole);
        await SendAndApproveAll();

        _engine.ActAs(LastApprover, LastRole);
        AssertOk(await _engine.Service.VoidApproval(Document()));

        var statuses = await Statuses();
        Assert.Contains(ApprovalStatus.Rejected, statuses);
        Assert.Contains(ApprovalStatus.GetRejected, statuses);
        Assert.Equal(3, statuses.Count(x => x == ApprovalStatus.GetRevoked));
    }

    [Fact]
    public async Task 指定人員_拒絕後重送_新一輪仍未全部通過時不可作廢()
    {
        SeedSpecifyFlow();
        await Send();
        await RejectAs(FirstApprover, FirstRole);
        await Send();
        await ApproveAs(FirstApprover, FirstRole);

        _engine.ActAs(LastApprover, LastRole);
        var result = await _engine.Service.VoidApproval(Document());

        Assert.False(result.IsSuccess);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task 重送後第一關核准_不受舊一輪紀錄影響(bool roleMode)
    {
        if (roleMode) SeedRoleFlow(); else SeedSpecifyFlow();
        await Send();
        await RevokeAsSender();
        await Send();

        var result = await _engine.Service.GetApprovalNotify();
        Assert.Empty(result.Data!.Items);

        _engine.ActAs(FirstApprover, FirstRole);
        Assert.Single((await _engine.Service.GetApprovalNotify()).Data!.Items);
        AssertOk(await _engine.Service.Approval(Document()));
        _engine.ActAs(LastApprover, LastRole);
        Assert.Single((await _engine.Service.GetApprovalNotify()).Data!.Items);
    }

    [Fact]
    public async Task 作廢後不可再作廢也不可撤銷()
    {
        SeedRoleFlow();
        await SendAndApproveAll();
        _engine.ActAs(LastApprover, LastRole);
        AssertOk(await _engine.Service.VoidApproval(Document()));

        Assert.False((await _engine.Service.VoidApproval(Document())).IsSuccess);

        _engine.ActAs(Sender, SenderRole);
        Assert.False((await _engine.Service.RevokeApproval(Document())).IsSuccess);
    }
}
