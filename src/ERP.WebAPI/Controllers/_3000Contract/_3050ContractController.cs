using ERP.Library.Enums;
using ERP.Library.Enums.Login;
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

        [SwaggerOperation("查詢合約單列表")]
        [PermissionAuthorize((int)PermissionType.合約單檢視)]
        [HttpGet, Route("Index")]
        public async Task<IActionResult> Index([FromQuery] ContractSearchVM vm)
        {
            var result = await _service.Index(vm);
            return Ok(result);
        }

        [SwaggerOperation("取得合約單")]
        [PermissionAuthorize((int)PermissionType.合約單檢視)]
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
        [PermissionAuthorize((int)PermissionType.合約單檢視)]
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
