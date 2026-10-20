using ERP.Library.Enums;
using ERP.Library.Enums.Login;
using ERP.Library.ViewModels;
using ERP.Library.ViewModels._4000Inventory;
using ERP.Service.API;
using ERP.Service.API._4000Inventory;
using ERP.WebAPI.CustomAttributes;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace ERP.WebAPI.Controllers._4000Inventory
{
    [SwaggerTag("領料單")]
    [ApiController]
    [Route("api/[controller]")]
    public class _4030MaterialIssueController : ControllerBase
    {
        private readonly I_4030MaterialIssueService _service;

        public _4030MaterialIssueController(I_4030MaterialIssueService service)
        {
            _service = service;
        }

        [SwaggerOperation("查詢領料單列表")]
        [PermissionAuthorize((int)PermissionType.領料單檢視)]
        [HttpGet, Route("Index")]
        public async Task<IActionResult> Index([FromQuery] MaterialIssueSearchVM vm)
        {
            var result = await _service.Index(vm);
            return Ok(result);
        }

        [SwaggerOperation("取得領料單")]
        [PermissionAuthorize((int)PermissionType.領料單檢視)]
        [HttpGet, Route("Get")]
        public async Task<IActionResult> Get([SwaggerParameter("領料單流水號")] int id)
        {
            var result = await _service.Get(id);
            return Ok(result);
        }

        [SwaggerOperation("匯出領料單列表")]
        [PermissionAuthorize((int)PermissionType.領料單檢視)]
        [Log(OperationActionType.Export, "匯出領料單列表")]
        [HttpGet, Route("Export")]
        public async Task<IActionResult> Export([FromQuery] MaterialIssueSearchVM vm)
        {
            var result = await _service.Export(vm);
            return Ok(result);
        }

        [SwaggerOperation("建立領料單")]
        [PermissionAuthorize((int)PermissionType.領料單編輯)]
        [ValidateModel]
        [HttpPost, Route("Add")]
        [Log(OperationActionType.Create, "建立領料單")]
        public async Task<IActionResult> Add(MaterialIssueAddVM vm)
        {
            var result = await _service.Add(vm);
            return Ok(result);
        }

        [SwaggerOperation("修改領料單")]
        [PermissionAuthorize((int)PermissionType.領料單編輯)]
        [ValidateModel]
        [HttpPost, Route("Edit")]
        [Log(OperationActionType.Edit, "修改領料單")]
        public async Task<IActionResult> Edit(MaterialIssueAddVM vm)
        {
            var result = await _service.Edit(vm);
            return Ok(result);
        }

        [SwaggerOperation("刪除領料單")]
        [PermissionAuthorize((int)PermissionType.領料單刪除)]
        [HttpDelete, Route("Delete")]
        [Log(OperationActionType.Delete, "刪除領料單")]
        public async Task<IActionResult> Delete([SwaggerParameter("領料單流水號")] int id)
        {
            var result = await _service.Delete(id);
            return Ok(result);
        }

        [SwaggerOperation("送出簽核領料單")]
        [PermissionAuthorize((int)PermissionType.領料單編輯)]
        [ValidateModel]
        [HttpPost, Route("SendApproval")]
        [Log(OperationActionType.Approval, "送出簽核領料單")]
        public async Task<IActionResult> SendApproval(ApprovalVM vm)
        {
            var result = await _service.SendApproval(vm);
            return Ok(result);
        }

        [SwaggerOperation("簽核領料單")]
        [PermissionAuthorize((int)PermissionType.領料單檢視)]
        [ValidateModel]
        [HttpPost, Route("Approval")]
        [Log(OperationActionType.Approval, "簽核領料單")]
        public async Task<IActionResult> Approval(ApprovalVM vm)
        {
            var result = await _service.Approval(vm);
            return Ok(result);
        }

        [SwaggerOperation("拒絕簽核領料單")]
        [PermissionAuthorize((int)PermissionType.領料單檢視)]
        [ValidateModel]
        [HttpPost, Route("RejectApproval")]
        [Log(OperationActionType.Approval, "拒絕簽核領料單")]
        public async Task<IActionResult> RejectApproval(ApprovalVM vm)
        {
            var result = await _service.RejectApproval(vm);
            return Ok(result);
        }

        [SwaggerOperation("撤銷送簽領料單")]
        [PermissionAuthorize((int)PermissionType.領料單編輯)]
        [ValidateModel]
        [HttpPost, Route("RevokeApproval")]
        [Log(OperationActionType.RevokeApproval, "撤銷送簽領料單")]
        public async Task<IActionResult> RevokeApproval(ApprovalVM vm)
        {
            var result = await _service.RevokeApproval(vm);
            return Ok(result);
        }

        [SwaggerOperation("作廢領料單")]
        [PermissionAuthorize((int)PermissionType.領料單檢視)]
        [ValidateModel]
        [HttpPost, Route("VoidApproval")]
        [Log(OperationActionType.RevokeApproval, "作廢領料單")]
        public async Task<IActionResult> VoidApproval(ApprovalVM vm)
        {
            var result = await _service.VoidApproval(vm);
            return Ok(result);
        }
    }
}
