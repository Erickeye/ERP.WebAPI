using ERP.EntityModels.Models._1000Company;
using ERP.Library.Enums;
using ERP.Library.ViewModels._1000Company;
using ERP.Service.API._1000Company;
using ERP.WebAPI.CustomAttributes;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ERP.WebAPI.Controllers._1000Company
{
    [SwaggerTag("請假單")]
    [ApiController]
    [Route("api/[controller]")]
    [ApiExplorerSettings(GroupName = "_1000Company")]
    public class _1030DayOffController : ControllerBase
    {
        private readonly I_1030DayoffService _service;
        public _1030DayOffController(I_1030DayoffService service)
        {
            _service = service;
        }

        [SwaggerOperation("新增請假單")]
        [Log(OperationActionType.Create, "新增請假單")]
        [HttpPost,Route("Create")]
        public async Task<IActionResult> Create(DayOffInputVM data)
        {
            var result = await _service.CreateOrEdit(data);
            return Ok(result);
        }
        [SwaggerOperation("修改請假單")]
        [Log(OperationActionType.Create, "修改請假單")]
        [HttpPost, Route("Edit")]
        public async Task<IActionResult> Edit(DayOffInputVM data)
        {
            var result = await _service.CreateOrEdit(data);
            return Ok(result);
        }

        [SwaggerOperation("檢視請假單")]
        [Log(OperationActionType.View , "檢視請假單")]
        [HttpGet,Route("Index")]
        public async Task<IActionResult> Index()
        {
            var result = await _service.Index();
            return Ok(result);
        }

        [SwaggerOperation("刪除請假單")]
        [Log(OperationActionType.View, "刪除請假單")]
        [HttpDelete, Route("Delete")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _service.Delete(id);
            return Ok(result);
        }
    }
}
