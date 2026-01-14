using ERP.Library.Enums;
using ERP.Library.ViewModels._4000Inventory;
using ERP.Service.API._4000Inventory;
using ERP.WebAPI.CustomAttributes;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace ERP.WebAPI.Controllers._4000Inventory
{
    [SwaggerTag("庫存")]
    [ApiController]
    [Route("api/[controller]")]
    public class _4010PurchaseController : ControllerBase
    {
        private readonly I_4010PurchaseService _service;

        public _4010PurchaseController(I_4010PurchaseService service)
        {
            _service = service;
        }

        [SwaggerOperation("查詢進貨單列表")]
        [HttpGet, Route("Index")]
        public async Task<IActionResult> Index( [FromQuery] PurchaseSearchVM vm)
        {
            var result = await _service.Index(vm);
            return Ok(result);
        }

        [SwaggerOperation("取得進貨單")]
        [HttpGet, Route("Get")]
        public async Task<IActionResult> Get( [SwaggerParameter] int id)
        {
            var result = await _service.Get(id);
            return Ok(result);
        }

        [SwaggerOperation("建立進貨單")]
        [ValidateModel]
        [HttpPost, Route("Add")]
        [Log(OperationActionType.Create, "建立進貨單")]
        public async Task<IActionResult> Add(PurchaseAddVM vm)
        {
            var result = await _service.Add(vm);
            return Ok(result);
        }

        [SwaggerOperation("修改進貨單")]
        [ValidateModel]
        [HttpPost, Route("Edit")]
        [Log(OperationActionType.Create, "修改進貨單")]
        public async Task<IActionResult> Edit(PurchaseAddVM vm)
        {
            var result = await _service.Edit(vm);
            return Ok(result);
        }
    }
}
