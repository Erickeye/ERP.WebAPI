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

        [SwaggerOperation("新增")]
        [HttpPost,Route("Create")]
        [Log(OperationActionType.Create, "新增使用者")]
        public async Task<ResultModel<string>> Create(t_user data)
        {
            var result = new ResultModel<string>();
            if (!ModelState.IsValid)
            {
                result.SetError(ErrorCodeType.FieldValueIsInvalid, ModelState.GetAllErrorMessagesAsString());
                return result;
            }
            result = await _service.Create(data);
            return result;
        }

        [SwaggerOperation("修改")]
        [HttpPost,Route("Edit")]
        [Log(OperationActionType.Edit, "修改使用者")]
        public async Task<ResultModel<string>> Edit(t_user data)
        {
            var result = new ResultModel<string>();
            if (!ModelState.IsValid)
            {
                result.SetError(ErrorCodeType.FieldValueIsInvalid, ModelState.GetAllErrorMessagesAsString());
                return result;
            }
            result = await _service.Edit(data);
            return result;
        }
    }
}
