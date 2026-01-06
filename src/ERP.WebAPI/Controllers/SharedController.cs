using ERP.Library.Enums._1000Company;
using ERP.Library.Extensions;
using ERP.Service.API._1000Company;
using ERP.Service.API.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace ERP.WebAPI.Controllers
{
    [SwaggerTag("共用方法")]
    [Route("api/Shared")]
    [ApiController]
    [ApiExplorerSettings(GroupName = "Default API")]
    public class SharedController : ControllerBase
    {

        private readonly ISharedService _service;
        private readonly I_1000Service _1000Service;
        public SharedController(ISharedService service, I_1000Service _1000Service)
        {
            _service = service;
            this._1000Service = _1000Service;
        }

        [SwaggerOperation("取得員工下拉選單")]
        [HttpGet, Route("GetStaffSelect")]
        public async Task<IActionResult> GetStaffSelect()
        {
            var result = await _1000Service.GetStaffSelect();
            return Ok(result);
        }

        [AllowAnonymous]
        [SwaggerOperation("取得血型種類")]
        [HttpGet, Route("GetBloodType")]
        public IActionResult GetBloodType()
        {
            var result = default(BloodType).ToEnumList();
            return Ok(result);
        }

        [AllowAnonymous]
        [SwaggerOperation("取得結婚狀態")]
        [HttpGet, Route("GetMarriageStatus")]
        public IActionResult GetMarriageStatus()
        {
            var result = default(MarriageStatus).ToEnumList();
            return Ok(result);
        }

        [AllowAnonymous]
        [SwaggerOperation("取得性別")]
        [HttpGet, Route("GetGender")]
        public IActionResult GetGender()
        {
            var result = default(Gender).ToEnumList();
            return Ok(result);
        }

        [AllowAnonymous]
        [SwaggerOperation("取得工作狀態")]
        [HttpGet, Route("GetJobStatus")]
        public IActionResult GetJobStatus()
        {
            var result = default(JobStatus).ToEnumList();
            return Ok(result);
        }

        [AllowAnonymous]
        [SwaggerOperation("取得假別")]
        [HttpGet, Route("GetLeaveType")]
        public IActionResult GetLeaveType()
        {
            var result = default(LeaveType).ToEnumList();
            return Ok(result);
        }

        [AllowAnonymous]
        [SwaggerOperation("取得加班日")]
        [HttpGet, Route("GetOverTimeType")]
        public IActionResult GetOverTimeType()
        {
            var result = default(OverTimeType).ToEnumList();
            return Ok(result);
        }


        [AllowAnonymous]
        [SwaggerOperation("取得文件速別")]
        [HttpGet, Route("GetDocumentLevelType")]
        public IActionResult GetDocumentLevelType()
        {
            var result = default(DocumentLevelType).ToEnumList();
            return Ok(result);
        }
    }
}
