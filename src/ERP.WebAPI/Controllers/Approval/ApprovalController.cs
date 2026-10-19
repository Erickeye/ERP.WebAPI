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
            if (data.TableType == TableType.領料單) return Ok(MaterialIssueBlocked());
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
            if (data.TableType == TableType.領料單) return Ok(MaterialIssueBlocked());
            var result = await _workflowService.Approval(data);
            return Ok(result);
        }

        [SwaggerOperation("拒絕簽核作業")]
        [HttpPost, Route("RejectApproval")]
        public async Task<IActionResult> RejectApproval(ApprovalVM data)
        {
            if (data.TableType == TableType.領料單) return Ok(MaterialIssueBlocked());
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
            if (data.TableType == TableType.領料單) return Ok(MaterialIssueBlocked());
            var result = await _workflowService.RevokeApproval(data);
            return Ok(result);
        }
        [ValidateModel]
        [SwaggerOperation("撤銷簽核作業")]
        [HttpPost, Route("VoidApproval")]
        [Log(OperationActionType.RevokeApproval, "撤銷簽核作業")]
        public async Task<IActionResult> VoidApproval(ApprovalVM data)
        {
            if (data.TableType == TableType.領料單) return Ok(MaterialIssueBlocked());
            var result = await _workflowService.VoidApproval(data);
            return Ok(result);
        }

        /// <summary>
        /// 領料單的簽核動作需與扣庫存同一交易，必須走領料單專用端點
        /// </summary>
        private static ResultModel<string> MaterialIssueBlocked()
        {
            return ResultModel.Error(ErrorCodeType.InvalidApproval, "領料單請使用領料單專用的簽核功能");
        }
    }
}
