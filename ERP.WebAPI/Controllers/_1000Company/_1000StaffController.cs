using ERP.EntityModels.Models._1000Company;
using ERP.Library.Enums;
using ERP.Library.ViewModels;
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
    [ApiExplorerSettings(GroupName = "_1000Company")]
    public class _1000StaffController : ControllerBase
    {
        private readonly I_1000Service _service;

        public _1000StaffController(I_1000Service service)
        {
            _service = service;
        }

        [SwaggerOperation("取得員工列表")]
        [HttpGet, Route("Index")]
        public async Task<ResultModel<List<StaffIndex>>> Index(string? deptID, bool isResignation = false)
        {
            var result = await _service.GetStaffIndex(deptID, isResignation);
            return result;
        }

        [SwaggerOperation("新增或修改")]
        [HttpPost, Route("Create")]
        public async Task<ResultModel<string>> Create(t_1000Staff data)
        {
            var result = await _service.CreateOrEdit(data);
            // 檢查 ModelState 是否有效
            if (!ModelState.IsValid)
            {
                var errorMessages = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                result.SetError(ErrorCodeType.FieldValueIsInvalid, string.Join("; ", errorMessages));
                return result;
            }

            return result;
        }

        [SwaggerOperation("刪除")]
        [HttpPost,Route("Delete")]
        public async Task<ResultModel<string>> Delete(int id)
        {
            var result = await _service.Delete(id);
            return result;
        }

        [SwaggerOperation("上傳大頭照")]
        [HttpPost,Route("uploadImg")]
        public async Task<ResultModel<string>> uploadImg(uploadImg data)
        {
            var result = await _service.uploadImg(data);
            return result;
        }

        [SwaggerOperation("上傳證照")]
        [HttpPost,Route("UploadCertificate")]
        public async Task<ResultModel<string>> UploadCertificate(UploadCertificate data)
        {
            var result = await _service.UploadCertificate(data);
            return result;
        }
    }
}
