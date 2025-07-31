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
        public async Task<ResultModel<ListResult<StaffIndex>>> Index(string? deptID, bool isResignation = false)
        {
            var result = await _service.GetStaffIndex(deptID, isResignation);
            return result;
        }

        [SwaggerOperation("新增")]
        [HttpPost, Route("Create")]
        [Log(OperationActionType.Create, "新增員工")]
        public async Task<ResultModel<string>> Create(t_1000Staff data)
        {
            var result = new ResultModel<string>();
            // 檢查 ModelState 是否有效
            if (!ModelState.IsValid)
            {
                result.SetError(ErrorCodeType.FieldValueIsInvalid, ModelState.GetAllErrorMessagesAsString());
                return result;
            }
            result = await _service.CreateOrEdit(data);

            return result;
        }

        [SwaggerOperation("修改")]
        [HttpPost, Route("Edit")]
        [Log(OperationActionType.Create, "修改員工")]
        public async Task<ResultModel<string>> Edit(t_1000Staff data)
        {
            var result = new ResultModel<string>();
            // 檢查 ModelState 是否有效
            if (!ModelState.IsValid)
            {
                result.SetError(ErrorCodeType.FieldValueIsInvalid, ModelState.GetAllErrorMessagesAsString());
                return result;
            }
            result = await _service.CreateOrEdit(data);

            return result;
        }

        [SwaggerOperation("刪除")]
        [HttpPost,Route("Delete")]
        [Log(OperationActionType.Delete, "刪除員工列表")]
        public async Task<ResultModel<string>> Delete(int id)
        {
            var result = await _service.Delete(id);
            return result;
        }

        [SwaggerOperation("上傳大頭照")]
        [HttpPost,Route("uploadImg")]
        [Log(OperationActionType.Create, "上傳大頭照")]
        public async Task<ResultModel<string>> uploadImg(uploadImg data)
        {
            var result = await _service.uploadImg(data);
            return result;
        }

        [SwaggerOperation("取得證照")]
        [HttpGet, Route("GetCertificate")]
        public async Task<ResultModel<string>> GetCertificate(int id)
        {
            var result = await _service.GetCertificate(id);
            return result;
        }

        [SwaggerOperation("上傳證照")]
        [HttpPost,Route("UploadCertificate")]
        [Log(OperationActionType.Create, "上傳證照")]
        public async Task<ResultModel<string>> UploadCertificate(UploadCertificate data)
        {
            var result = await _service.UploadCertificate(data);
            return result;
        }

        [SwaggerOperation("修改證照")]
        [HttpPost,Route("EditCertificate")]
        [Log(OperationActionType.Edit, "修改證照")]
        public async Task<ResultModel<string>> EditCertificate(EditCertificate data)
        {
            var result = await _service.EditCertificate(data);
            return result;
        }

        [SwaggerOperation("刪除證照")]
        [HttpPost, Route("DeleteCertificate")]
        [Log(OperationActionType.Delete, "刪除證照")]
        public async Task<ResultModel<string>> DeleteCertificate(int id)
        {
            var result = await _service.DeleteCertificate(id);
            return result;
        }
    }
}
