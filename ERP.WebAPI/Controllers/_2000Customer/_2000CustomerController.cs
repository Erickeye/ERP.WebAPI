using ERP.Library.Enums;
using ERP.Service.API._2000Customer;
using ERP.WebAPI.CustomAttributes;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace ERP.WebAPI.Controllers._2000Customer
{
    [SwaggerTag("客戶")]
    [ApiController]
    [Route("api/[controller]")]
    [ApiExplorerSettings(GroupName = "_2000Costomer")]
    public class _2000CustomerController : ControllerBase
    {
        private readonly I_2000CustomerService _service;

        public _2000CustomerController(I_2000CustomerService service)
        {
            _service = service;
        }

        [SwaggerOperation("檢視客戶列表")]
        [HttpGet,Route("Index")]
        [Log(OperationActionType.View, "檢視客戶列表")]
        public async Task<IActionResult> Index()
        {
            var result = await _service.Index();
            return Ok(result);
        }
    }
}
