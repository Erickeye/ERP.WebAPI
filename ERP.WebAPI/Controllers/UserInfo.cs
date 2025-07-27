using ERP.Service.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace ERP.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserInfo : ControllerBase
    {
        private IUserService _userService;

        public UserInfo(IUserService userService)
        {
            _userService = userService;
        }

        [SwaggerOperation("取得使用者資訊")]
        [HttpGet, Route("GetUserInfo")]
        public async Task<IActionResult> GetUserInfo(int userId)
        {
            var result = await _userService.GetUserInfo(userId);
            return Ok(result);
        }
    }
}
