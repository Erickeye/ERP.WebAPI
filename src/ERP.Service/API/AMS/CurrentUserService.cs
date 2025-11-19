using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Service.API.AMS
{
    public interface ICurrentUserService
    {
        int UserId { get; }
        string? UserName { get; }
        int RoleId { get; }
        string? Role { get; }
    }
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public int UserId
        {
            get
            {
                var value = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                return int.TryParse(value, out var id) ? id : 0;
            }
        }

        public string? UserName =>
            _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Name)?.Value;

        public int RoleId
        {
            get
            {
                var value = _httpContextAccessor.HttpContext?.User?.FindFirst("RoleId")?.Value;
                return int.TryParse(value, out var id) ? id : 0;
            }
        }

        public string? Role =>
            _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Role)?.Value;
    }
}
