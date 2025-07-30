using ERP.Library.Enums;
using ERP.Library.ViewModels;
using ERP.Library.ViewModels._1000Company;
using ERP.Service.API._1000Company;
using ERP.WebAPI.CustomAttributes;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace ERP.WebAPI.Controllers._1000Company
{
    [SwaggerTag("員工")]
    [ApiController]
    [Route("api/[controller]")]
    [ApiExplorerSettings(GroupName = "_1000Company")]
    public class _1000StaffController : ControllerBase
    {
        private readonly I_1000Service _service;

        public _1000StaffController(I_1000Service service)
        {
            _service = service;
        }

        [SwaggerOperation("檢視員工列表")]
        [HttpGet, Route("Index")]
        [Log(OperationActionType.View, "檢視員工列表")]
        public async Task<ResultModel<List<StaffIndex>>> Index(string? deptID, bool isResignation = false)
        {
            var result = await _service.GetStaffIndex(deptID, isResignation);
            return result;
        }
    }
}
