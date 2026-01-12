using ERP.Library.Enums;
using ERP.Library.Enums.Login;
using ERP.Library.Extensions;
using ERP.Library.ViewModels._1000Company;
using ERP.Service.API._1000Company;
using ERP.WebAPI.CustomAttributes;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace ERP.WebAPI.Controllers._1000Company
{
    [SwaggerTag("員工")]
    [ApiController]
    [Route("api/[controller]")]
    [ApiExplorerSettings]
    public class _1000StaffController : ControllerBase
    {
        private readonly I_1000Service _service;

        public _1000StaffController(I_1000Service service)
        {
            _service = service;
        }

        [SwaggerOperation("檢視員工列表")]
        [HttpGet, Route("Index")]
        [PermissionAuthorize((int)PermissionType.員工檢視)]
        [Log(OperationActionType.View, "檢視員工列表")]
        public async Task<IActionResult> Index(string? deptID, bool isResignation = false)
        {
            var result = await _service.GetStaffIndex(deptID!, isResignation);
            return Ok(result);
        }

        [SwaggerOperation("新增")]
        [HttpPost, Route("Create")]
        [ValidateModel]
        [PermissionAuthorize((int)PermissionType.員工檢視)]
        [Log(OperationActionType.Create, "新增員工")]
        public async Task<IActionResult> Create(StaffInputVM data)
        {
            var result = await _service.CreateOrEdit(data);
            return Ok(result);
        }

        [SwaggerOperation("修改")]
        [HttpPost, Route("Edit")]
        [ValidateModel]
        [PermissionAuthorize((int)PermissionType.員工編輯)]
        [Log(OperationActionType.Create, "修改員工")]
        public async Task<IActionResult> Edit(StaffInputVM data)
        {
            var result = await _service.CreateOrEdit(data);
            return Ok(result);
        }

        [SwaggerOperation("刪除")]
        [HttpDelete,Route("Delete")]
        [PermissionAuthorize((int)PermissionType.員工刪除)]
        [Log(OperationActionType.Delete, "刪除員工列表")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _service.Delete(id);
            return Ok(result);
        }

        [SwaggerOperation("上傳大頭照")]
        [HttpPost,Route("uploadImg")]
        [PermissionAuthorize((int)PermissionType.員工編輯)]
        [Log(OperationActionType.Create, "上傳大頭照")]
        public async Task<IActionResult> uploadImg(UploadImg data)
        {
            var result = await _service.uploadImg(data);
            return Ok(result);
        }

        [SwaggerOperation("取得證照")]
        [HttpGet, Route("GetCertificate")]
        public async Task<IActionResult> GetCertificate(int id)
        {
            var result = await _service.GetCertificate(id);
            return Ok(result);
        }

        [SwaggerOperation("上傳證照")]
        [HttpPost,Route("UploadCertificate")]
        [PermissionAuthorize((int)PermissionType.員工編輯)]
        [Log(OperationActionType.Create, "上傳證照")]
        public async Task<IActionResult> UploadCertificate(UploadCertificate data)
        {
            var result = await _service.UploadCertificate(data);
            return Ok(result);
        }

        [SwaggerOperation("修改證照")]
        [HttpPost,Route("EditCertificate")]
        [PermissionAuthorize((int)PermissionType.員工編輯)]
        [Log(OperationActionType.Edit, "修改證照")]
        public async Task<IActionResult> EditCertificate(EditCertificate data)
        {
            var result = await _service.EditCertificate(data);
            return Ok(result);
        }

        [SwaggerOperation("刪除證照")]
        [HttpDelete, Route("DeleteCertificate")]
        [PermissionAuthorize((int)PermissionType.員工編輯)]
        [Log(OperationActionType.Delete, "刪除證照")]
        public async Task<IActionResult> DeleteCertificate(int id)
        {
            var result = await _service.DeleteCertificate(id);
            return Ok(result);
        }
    }
}
