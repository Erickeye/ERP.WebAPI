using ERP.Models.AMS;
using ERP.Service.Services;
using ERP.Library.ViewModels.Login;
using Microsoft.AspNetCore.Mvc;
using TLRIMOA.Library.ViewModels;
// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ERP.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly IAuthService _authService;

        public LoginController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> AuthenticateAsync(string account, string password)
        {
            var result = await _authService.AuthenticateAsync(account, password);
            return Ok(result);
        }

        // GET: api/<Login>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<Login>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<Login>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<Login>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<Login>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
