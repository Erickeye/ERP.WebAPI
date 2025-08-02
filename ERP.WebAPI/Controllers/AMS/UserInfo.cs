using ERP.Service.API.AMS;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace ERP.WebAPI.Controllers.AMS
{
    [ApiController]
    [Route("api/[controller]")]
    [ApiExplorerSettings(GroupName = "Default API")]
    public class UserInfo : ControllerBase
    {
        private IUserInfoService _userInfoService;

        public UserInfo(IUserInfoService userInfoService)
        {
            _userInfoService = userInfoService;
        }

        [SwaggerOperation("取得使用者資訊")]
        [HttpGet, Route("GetUserInfo")]
        public async Task<IActionResult> GetUserInfo()
        {
            var result = await _userInfoService.GetUserInfo();
            return Ok(result);
        }
    }
}
