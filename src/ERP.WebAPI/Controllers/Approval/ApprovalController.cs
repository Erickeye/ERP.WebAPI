using ERP.EntityModels.Models;
using ERP.Library.Enums;
using ERP.Library.Enums._1000Company;
using ERP.Library.Enums.Other;
using ERP.Library.ViewModels;
using ERP.Service.API;
using ERP.Service.Helpers;
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
        private readonly IApprovalService _service;

        public ApprovalController(IApprovalService service)
        {
            _service = service;
        }

        [SwaggerOperation("送出簽核流程")]
        [HttpPost, Route("SendApprovalProcess")]
        [Log(OperationActionType.Submit, "送出簽核流程")]
        public async Task<IActionResult> SendApprovalProcess(SendApprovalProcessVM data)
        {
            var result = await _service.SendApprovalProcess(data);
            return Ok(result);
        }

        [SwaggerOperation("檢視簽核紀錄")]
        [HttpGet, Route("GetApprovalProgress")]
        [Log(OperationActionType.Submit, "送出簽核流程")]
        public async Task<IActionResult> GetApprovalProgress([FromQuery] SendApprovalProcessVM data)
        {
            var result = await _service.GetApprovalProgress(data);
            return Ok(result);
        }

        [SwaggerOperation("簽核作業")]
        [HttpPost, Route("Approval")]
        [Log(OperationActionType.Submit, "簽核作業")]
        public async Task<IActionResult> Approval(ApprovalVM data)
        {
            var result = await _service.Approval(data);
            return Ok(result);
        }

        [SwaggerOperation("拒絕簽核作業")]
        [HttpPost, Route("RejectApproval")]
        [Log(OperationActionType.Submit, "拒絕簽核作業")]
        public async Task<IActionResult> RejectApproval(ApprovalVM data)
        {
            var result = await _service.RejectApproval(data);
            return Ok(result);
        }

        [SwaggerOperation("取得簽核訊息通知")]
        [HttpGet, Route("GetApprovalNotify")]
        [Log(OperationActionType.Submit, "取得簽核訊息通知")]
        public async Task<IActionResult> GetApprovalNotify()
        {
            var result = await _service.GetApprovalNotify();
            return Ok(result);
        }

        [SwaggerOperation("檢視簽核設定列表")]
        [HttpGet, Route("SettingsIndex")]
        [Log(OperationActionType.View, "檢視簽核設定")]
        public async Task<IActionResult> SettingsIndex()
        {
            var result = await _service.SettingsIndex();
            return Ok(result);
        }

        [SwaggerOperation("檢視簽核設定")]
        [HttpGet, Route("CheckSettings")]
        [Log(OperationActionType.View, "檢視簽核設定")]
        public async Task<IActionResult> CheckSettings([SwaggerParameter("簽核設定流水號")] int approvalSettingsId)
        {
            var result = await _service.CheckSettings(approvalSettingsId);
            return Ok(result);
        }
        [ValidateModel]
        [SwaggerOperation("新增簽核設定")]
        [HttpPost, Route("CreateSettings")]
        [Log(OperationActionType.Create, "新增簽核設定")]
        public async Task<IActionResult> CreateSettings(ApprovalSettingsInputVM vm)
        {
            var result = await _service.CreateSettings(vm);
            return Ok(result);
        }
        [ValidateModel]
        [SwaggerOperation("編輯簽核設定")]
        [HttpPost, Route("EditSetting")]
        [Log(OperationActionType.Edit, "編輯簽核設定")]
        public async Task<IActionResult> EditSetting(ApprovalCheckSettingsVM vm)
        {
            var result = await _service.EditSetting(vm);
            return Ok(result);
        }
        [ValidateModel]
        [SwaggerOperation("刪除簽核設定")]
        [HttpPost, Route("DeleteSettings")]
        [Log(OperationActionType.Edit, "刪除簽核設定")]
        public async Task<IActionResult> DeleteSettings(int id)
        {
            var result = await _service.DeleteSettings(id);
            return Ok(result);
        }
    }
}
