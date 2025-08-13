using ERP.Library.Enums;
using ERP.Library.ViewModels._2000Customer;
using ERP.Service.API._2000Customer;
using ERP.WebAPI.CustomAttributes;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace ERP.WebAPI.Controllers._2000Customer
{
    [SwaggerTag("客戶聯絡人")]
    [ApiController]
    [Route("api/[controller]")]
    [ApiExplorerSettings(GroupName = "_2000Costomer")]
    public class _2010CustemployController : ControllerBase
    {
        private readonly I_2010CustemployService _service;

        public _2010CustemployController(I_2010CustemployService service)
        {
            _service = service;
        }

        [SwaggerOperation("檢視客戶聯絡人列表")]
        [HttpGet, Route("Index")]
        [Log(OperationActionType.View, "檢視客戶聯絡人列表")]
        public async Task<IActionResult> Index(int customerId)
        {
            var result = await _service.Index(customerId);
            return Ok(result);
        }

        [SwaggerOperation("檢視客戶聯絡人資料")]
        [HttpGet, Route("Get")]
        [Log(OperationActionType.View, "檢視客戶聯絡人資料")]
        public async Task<IActionResult> Get(int id)
        {
            var result = await _service.Get(id);
            return Ok(result);
        }

        [SwaggerOperation("新增客戶聯絡人資料")]
        [HttpPost, Route("Create")]
        [Log(OperationActionType.Create, "新增客戶聯絡人資料")]
        public async Task<IActionResult> Create(CustemployInputVM data)
        {
            var result = await _service.CreateOrEdit(data);
            return Ok(result);
        }

        [SwaggerOperation("修改客戶聯絡人資料")]
        [HttpPost, Route("Edit")]
        [Log(OperationActionType.Edit, "修改客戶聯絡人")]
        public async Task<IActionResult> Edit(CustemployInputVM data)
        {
            var result = await _service.CreateOrEdit(data);
            return Ok(result);
        }

        [SwaggerOperation("刪除客戶聯絡人資料")]
        [HttpPost, Route("Delete")]
        [Log(OperationActionType.Delete, "刪除客戶聯絡人")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _service.Delete(id);
            return Ok(result);
        }
    }
}
