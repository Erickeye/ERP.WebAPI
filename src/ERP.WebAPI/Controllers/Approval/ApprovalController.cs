using ERP.Approval.Abstractions;
using ERP.Library.Enums;
using ERP.Library.Enums.Other;
using ERP.Library.ViewModels;
using ERP.WebAPI.CustomAttributes;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace ERP.WebAPI.Controllers.Approval
{
    [SwaggerTag("簽核系統")]
    [ApiController]
    [Route("api/[controller]")]
    [ApiExplorerSettings(GroupName = "Default API")]
    public class ApprovalController : ControllerBase
    {
        private readonly IApprovalWorkflowService _workflowService;
        private readonly IApprovalSettingsService _settingsService;

        public ApprovalController(
            IApprovalWorkflowService workflowService,
            IApprovalSettingsService settingsService)
        {
            _workflowService = workflowService;
            _settingsService = settingsService;
        }

        [SwaggerOperation("送出簽核流程")]
        [HttpPost, Route("SendApprovalProcess")]
        public async Task<IActionResult> SendApprovalProcess(ApprovalVM data)
        {
            if (IsHandledByModule(data.TableType)) return Ok(ModuleBlocked(data.TableType));
            var result = await _workflowService.SendApprovalProcess(data);
            return Ok(result);
        }

        [SwaggerOperation("檢視簽核紀錄")]
        [HttpGet, Route("GetApprovalProgress")]
        public async Task<IActionResult> GetApprovalProgress([FromQuery] ApprovalVM data)
        {
            var result = await _workflowService.GetApprovalProgress(data);
            return Ok(result);
        }

        [SwaggerOperation("簽核作業")]
        [HttpPost, Route("Approval")]
        [Log(OperationActionType.Approval, "簽核作業")]
        public async Task<IActionResult> Approval(ApprovalVM data)
        {
            if (IsHandledByModule(data.TableType)) return Ok(ModuleBlocked(data.TableType));
            var result = await _workflowService.Approval(data);
            return Ok(result);
        }

        [SwaggerOperation("拒絕簽核作業")]
        [HttpPost, Route("RejectApproval")]
        public async Task<IActionResult> RejectApproval(ApprovalVM data)
        {
            if (IsHandledByModule(data.TableType)) return Ok(ModuleBlocked(data.TableType));
            var result = await _workflowService.RejectApproval(data);
            return Ok(result);
        }

        [SwaggerOperation("取得簽核訊息通知")]
        [HttpGet, Route("GetApprovalNotify")]
        public async Task<IActionResult> GetApprovalNotify()
        {
            var result = await _workflowService.GetApprovalNotify();
            return Ok(result);
        }

        [SwaggerOperation("檢視簽核設定列表")]
        [HttpGet, Route("SettingsIndex")]
        [Log(OperationActionType.View, "檢視簽核設定")]
        public async Task<IActionResult> SettingsIndex()
        {
            var result = await _settingsService.SettingsIndex();
            return Ok(result);
        }

        [SwaggerOperation("檢視簽核設定")]
        [HttpGet, Route("CheckSettings")]
        [Log(OperationActionType.View, "檢視簽核設定")]
        public async Task<IActionResult> CheckSettings([SwaggerParameter("簽核設定流水號")] int approvalSettingsId)
        {
            var result = await _settingsService.CheckSettings(approvalSettingsId);
            return Ok(result);
        }
        [ValidateModel]
        [SwaggerOperation("新增簽核設定")]
        [HttpPost, Route("CreateSettings")]
        [Log(OperationActionType.Create, "新增簽核設定")]
        public async Task<IActionResult> CreateSettings(ApprovalSettingsInputVM vm)
        {
            var result = await _settingsService.CreateSettings(vm);
            return Ok(result);
        }
        [ValidateModel]
        [SwaggerOperation("編輯簽核設定")]
        [HttpPost, Route("EditSetting")]
        [Log(OperationActionType.Edit, "編輯簽核設定")]
        public async Task<IActionResult> EditSetting(ApprovalCheckSettingsVM vm)
        {
            var result = await _settingsService.EditSetting(vm);
            return Ok(result);
        }
        [ValidateModel]
        [SwaggerOperation("刪除簽核設定")]
        [HttpPost, Route("DeleteSettings")]
        [Log(OperationActionType.Edit, "刪除簽核設定")]
        public async Task<IActionResult> DeleteSettings(int id)
        {
            var result = await _settingsService.DeleteSettings(id);
            return Ok(result);
        }

        [ValidateModel]
        [SwaggerOperation("撤銷簽核作業")]
        [HttpPost, Route("RevokeApproval")]
        [Log(OperationActionType.RevokeApproval, "撤銷簽核作業")]
        public async Task<IActionResult> RevokeApproval(ApprovalVM data)
        {
            if (IsHandledByModule(data.TableType)) return Ok(ModuleBlocked(data.TableType));
            var result = await _workflowService.RevokeApproval(data);
            return Ok(result);
        }
        [ValidateModel]
        [SwaggerOperation("撤銷簽核作業")]
        [HttpPost, Route("VoidApproval")]
        [Log(OperationActionType.RevokeApproval, "撤銷簽核作業")]
        public async Task<IActionResult> VoidApproval(ApprovalVM data)
        {
            if (IsHandledByModule(data.TableType)) return Ok(ModuleBlocked(data.TableType));
            var result = await _workflowService.VoidApproval(data);
            return Ok(result);
        }

        /// <summary>
        /// 領料單、出差單的簽核動作需與單據狀態（領料單另含庫存）同一交易，必須走各自的專用端點
        /// </summary>
        private static bool IsHandledByModule(TableType tableType)
        {
            return tableType == TableType.領料單 || tableType == TableType.出差單;
        }

        private static ResultModel<string> ModuleBlocked(TableType tableType)
        {
            var name = tableType == TableType.出差單 ? "出差單" : "領料單";
            return ResultModel.Error(ErrorCodeType.InvalidApproval, $"{name}請使用{name}專用的簽核功能");
        }
    }
}
