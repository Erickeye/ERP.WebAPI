using ERP.Library.Enums;
using ERP.Library.Enums.Login;
using ERP.Library.ViewModels._1000Company;
using ERP.Service.API._1000Company;
using ERP.WebAPI.CustomAttributes;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace ERP.WebAPI.Controllers._1000Company
{
    [SwaggerTag("公司")]
    [ApiController]
    [Route("api/[controller]")]
    public class _1080CompanyController : ControllerBase
    {
        private readonly I_1080CompanyService _service;

        public _1080CompanyController(I_1080CompanyService service)
        {
            _service = service;
        }

        [SwaggerOperation("查詢公司列表")]
        [PermissionAuthorize((int)PermissionType.公司檢視)]
        [HttpGet, Route("Index")]
        public async Task<IActionResult> Index([FromQuery] CompanySearchVM vm)
        {
            var result = await _service.Index(vm);
            return Ok(result);
        }

        [SwaggerOperation("取得公司")]
        [PermissionAuthorize((int)PermissionType.公司檢視)]
        [HttpGet, Route("Get")]
        public async Task<IActionResult> Get([SwaggerParameter("公司流水號")] int id)
        {
            var result = await _service.Get(id);
            return Ok(result);
        }

        // 填單用選單不要求「公司檢視」：只要能編輯領料單或出差單就要選得到公司
        [SwaggerOperation("取得填單用的公司選單（只列啟用中；includeId 為單據原本的公司，已停用也一併回傳）")]
        [PermissionAuthorize((int)PermissionType.領料單編輯, (int)PermissionType.出差單)]
        [HttpGet, Route("GetSelect")]
        public async Task<IActionResult> GetSelect([SwaggerParameter("單據原本的公司流水號")] int? includeId)
        {
            var result = await _service.GetSelect(includeId);
            return Ok(result);
        }

        [SwaggerOperation("新增公司")]
        [PermissionAuthorize((int)PermissionType.公司編輯)]
        [ValidateModel]
        [HttpPost, Route("Add")]
        [Log(OperationActionType.Create, "新增公司")]
        public async Task<IActionResult> Add(CompanyAddVM vm)
        {
            var result = await _service.Add(vm);
            return Ok(result);
        }

        [SwaggerOperation("修改公司")]
        [PermissionAuthorize((int)PermissionType.公司編輯)]
        [ValidateModel]
        [HttpPost, Route("Edit")]
        [Log(OperationActionType.Edit, "修改公司")]
        public async Task<IActionResult> Edit(CompanyAddVM vm)
        {
            var result = await _service.Edit(vm);
            return Ok(result);
        }

        [SwaggerOperation("停用公司")]
        [PermissionAuthorize((int)PermissionType.公司編輯)]
        [HttpPost, Route("Disable")]
        [Log(OperationActionType.Edit, "停用公司")]
        public async Task<IActionResult> Disable([SwaggerParameter("公司流水號")] int id)
        {
            var result = await _service.Disable(id);
            return Ok(result);
        }

        [SwaggerOperation("啟用公司")]
        [PermissionAuthorize((int)PermissionType.公司編輯)]
        [HttpPost, Route("Enable")]
        [Log(OperationActionType.Edit, "啟用公司")]
        public async Task<IActionResult> Enable([SwaggerParameter("公司流水號")] int id)
        {
            var result = await _service.Enable(id);
            return Ok(result);
        }

        [SwaggerOperation("刪除公司")]
        [PermissionAuthorize((int)PermissionType.公司刪除)]
        [HttpDelete, Route("Delete")]
        [Log(OperationActionType.Delete, "刪除公司")]
        public async Task<IActionResult> Delete([SwaggerParameter("公司流水號")] int id)
        {
            var result = await _service.Delete(id);
            return Ok(result);
        }
    }
}
