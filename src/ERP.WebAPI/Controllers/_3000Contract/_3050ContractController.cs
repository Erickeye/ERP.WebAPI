using ERP.Library.Enums;
using ERP.Library.Enums.Login;
using ERP.Library.ViewModels;
using ERP.Library.ViewModels._3000Contract;
using ERP.Service.API._3000Contract;
using ERP.WebAPI.CustomAttributes;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace ERP.WebAPI.Controllers._3000Contract
{
    [SwaggerTag("合約單")]
    [ApiController]
    [Route("api/[controller]")]
    public class _3050ContractController : ControllerBase
    {
        private readonly I_3050ContractService _service;

        public _3050ContractController(I_3050ContractService service)
        {
            _service = service;
        }

        // 列表、單筆與下載不加功能權限：簽核人不需要額外權限，可見範圍由 Service 依權限、建單人與簽核紀錄限縮
        [SwaggerOperation("查詢合約單列表")]
        [HttpGet, Route("Index")]
        public async Task<IActionResult> Index([FromQuery] ContractSearchVM vm)
        {
            var result = await _service.Index(vm);
            return Ok(result);
        }

        [SwaggerOperation("取得合約單")]
        [HttpGet, Route("Get")]
        public async Task<IActionResult> Get([SwaggerParameter("合約單流水號")] int id)
        {
            var result = await _service.Get(id);
            return Ok(result);
        }

        [SwaggerOperation("建立合約單")]
        [PermissionAuthorize((int)PermissionType.合約單編輯)]
        [ValidateModel]
        [HttpPost, Route("Add")]
        [Log(OperationActionType.Create, "建立合約單")]
        public async Task<IActionResult> Add(ContractAddVM vm)
        {
            var result = await _service.Add(vm);
            return Ok(result);
        }

        [SwaggerOperation("修改合約單")]
        [PermissionAuthorize((int)PermissionType.合約單編輯)]
        [ValidateModel]
        [HttpPost, Route("Edit")]
        [Log(OperationActionType.Edit, "修改合約單")]
        public async Task<IActionResult> Edit(ContractAddVM vm)
        {
            var result = await _service.Edit(vm);
            return Ok(result);
        }

        // 上傳與更換限草稿、已退回，由 Service 檢查狀態；表單欄位為 id 與 file（PDF，10 MB 以內）
        [SwaggerOperation("上傳或更換合約掃描檔")]
        [PermissionAuthorize((int)PermissionType.合約單編輯)]
        [ValidateModel]
        [RequestSizeLimit(12 * 1024 * 1024)]
        [HttpPost, Route("UploadAttachment")]
        [Log(OperationActionType.Edit, "上傳合約掃描檔")]
        public async Task<IActionResult> UploadAttachment([FromForm] ContractAttachmentVM vm)
        {
            var result = await _service.UploadAttachment(vm);
            return Ok(result);
        }

        [SwaggerOperation("下載合約掃描檔")]
        [HttpGet, Route("DownloadAttachment")]
        [Log(OperationActionType.View, "下載合約掃描檔")]
        public async Task<IActionResult> DownloadAttachment([SwaggerParameter("合約單流水號")] int id)
        {
            var result = await _service.DownloadAttachment(id);
            if (!result.IsSuccess || result.Data == null)
            {
                return Ok(result);
            }

            return File(result.Data.Content, "application/pdf", result.Data.FileName);
        }

        [SwaggerOperation("送出簽核合約單")]
        [PermissionAuthorize((int)PermissionType.合約單編輯)]
        [ValidateModel]
        [HttpPost, Route("SendApproval")]
        [Log(OperationActionType.Approval, "送出簽核合約單")]
        public async Task<IActionResult> SendApproval(ApprovalVM vm)
        {
            var result = await _service.SendApproval(vm);
            return Ok(result);
        }

        // 核准與拒絕不加功能權限：由簽核紀錄決定能否簽核（引擎檢查是否輪到）
        [SwaggerOperation("簽核合約單")]
        [ValidateModel]
        [HttpPost, Route("Approval")]
        [Log(OperationActionType.Approval, "簽核合約單")]
        public async Task<IActionResult> Approval(ApprovalVM vm)
        {
            var result = await _service.Approval(vm);
            return Ok(result);
        }

        [SwaggerOperation("拒絕簽核合約單")]
        [ValidateModel]
        [HttpPost, Route("RejectApproval")]
        [Log(OperationActionType.Approval, "拒絕簽核合約單")]
        public async Task<IActionResult> RejectApproval(ApprovalVM vm)
        {
            var result = await _service.RejectApproval(vm);
            return Ok(result);
        }

        [SwaggerOperation("撤銷簽核合約單")]
        [PermissionAuthorize((int)PermissionType.合約單編輯)]
        [ValidateModel]
        [HttpPost, Route("RevokeApproval")]
        [Log(OperationActionType.RevokeApproval, "撤銷簽核合約單")]
        public async Task<IActionResult> RevokeApproval(ApprovalVM vm)
        {
            var result = await _service.RevokeApproval(vm);
            return Ok(result);
        }

        // 作廢不加功能權限：由簽核紀錄決定（引擎檢查是否為最後一關簽核人）
        [SwaggerOperation("作廢合約單")]
        [ValidateModel]
        [HttpPost, Route("VoidApproval")]
        [Log(OperationActionType.RevokeApproval, "作廢合約單")]
        public async Task<IActionResult> VoidApproval(ApprovalVM vm)
        {
            var result = await _service.VoidApproval(vm);
            return Ok(result);
        }

        [SwaggerOperation("刪除合約單")]
        [PermissionAuthorize((int)PermissionType.合約單刪除)]
        [HttpDelete, Route("Delete")]
        [Log(OperationActionType.Delete, "刪除合約單")]
        public async Task<IActionResult> Delete([SwaggerParameter("合約單流水號")] int id)
        {
            var result = await _service.Delete(id);
            return Ok(result);
        }
    }
}
