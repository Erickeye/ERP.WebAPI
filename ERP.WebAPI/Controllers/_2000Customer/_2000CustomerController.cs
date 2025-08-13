using ERP.Library.Enums;
using ERP.Library.ViewModels;
using ERP.Library.ViewModels._2000Customer;
using ERP.Service.API._2000Customer;
using ERP.WebAPI.CustomAttributes;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using ERP.Library.Extensions;

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

        [SwaggerOperation("檢視客戶資料")]
        [HttpGet, Route("Get")]
        [Log(OperationActionType.View, "檢視客戶資料")]
        public async Task<IActionResult> Get(int id)
        {
            var result = await _service.Get(id);
            return Ok(result);
        }

        [SwaggerOperation("新增客戶資料")]
        [HttpPost,Route("Create")]
        [ValidateModel]
        [Log(OperationActionType.Create, "新增客戶資料")]
        public async Task<IActionResult> Create(CustomerInputVM data)
        {
            var result = await _service.CreateOrEdit(data);
            return Ok(result);
        }

        [SwaggerOperation("修改客戶資料")]
        [HttpPost, Route("Edit")]
        [ValidateModel]
        [Log(OperationActionType.Edit, "修改客戶")]
        public async Task<IActionResult> Edit(CustomerInputVM data)
        {
            var result = await _service.CreateOrEdit(data);
            return Ok(result);
        }

        [SwaggerOperation("刪除客戶資料")]
        [HttpPost, Route("Delete")]
        [Log(OperationActionType.Delete, "刪除客戶")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _service.Delete(id);
            return Ok(result);
        }
    }
}
