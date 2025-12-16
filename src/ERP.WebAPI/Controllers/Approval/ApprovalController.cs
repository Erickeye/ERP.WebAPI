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

        [SwaggerOperation("檢視簽核設定")]
        [HttpGet, Route("CheckSettings")]
        [Log(OperationActionType.View, "檢視簽核設定")]
        public async Task<IActionResult> CheckSettings()
        {
            var result = await _service.CheckSettings();
            return Ok(result);
        }

        [ValidateModel]
        [SwaggerOperation("新增簽核設定")]
        [HttpPost,Route("CreateSettings")]
        [Log(OperationActionType.Create, "新增簽核設定")]
        public async Task<IActionResult> CreateSettings(ApprovalSettingsInputVM vm)
        {
            var result = await _service.CreateSettings(vm);
            return Ok(result);
        }

        [ValidateModel]
        [SwaggerOperation("修改簽核設定")]
        [HttpPost, Route("EditSettings")]
        [Log(OperationActionType.Edit, "修改簽核設定")]
        public async Task<IActionResult> EditSettings(ApprovalSettingsInputVM vm)
        {
            var result = await _service.EditSettings(vm);
            return Ok(result);
        }

        [SwaggerOperation("檢視簽核步驟")]
        [HttpGet, Route("CheckStep")]
        [Log(OperationActionType.View, "檢視簽核步驟")]
        public async Task<IActionResult> CheckStep( [SwaggerParameter("簽核設定流水號")] int approvalSettingsId)
        {
            var result = await _service.CheckStep(approvalSettingsId);
            return Ok(result);
        }

        [SwaggerOperation("新增簽核步驟")]
        [HttpPost, Route("CreateStep")]
        [Log(OperationActionType.Create, "新增簽核步驟")]
        public async Task<IActionResult> CreateStep(ApprovakStepInputVM vm)
        {
            var result = await _service.CreateStep(vm);
            return Ok(result);
        }

        [SwaggerOperation("修改簽核步驟")]
        [HttpPost, Route("EditStep")]
        [Log(OperationActionType.Edit, "修改簽核步驟")]
        public async Task<IActionResult> EditStep(ApprovakStepInputVM vm)
        {
            var result = await _service.EditStep(vm);
            return Ok(result);
        }

        [SwaggerOperation("刪除簽核步驟")]
        [HttpDelete, Route("DeleteStep")]
        [Log(OperationActionType.Delete, "刪除簽核步驟")]
        public async Task<IActionResult> DeleteStep(int id)
        {
            var result = await _service.DeleteStep(id);
            return Ok(result);
        }

        [SwaggerOperation("檢視簽核步驟")]
        [HttpGet, Route("CheckStepNumber")]
        [Log(OperationActionType.View, "檢視簽核步驟")]
        public async Task<IActionResult> CheckStepNumber(int ApprovalStepId)
        {
            var result = await _service.CheckStepNumber(ApprovalStepId);
            return Ok(result);
        }

        [SwaggerOperation("新增簽核步驟成員")]
        [HttpPost, Route("CreateStepNumber")]
        [Log(OperationActionType.Create, "新增簽核步驟成員")]
        public async Task<IActionResult> CreateStepNumber(ApprovalStepNumberInputVM data)
        {
            var result = await _service.CreateOrEditStepNumber(data);
            return Ok(result);
        }

        [SwaggerOperation("修改簽核步驟成員")]
        [HttpPost, Route("EditStepNumber")]
        [Log(OperationActionType.Edit, "修改簽核步驟成員")]
        public async Task<IActionResult> EditStepNumber(ApprovalStepNumberInputVM data)
        {
            var result = await _service.CreateOrEditStepNumber(data);
            return Ok(result);
        }

        [SwaggerOperation("刪除簽核步驟成員")]
        [HttpDelete, Route("EditStepNumber")]
        [Log(OperationActionType.Delete, "刪除簽核步驟成員")]
        public async Task<IActionResult> DeleteStepNumber(int id)
        {
            var result = await _service.DeleteStepNumber(id);
            return Ok(result);
        }

        [SwaggerOperation("取得簽核表單種類")]
        [HttpGet, Route("GetTableType")]
        public IActionResult GetTableType()
        {
            var result = EnumHelper.GetEnumList<TableType>();
            return Ok(result);
        }

       [SwaggerOperation("取得簽核表單模型")]
       [HttpGet, Route("GetApprovalMode")]
        public IActionResult GetApprovalMode()
        {
            var result = EnumHelper.GetEnumList<ApprovalMode>();
            return Ok(result);
        }
    }
}
