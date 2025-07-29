using ERP.Library.ViewModels.Login;
using ERP.Service.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Security.Claims;
// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ERP.WebAPI.Controllers
{
    [SwaggerTag("登入作業")]
    [ApiController]
    [Route("api/[controller]")]
    [ApiExplorerSettings(GroupName = "Default API")]
    public class LoginController : ControllerBase
    {
        private readonly ILoginService _authService;

        public LoginController(ILoginService authService)
        {
            _authService = authService;
        }

        [AllowAnonymous]
        [SwaggerOperation("登入")]
        [HttpPost, Route("Login")]
        public async Task<IActionResult> Login(LoginRequest login)
        {
            var result = await _authService.AuthenticateAsync(login.Account, login.Password);
            return Ok(result);
        }
   
        [SwaggerOperation("刷新Token")]
        [HttpPost, Route("Refresh")]
        public async Task<IActionResult> Refresh(AccessRefreshToken token)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            var result = await _authService.RefreshTokenAsync(token);
            return Ok(result);
        }

        [SwaggerOperation("登出")]
        [HttpPost, Route("Logout")]
        public async Task<IActionResult> Logout()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var result = await _authService.Logout(userIdClaim);
            return Ok(result);
        }
    }
}
