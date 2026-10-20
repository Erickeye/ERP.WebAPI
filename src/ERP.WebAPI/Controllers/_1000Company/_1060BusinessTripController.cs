using ERP.Library.Enums;
using ERP.Library.Enums.Login;
using ERP.Library.ViewModels;
using ERP.Library.ViewModels._1000Company;
using ERP.Service.API._1000Company;
using ERP.WebAPI.CustomAttributes;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace ERP.WebAPI.Controllers._1000Company
{
    [SwaggerTag("出差單")]
    [ApiController]
    [Route("api/[controller]")]
    public class _1060BusinessTripController : ControllerBase
    {
        private readonly I_1060BusinessTripService _service;

        public _1060BusinessTripController(I_1060BusinessTripService service)
        {
            _service = service;
        }

        // 列表與單筆不加功能權限：簽核人不需要額外權限，可見範圍由 Service 依登入者與簽核紀錄限縮
        [SwaggerOperation("查詢出差單列表")]
        [HttpGet, Route("Index")]
        public async Task<IActionResult> Index([FromQuery] BusinessTripSearchVM vm)
        {
            var result = await _service.Index(vm);
            return Ok(result);
        }

        [SwaggerOperation("取得出差單")]
        [HttpGet, Route("Get")]
        public async Task<IActionResult> Get([SwaggerParameter("出差單流水號")] int id)
        {
            var result = await _service.Get(id);
            return Ok(result);
        }

        [SwaggerOperation("取得目前使用者對應的員工（預設出差人）")]
        [PermissionAuthorize((int)PermissionType.出差單)]
        [HttpGet, Route("GetCurrentStaff")]
        public async Task<IActionResult> GetCurrentStaff()
        {
            var result = await _service.GetCurrentStaff();
            return Ok(result);
        }

        [SwaggerOperation("建立出差單")]
        [PermissionAuthorize((int)PermissionType.出差單)]
        [ValidateModel]
        [HttpPost, Route("Add")]
        [Log(OperationActionType.Create, "建立出差單")]
        public async Task<IActionResult> Add(BusinessTripAddVM vm)
        {
            var result = await _service.Add(vm);
            return Ok(result);
        }

        [SwaggerOperation("修改出差單")]
        [PermissionAuthorize((int)PermissionType.出差單)]
        [ValidateModel]
        [HttpPost, Route("Edit")]
        [Log(OperationActionType.Edit, "修改出差單")]
        public async Task<IActionResult> Edit(BusinessTripAddVM vm)
        {
            var result = await _service.Edit(vm);
            return Ok(result);
        }

        [SwaggerOperation("刪除出差單")]
        [PermissionAuthorize((int)PermissionType.出差單)]
        [HttpDelete, Route("Delete")]
        [Log(OperationActionType.Delete, "刪除出差單")]
        public async Task<IActionResult> Delete([SwaggerParameter("出差單流水號")] int id)
        {
            var result = await _service.Delete(id);
            return Ok(result);
        }

        [SwaggerOperation("送出簽核出差單")]
        [PermissionAuthorize((int)PermissionType.出差單)]
        [ValidateModel]
        [HttpPost, Route("SendApproval")]
        [Log(OperationActionType.Approval, "送出簽核出差單")]
        public async Task<IActionResult> SendApproval(ApprovalVM vm)
        {
            var result = await _service.SendApproval(vm);
            return Ok(result);
        }

        // 核准與拒絕不加功能權限：由簽核紀錄決定能否簽核（引擎檢查是否輪到），簽核人不必是建單人或出差人
        [SwaggerOperation("簽核出差單")]
        [ValidateModel]
        [HttpPost, Route("Approval")]
        [Log(OperationActionType.Approval, "簽核出差單")]
        public async Task<IActionResult> Approval(ApprovalVM vm)
        {
            var result = await _service.Approval(vm);
            return Ok(result);
        }

        [SwaggerOperation("拒絕簽核出差單")]
        [ValidateModel]
        [HttpPost, Route("RejectApproval")]
        [Log(OperationActionType.Approval, "拒絕簽核出差單")]
        public async Task<IActionResult> RejectApproval(ApprovalVM vm)
        {
            var result = await _service.RejectApproval(vm);
            return Ok(result);
        }

        [SwaggerOperation("撤銷簽核出差單")]
        [PermissionAuthorize((int)PermissionType.出差單)]
        [ValidateModel]
        [HttpPost, Route("RevokeApproval")]
        [Log(OperationActionType.RevokeApproval, "撤銷簽核出差單")]
        public async Task<IActionResult> RevokeApproval(ApprovalVM vm)
        {
            var result = await _service.RevokeApproval(vm);
            return Ok(result);
        }

        // 作廢不加功能權限：由簽核紀錄決定（引擎檢查是否為最後一關簽核人），最後一關簽核人不必是建單人或出差人
        [SwaggerOperation("作廢出差單")]
        [ValidateModel]
        [HttpPost, Route("VoidApproval")]
        [Log(OperationActionType.RevokeApproval, "作廢出差單")]
        public async Task<IActionResult> VoidApproval(ApprovalVM vm)
        {
            var result = await _service.VoidApproval(vm);
            return Ok(result);
        }
    }
}
