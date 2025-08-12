using ERP.EntityModels.Models._1000Company;
using ERP.Library.Enums;
using ERP.Library.ViewModels;
using ERP.Library.ViewModels._1000Company;
using ERP.Library.Extensions;
using ERP.Service.API._1000Company;
using ERP.WebAPI.CustomAttributes;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace ERP.WebAPI.Controllers._1000Company
{
    [SwaggerTag("員工")]
    [ApiController]
    [Route("api/[controller]")]
    [ApiExplorerSettings(GroupName = "_1000Company")]
    public class _1000StaffController : ControllerBase
    {
        private readonly I_1000Service _service;

        public _1000StaffController(I_1000Service service)
        {
            _service = service;
        }

        [SwaggerOperation("檢視員工列表")]
        [HttpGet, Route("Index")]
        [Log(OperationActionType.View, "檢視員工列表")]
        public async Task<IActionResult> Index(string? deptID, bool isResignation = false)
        {
            var result = await _service.GetStaffIndex(deptID!, isResignation);
            return Ok(result);
        }

        [SwaggerOperation("新增")]
        [HttpPost, Route("Create")]
        [Log(OperationActionType.Create, "新增員工")]
        public async Task<IActionResult> Create(t_1000Staff data)
        {
            var result = new ResultModel<string>();
            // 檢查 ModelState 是否有效
            if (!ModelState.IsValid)
            {
                result.SetError(ErrorCodeType.FieldValueIsInvalid, ModelState.GetAllErrorMessagesAsString());
                return Ok(result);
            }
            result = await _service.CreateOrEdit(data);

            return Ok(result);
        }

        [SwaggerOperation("修改")]
        [HttpPost, Route("Edit")]
        [Log(OperationActionType.Create, "修改員工")]
        public async Task<IActionResult> Edit(t_1000Staff data)
        {
            var result = new ResultModel<string>();
            // 檢查 ModelState 是否有效
            if (!ModelState.IsValid)
            {
                result.SetError(ErrorCodeType.FieldValueIsInvalid, ModelState.GetAllErrorMessagesAsString());
                return Ok(result);
            }
            result = await _service.CreateOrEdit(data);

            return Ok(result);
        }

        [SwaggerOperation("刪除")]
        [HttpDelete,Route("Delete")]
        [Log(OperationActionType.Delete, "刪除員工列表")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _service.Delete(id);
            return Ok(result);
        }

        [SwaggerOperation("上傳大頭照")]
        [HttpPost,Route("uploadImg")]
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
        [Log(OperationActionType.Create, "上傳證照")]
        public async Task<IActionResult> UploadCertificate(UploadCertificate data)
        {
            var result = await _service.UploadCertificate(data);
            return Ok(result);
        }

        [SwaggerOperation("修改證照")]
        [HttpPost,Route("EditCertificate")]
        [Log(OperationActionType.Edit, "修改證照")]
        public async Task<IActionResult> EditCertificate(EditCertificate data)
        {
            var result = await _service.EditCertificate(data);
            return Ok(result);
        }

        [SwaggerOperation("刪除證照")]
        [HttpDelete, Route("DeleteCertificate")]
        [Log(OperationActionType.Delete, "刪除證照")]
        public async Task<IActionResult> DeleteCertificate(int id)
        {
            var result = await _service.DeleteCertificate(id);
            return Ok(result);
        }
    }
}
