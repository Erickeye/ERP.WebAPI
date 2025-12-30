using ERP.Library.Enums;
using ERP.Library.Enums._1000Company;
using ERP.Library.ViewModels._1000Company;
using ERP.Service.API._1000Company;
using ERP.Service.Helpers;
using ERP.WebAPI.CustomAttributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

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
        [ValidateModel]
        [Log(OperationActionType.Create, "新增請假單")]
        [HttpPost,Route("Create")]
        public async Task<IActionResult> Create(DayOffInputVM data)
        {
            var result = await _service.Create(data);
            return Ok(result);
        }
        [SwaggerOperation("修改請假單")]
        [ValidateModel]
        [Log(OperationActionType.Create, "修改請假單")]
        [HttpPost, Route("Edit")]
        public async Task<IActionResult> Edit(DayOffInputVM data)
        {
            var result = await _service.Edit(data);
            return Ok(result);
        }

        [SwaggerOperation("檢視請假單")]
        [HttpGet,Route("Index")]
        public async Task<IActionResult> Index([FromQuery] DayOffIndexSearch search)
        {
            var result = await _service.Index(search);
            return Ok(result);
        }

        [SwaggerOperation("刪除請假單")]
        [Log(OperationActionType.View, "刪除請假單")]
        [HttpDelete, Route("Delete")]
        public async Task<IActionResult> Delete([SwaggerParameter("請假單號")] int id)
        {
            var result = await _service.Delete(id);
            return Ok(result);
        }
        [SwaggerOperation("取得已請假特休天數")]
        [HttpGet,Route("GetTheYearSpecialLeaveDays")]
        public async Task<IActionResult> GetTheYearSpecialLeaveDays([SwaggerParameter("員工識別碼")] int staffId)
        {
            var result = await _service.GetTheYearSpecialLeaveDays(staffId);
            return Ok(result);
        }
        [SwaggerOperation("取得該年總計特休天數")]
        [HttpGet, Route("GetTheYearSpecialTotalDays")]
        public async Task<IActionResult> GetTheYearSpecialTotalDays([SwaggerParameter("員工識別碼")] int staffId)
        {
            var result = await _service.GetTheYearSpecialTotalDays(staffId);
            return Ok(result);
        }
        [SwaggerOperation("取得今年剩餘特休天數")]
        [HttpGet, Route("GetRemainSpecialDays")]
        public async Task<IActionResult> GetRemainSpecialDays([SwaggerParameter("員工識別碼")] int staffId)
        {
            var result = await _service.GetRemainSpecialDays(staffId);
            return Ok(result);
        }

        [AllowAnonymous]
        [SwaggerOperation("取得假別")]
        [HttpGet, Route("GetLeaveType")]
        public IActionResult GetLeaveType()
        {
            var result = EnumHelper.GetEnumList<LeaveType>();
            return Ok(result);
        }
    }
}
