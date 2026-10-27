using ERP.Library.Enums;
using ERP.Library.Enums.Login;
using ERP.Library.ViewModels._4000Inventory;
using ERP.Service.API._4000Inventory;
using ERP.WebAPI.CustomAttributes;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace ERP.WebAPI.Controllers._4000Inventory
{
    [SwaggerTag("供應商")]
    [ApiController]
    [Route("api/[controller]")]
    public class _4060SupplierController : ControllerBase
    {
        private readonly I_4060SupplierService _service;

        public _4060SupplierController(I_4060SupplierService service)
        {
            _service = service;
        }

        [SwaggerOperation("查詢供應商列表")]
        [PermissionAuthorize((int)PermissionType.供應商檢視)]
        [HttpGet, Route("Index")]
        public async Task<IActionResult> Index([FromQuery] SupplierSearchVM vm)
        {
            var result = await _service.Index(vm);
            return Ok(result);
        }

        [SwaggerOperation("取得供應商")]
        [PermissionAuthorize((int)PermissionType.供應商檢視)]
        [HttpGet, Route("Get")]
        public async Task<IActionResult> Get([SwaggerParameter("供應商流水號")] int id)
        {
            var result = await _service.Get(id);
            return Ok(result);
        }

        // 填單用選單不要求「供應商檢視」：只要能編輯進貨單、庫存或合約單就要選得到供應商（合約單檢視者篩選列表也會用到）
        [SwaggerOperation("取得填單用的供應商選單（只列啟用中；includeId 為單據原本的供應商，已停用也一併回傳）")]
        [PermissionAuthorize((int)PermissionType.進貨單編輯, (int)PermissionType.庫存編輯, (int)PermissionType.合約單編輯, (int)PermissionType.合約單檢視)]
        [HttpGet, Route("GetSelect")]
        public async Task<IActionResult> GetSelect([SwaggerParameter("單據原本的供應商流水號")] int? includeId)
        {
            var result = await _service.GetSelect(includeId);
            return Ok(result);
        }

        [SwaggerOperation("新增供應商")]
        [PermissionAuthorize((int)PermissionType.供應商編輯)]
        [ValidateModel]
        [HttpPost, Route("Add")]
        [Log(OperationActionType.Create, "新增供應商")]
        public async Task<IActionResult> Add(SupplierAddVM vm)
        {
            var result = await _service.Add(vm);
            return Ok(result);
        }

        [SwaggerOperation("修改供應商")]
        [PermissionAuthorize((int)PermissionType.供應商編輯)]
        [ValidateModel]
        [HttpPost, Route("Edit")]
        [Log(OperationActionType.Edit, "修改供應商")]
        public async Task<IActionResult> Edit(SupplierAddVM vm)
        {
            var result = await _service.Edit(vm);
            return Ok(result);
        }

        [SwaggerOperation("停用供應商")]
        [PermissionAuthorize((int)PermissionType.供應商編輯)]
        [HttpPost, Route("Disable")]
        [Log(OperationActionType.Edit, "停用供應商")]
        public async Task<IActionResult> Disable([SwaggerParameter("供應商流水號")] int id)
        {
            var result = await _service.Disable(id);
            return Ok(result);
        }

        [SwaggerOperation("啟用供應商")]
        [PermissionAuthorize((int)PermissionType.供應商編輯)]
        [HttpPost, Route("Enable")]
        [Log(OperationActionType.Edit, "啟用供應商")]
        public async Task<IActionResult> Enable([SwaggerParameter("供應商流水號")] int id)
        {
            var result = await _service.Enable(id);
            return Ok(result);
        }

        [SwaggerOperation("刪除供應商")]
        [PermissionAuthorize((int)PermissionType.供應商刪除)]
        [HttpDelete, Route("Delete")]
        [Log(OperationActionType.Delete, "刪除供應商")]
        public async Task<IActionResult> Delete([SwaggerParameter("供應商流水號")] int id)
        {
            var result = await _service.Delete(id);
            return Ok(result);
        }
    }
}
