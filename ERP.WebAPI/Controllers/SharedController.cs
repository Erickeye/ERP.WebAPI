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

        private static readonly Dictionary<string, Type> EnumMapping = new()
        {
            ["BloodType"] = typeof(BloodType),
            ["MarriageStatus"] = typeof(MarriageStatus),
            ["Gender"] = typeof(Gender),
            ["JobStatus"] = typeof(JobStatus),
            ["LeaveType"] = typeof(LeaveType),
            ["OverTimeType"] = typeof(OverTimeType),
            ["DocumentLevelType"] = typeof(DocumentLevelType),
        };

        [HttpGet,Route("{enumName}")]
        public IActionResult GetEnumList(string enumName)
        {
            if (!EnumMapping.TryGetValue(enumName, out var enumType))
            {
                return NotFound();
            }

            // 利用反射呼叫泛型方法
            var method = typeof(ISharedService).GetMethod("GetEnumList")!;
            var genericMethod = method.MakeGenericMethod(enumType);
            var result = genericMethod.Invoke(_service, null) as ResultModel<List<SelectModel>>;

            if (result == null)
            {
                result.SetError(ErrorCodeType.NotFoundData);
            }

            return Ok(result);
        }

        //[SwaggerOperation("取得血型種類")]
        //[HttpGet,Route("GetBloodType")]
        //public IActionResult GetBloodType() {
        //    var result = _service.GetBloodType();
        //    return Ok(result);
        //}

        //[SwaggerOperation("取得結婚狀態")]
        //[HttpGet,Route("GetMarriageStatus")]
        //public IActionResult GetMarriageStatus()
        //{
        //    var result = _service.GetMarriageStatus();
        //    return Ok(result);
        //}

        //[SwaggerOperation("取得性別")]
        //[HttpGet, Route("GetGender")]
        //public IActionResult GetGender()
        //{
        //    var result = _service.GetGender();
        //    return Ok(result);
        //}

        //[SwaggerOperation("取得工作狀態")]
        //[HttpGet, Route("GetJobStatus")]
        //public IActionResult GetJobStatus()
        //{
        //    var result = _service.GetJobStatus();
        //    return Ok(result);
        //}

        //[SwaggerOperation("取得假別")]
        //[HttpGet, Route("GetLeaveType")]
        //public IActionResult GetLeaveType()
        //{
        //    var result = _service.GetLeaveType();
        //    return Ok(result);
        //}

        //[SwaggerOperation("取得加班日")]
        //[HttpGet, Route("GetOverTimeType")]
        //public IActionResult GetOverTimeType()
        //{
        //    var result = _service.GetOverTimeType();
        //    return Ok(result);
        //}


        //[SwaggerOperation("取得文件速別")]
        //[HttpGet, Route("GetDocumentLevelType")]
        //public IActionResult GetDocumentLevelType()
        //{
        //    var result = _service.GetDocumentLevelType();
        //    return Ok(result);
        //}
    }
}
