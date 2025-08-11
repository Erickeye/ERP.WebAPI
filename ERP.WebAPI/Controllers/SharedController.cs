using ERP.Data;
using ERP.Library.Enums;
using ERP.Library.Enums._1000Company;
using ERP.Library.ViewModels;
using ERP.Service.API.Shared;
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
        public SharedController(ISharedService service)
        {
            _service = service;
        }

        [SwaggerOperation("取得血型種類")]
        [HttpGet, Route("GetBloodType")]
        public IActionResult GetBloodType()
        {
            var result = _service.GetEnumList<BloodType>();
            return Ok(result);
        }

        [SwaggerOperation("取得結婚狀態")]
        [HttpGet, Route("GetMarriageStatus")]
        public IActionResult GetMarriageStatus()
        {
            var result = _service.GetEnumList<MarriageStatus>();
            return Ok(result);
        }

        [SwaggerOperation("取得性別")]
        [HttpGet, Route("GetGender")]
        public IActionResult GetGender()
        {
            var result = _service.GetEnumList<Gender>();
            return Ok(result);
        }

        [SwaggerOperation("取得工作狀態")]
        [HttpGet, Route("GetJobStatus")]
        public IActionResult GetJobStatus()
        {
            var result = _service.GetEnumList<JobStatus>();
            return Ok(result);
        }

        [SwaggerOperation("取得假別")]
        [HttpGet, Route("GetLeaveType")]
        public IActionResult GetLeaveType()
        {
            var result = _service.GetEnumList<LeaveType>();
            return Ok(result);
        }

        [SwaggerOperation("取得加班日")]
        [HttpGet, Route("GetOverTimeType")]
        public IActionResult GetOverTimeType()
        {
            var result = _service.GetEnumList<OverTimeType>();
            return Ok(result);
        }


        [SwaggerOperation("取得文件速別")]
        [HttpGet, Route("GetDocumentLevelType")]
        public IActionResult GetDocumentLevelType()
        {
            var result = _service.GetEnumList<DocumentLevelType>();
            return Ok(result);
        }
    }
}
