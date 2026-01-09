using ERP.Library.Enums._1000Company;
using ERP.Library.Extensions;
using ERP.Service.API;
using ERP.Service.API._1000Company;
using ERP.Service.API._2000Customer;
using ERP.Service.API._4000Inventory;
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
        private readonly I_2000CustomerService _2000SCustomerService;
        private readonly I_4060SupplierService _4060SupplierService;
        public SharedController(ISharedService service, I_1000Service _1000Service, I_2000CustomerService _2000SCustomerService, I_4060SupplierService _4060SupplierService)
        {
            _service = service;
            this._1000Service = _1000Service;
            this._2000SCustomerService = _2000SCustomerService;
            this._4060SupplierService = _4060SupplierService;
        }

        [SwaggerOperation("取得員工下拉選單")]
        [HttpGet, Route("GetStaffSelect")]
        public async Task<IActionResult> GetStaffSelect()
        {
            var result = await _1000Service.GetStaffSelect();
            return Ok(result);
        }
        [SwaggerOperation("取得客戶下拉選單")]
        [HttpGet, Route("GetCustomerSelect")]
        public async Task<IActionResult> GetCustomerSelect()
        {
            var result = await _2000SCustomerService.GetCustomerSelect();
            return Ok(result);
        }
        [SwaggerOperation("取得供應商下拉選單")]
        [HttpGet, Route("GetSupplierSelect")]
        public async Task<IActionResult> GetSupplierSelect()
        {
            var result = await _4060SupplierService.GetSupplierSelect();
            return Ok(result);
        }

        [SwaggerOperation("取得庫存位下拉選單")]
        [HttpGet, Route("GetInventorySelect")]
        public async Task<IActionResult> GetInventorySelect()
        {
            var result = await _service.GetInventorySelect();
            return Ok(result);
        }

        [SwaggerOperation("取得付款方式下拉選單")]
        [HttpGet, Route("GetPaymentMethodSelect")]
        public async Task<IActionResult> GetPaymentMethodSelect()
        {
            var result = await _service.GetPaymentMethodSelect();
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
