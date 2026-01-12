using ERP.Library.ViewModels;
using ERP.Library.ViewModels._4000Inventory;
using ERP.Service.API._4000Inventory;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace ERP.WebAPI.Controllers._4000Inventory
{
    [SwaggerTag("庫存")]
    [ApiController]
    [Route("api/[controller]")]
    public class _4000InventoryController : ControllerBase
    {
        private readonly I_4000InventoryService _service;
        public _4000InventoryController(I_4000InventoryService service)
        {
            _service = service;
        }
        [SwaggerOperation("檢視庫存列表")]
        [HttpGet, Route("Index")]
        public async Task<IActionResult> Index([FromQuery] InventorySearchVM vm)
        {
            var result = await _service.Index(vm);
            return Ok(result);
        }
    }
}
