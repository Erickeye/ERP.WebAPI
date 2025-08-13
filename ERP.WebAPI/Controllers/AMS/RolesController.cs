using Azure;
using ERP.Library.Enums;
using ERP.Library.ViewModels;
using ERP.Library.ViewModels.AMS;
using ERP.Models.AMS;
using ERP.Service.API.AMS;
using ERP.WebAPI.CustomAttributes;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using ERP.Library.Extensions;

namespace ERP.WebAPI.Controllers.AMS
{
    [SwaggerTag("角色")]
    [ApiController]
    [Route("api/[controller]")]
    [ApiExplorerSettings(GroupName = "Default API")]
    public class RolesController : ControllerBase
    {
        private readonly IRoleService _servsice;

        public RolesController(IRoleService servsice)
        {
            _servsice = servsice;
        }

        [SwaggerOperation("檢視")]
        [HttpGet,Route("Index")]
        [Log(OperationActionType.View,"檢視角色表單")]
        public async Task<IActionResult> Index()
        {
            var result = await _servsice.Index();
            return Ok(result);
        }

        [SwaggerOperation("新增名稱")]
        [HttpPost, Route("Create")]
        [ValidateModel]
        [Log(OperationActionType.Create, "新增角色名稱")]
        public async Task<IActionResult> Create(Role data)
        {
            var result = await _servsice.Create(data);
            return Ok(result);
        }

        [SwaggerOperation("修改名稱")]
        [HttpPost, Route("Edit")]
        [ValidateModel]
        [Log(OperationActionType.Edit, "修改角色名稱")]
        public async Task<IActionResult> Edit(Role data)
        {
            var result = await _servsice.Edit(data);
            return Ok(result);
        }

        [SwaggerOperation("檢視角色權限")]
        [HttpPost, Route("GetRolePermissions")]
        [Log(OperationActionType.View, "檢視角色權限")]
        public async Task<IActionResult> GetRolePermissions(int roleId)
        {
            var result = await _servsice.GetRolePermissions(roleId);
            return Ok(result);
        }

        [SwaggerOperation("修改角色權限")]
        [HttpPost, Route("PermissionsEdit")]
        [Log(OperationActionType.Edit, "修改角色權限")]
        public async Task<IActionResult> PermissionsEdit(RolePermissions data)
        {
            var result = await _servsice.PermissionsEdit(data);
            return Ok(result);
        }

        [SwaggerOperation("修改權限金額")]
        [HttpPost, Route("UpdatePermissionsAmount")]
        [Log(OperationActionType.Edit, "修改權限金額")]
        public async Task<IActionResult> UpdatePermissionsAmount(List<Level> data)
        {
            var result = await _servsice.UpdatePermissionsAmount(data);
            return Ok(result);
        }

        [SwaggerOperation("刪除")]
        [HttpDelete, Route("Delete")]
        [Log(OperationActionType.Delete, "刪除")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _servsice.Delete(id);
            return Ok(result);
        }
    }
}
