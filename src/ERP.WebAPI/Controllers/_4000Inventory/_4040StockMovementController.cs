using ERP.Library.Enums;
using ERP.Library.Enums.Login;
using ERP.Library.ViewModels._4000Inventory;
using ERP.Service.API._4000Inventory;
using ERP.WebAPI.CustomAttributes;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace ERP.WebAPI.Controllers._4000Inventory
{
    [SwaggerTag("庫存異動")]
    [ApiController]
    [Route("api/[controller]")]
    public class _4040StockMovementController : ControllerBase
    {
        private readonly I_4040StockMovementService _service;

        public _4040StockMovementController(I_4040StockMovementService service)
        {
            _service = service;
        }

        [SwaggerOperation("查詢庫存異動紀錄")]
        [PermissionAuthorize((int)PermissionType.庫存檢視)]
        [HttpGet, Route("Index")]
        public async Task<IActionResult> Index([FromQuery] StockMovementSearchVM vm)
        {
            var result = await _service.Index(vm);
            return Ok(result);
        }
    }
}
