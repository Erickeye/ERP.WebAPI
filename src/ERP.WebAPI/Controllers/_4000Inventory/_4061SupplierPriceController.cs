using ERP.Library.Enums;
using ERP.Library.Enums.Login;
using ERP.Library.ViewModels._4000Inventory;
using ERP.Service.API._4000Inventory;
using ERP.WebAPI.CustomAttributes;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace ERP.WebAPI.Controllers._4000Inventory
{
    [SwaggerTag("供應商報價")]
    [ApiController]
    [Route("api/[controller]")]
    public class _4061SupplierPriceController : ControllerBase
    {
        private readonly I_4061SupplierPriceService _service;

        public _4061SupplierPriceController(I_4061SupplierPriceService service)
        {
            _service = service;
        }

        [SwaggerOperation("查詢某供應商的報價列表")]
        [PermissionAuthorize((int)PermissionType.供應商檢視)]
        [HttpGet, Route("Index")]
        public async Task<IActionResult> Index([FromQuery] SupplierPriceSearchVM vm)
        {
            var result = await _service.Index(vm);
            return Ok(result);
        }

        [SwaggerOperation("取得供應商報價")]
        [PermissionAuthorize((int)PermissionType.供應商檢視)]
        [HttpGet, Route("Get")]
        public async Task<IActionResult> Get([SwaggerParameter("供應商報價流水號")] int id)
        {
            var result = await _service.Get(id);
            return Ok(result);
        }

        [SwaggerOperation("新增供應商報價")]
        [PermissionAuthorize((int)PermissionType.供應商編輯)]
        [ValidateModel]
        [HttpPost, Route("Add")]
        [Log(OperationActionType.Create, "新增供應商報價")]
        public async Task<IActionResult> Add(SupplierPriceAddVM vm)
        {
            var result = await _service.Add(vm);
            return Ok(result);
        }

        [SwaggerOperation("修改供應商報價")]
        [PermissionAuthorize((int)PermissionType.供應商編輯)]
        [ValidateModel]
        [HttpPost, Route("Edit")]
        [Log(OperationActionType.Edit, "修改供應商報價")]
        public async Task<IActionResult> Edit(SupplierPriceAddVM vm)
        {
            var result = await _service.Edit(vm);
            return Ok(result);
        }

        [SwaggerOperation("刪除供應商報價")]
        [PermissionAuthorize((int)PermissionType.供應商刪除)]
        [HttpDelete, Route("Delete")]
        [Log(OperationActionType.Delete, "刪除供應商報價")]
        public async Task<IActionResult> Delete([SwaggerParameter("供應商報價流水號")] int id)
        {
            var result = await _service.Delete(id);
            return Ok(result);
        }
    }
}
