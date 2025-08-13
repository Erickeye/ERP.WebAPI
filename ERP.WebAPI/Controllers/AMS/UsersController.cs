using ERP.Library.Enums;
using ERP.Library.ViewModels;
using ERP.Library.Extensions;
using ERP.Models.AMS;
using ERP.WebAPI.CustomAttributes;
using ERP.Service.API.AMS;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Annotations;
using Microsoft.Extensions.Primitives;
using ERP.Library.ViewModels.AMS;

namespace ERP.WebAPI.Controllers.AMS
{
    [SwaggerTag("使用者")]
    [ApiController]
    [Route("api/[controller]")]
    [ApiExplorerSettings(GroupName = "Default API")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _service;

        public UsersController(IUserService service)
        {
            _service = service;
        }

        [SwaggerOperation("檢視")]
        [HttpGet,Route("Index")]
        [Log(OperationActionType.View, "檢視使用者")]
        public async Task<IActionResult> Index(string? inputUser)
        {
            var result = new ResultModel<ListResult<UserViewModel>>();
            result = await _service.Index(inputUser!);
            return Ok(result);
        }

        [SwaggerOperation("新增")]
        [HttpPost,Route("Create")]
        [ValidateModel]
        [Log(OperationActionType.Create, "新增使用者")]
        public async Task<IActionResult> Create(t_user data)
        {
            var result = await _service.Create(data);
            return Ok(result);
        }

        [SwaggerOperation("修改")]
        [HttpPost,Route("Edit")]
        [ValidateModel]
        [Log(OperationActionType.Edit, "修改使用者")]
        public async Task<IActionResult> Edit(t_user data)
        {
            var result = await _service.Edit(data);
            return Ok(result);
        }

        [SwaggerOperation("刪除")]
        [HttpDelete,Route("Delete")]
        [Log(OperationActionType.Edit, "刪除使用者")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _service.Delete(id);
            return Ok(result);
        }
    }
}
