using ERP.EntityModels.Models.Other;
using ERP.Library.Enums;
using ERP.Library.ViewModels.Approval;
using ERP.Service.API;
using ERP.WebAPI.CustomAttributes;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using static System.Runtime.InteropServices.JavaScript.JSType;

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
            return Ok();
        }

        [SwaggerOperation("檢視簽核設定")]
        [HttpGet, Route("CheckSettings")]
        [Log(OperationActionType.View, "檢視簽核設定")]
        public async Task<IActionResult> CheckSettings()
        {
            var result = await _service.CheckSettings();
            return Ok();
        }

        [SwaggerOperation("新增簽核設定")]
        [HttpPost,Route("CreateSettings")]
        [Log(OperationActionType.Create, "新增簽核設定")]
        public async Task<IActionResult> CreateSettings(ApprovalSettings data)
        {
            var result = await _service.CreateOrEditSettings(data);
            return Ok(result);
        }

        [SwaggerOperation("修改簽核設定")]
        [HttpPost, Route("EditSettings")]
        [Log(OperationActionType.Edit, "修改簽核設定")]
        public async Task<IActionResult> Editettings(ApprovalSettings data)
        {
            var result = await _service.CreateOrEditSettings(data);
            return Ok(result);
        }

        [SwaggerOperation("檢視簽核步驟")]
        [HttpGet, Route("CheckStep")]
        [Log(OperationActionType.View, "檢視簽核步驟")]
        public async Task<IActionResult> CheckStep(int approvalSettingsId)
        {
            var result = await _service.CheckStep(approvalSettingsId);
            return Ok();
        }

        [SwaggerOperation("新增簽核步驟")]
        [HttpPost, Route("CreateStep")]
        [Log(OperationActionType.Create, "新增簽核步驟")]
        public async Task<IActionResult> CreateStep(ApprovalStep data)
        {
            var result = await _service.CreateOrEditStep(data);
            return Ok(result);
        }

        [SwaggerOperation("修改簽核步驟")]
        [HttpPost, Route("EditStep")]
        [Log(OperationActionType.Edit, "修改簽核步驟")]
        public async Task<IActionResult> Editettings(ApprovalStep data)
        {
            var result = await _service.CreateOrEditStep(data);
            return Ok(result);
        }
    }
}
