using ERP.Library.Enums;
using ERP.Library.ViewModels;
using ERP.Library.ViewModels._4000Inventory;
using ERP.Service.API;
using ERP.Service.API._4000Inventory;
using ERP.WebAPI.CustomAttributes;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace ERP.WebAPI.Controllers._4000Inventory
{
    [SwaggerTag("進貨單")]
    [ApiController]
    [Route("api/[controller]")]
    public class _4010PurchaseController : ControllerBase
    {
        private readonly I_4010PurchaseService _service;
        private readonly ISharedService _sharedService;

        public _4010PurchaseController(I_4010PurchaseService service, ISharedService sharedService)
        {
            _service = service;
            _sharedService = sharedService;
        }

        [SwaggerOperation("查詢進貨單列表")]
        [HttpGet, Route("Index")]
        public async Task<IActionResult> Index( [FromQuery] PurchaseSearchVM vm)
        {
            var result = await _service.Index(vm);
            return Ok(result);
        }

        [SwaggerOperation("取得進貨單")]
        [HttpGet, Route("Get")]
        public async Task<IActionResult> Get( [SwaggerParameter] int id)
        {
            var result = await _service.Get(id);
            return Ok(result);
        }

        [SwaggerOperation("建立進貨單")]
        [ValidateModel]
        [HttpPost, Route("Add")]
        [Log(OperationActionType.Create, "建立進貨單")]
        public async Task<IActionResult> Add(PurchaseAddVM vm)
        {
            var result = await _service.Add(vm);
            return Ok(result);
        }

        [SwaggerOperation("修改進貨單")]
        [ValidateModel]
        [HttpPost, Route("Edit")]
        [Log(OperationActionType.Create, "修改進貨單")]
        public async Task<IActionResult> Edit(PurchaseAddVM vm)
        {
            var result = await _service.Edit(vm);
            return Ok(result);
        }

        [SwaggerOperation("取得庫存位下拉選單")]
        [HttpGet, Route("GetInventorySelect")]
        public async Task<IActionResult> GetInventorySelect()
        {
            var result = await _sharedService.GetInventorySelect();
            return Ok(result);
        }

        [SwaggerOperation("送出簽核進貨單")]
        [ValidateModel]
        [HttpPost, Route("SendApproval")]
        [Log(OperationActionType.Approval, "送出簽核進貨單")]
        public async Task<IActionResult> SendApproval(ApprovalVM vm)
        {
            var result = await _service.SendApproval(vm);
            return Ok(result);
        }

        [SwaggerOperation("簽核進貨單")]
        [ValidateModel]
        [HttpPost, Route("Approval")]
        [Log(OperationActionType.Approval, "簽核進貨單")]
        public async Task<IActionResult> Approval(ApprovalVM vm)
        {
            var result = await _service.Approval(vm);
            return Ok(result);
        }

        [SwaggerOperation("撤銷簽核進貨單")]
        [ValidateModel]
        [HttpPost, Route("VoidApproval")]
        [Log(OperationActionType.RevokeApproval, "修改進貨單")]
        public async Task<IActionResult> VoidApproval(ApprovalVM vm)
        {
            var result = await _service.VoidApproval(vm);
            return Ok(result);
        }
    }
}
