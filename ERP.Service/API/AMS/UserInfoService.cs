using ERP.Data;
using ERP.Library.Enums;
using ERP.Library.ViewModels;
using ERP.Library.ViewModels.UserInfo;
using ERP.Models.AMS;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Service.API.AMS
{
    public interface IUserInfoService
    {
        Task<ResultModel<UserInfo>> GetUserInfo();
    }
    public class UserInfoService : IUserInfoService
    {
        private readonly ERPContext _context;
        private readonly IHttpContextAccessor httpContextAccessor;

        public UserInfoService(ERPContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            this.httpContextAccessor = httpContextAccessor;
        }
        public async Task<ResultModel<UserInfo>> GetUserInfo()
        {
            var result = new ResultModel<UserInfo>();
            var userIdString = httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdString, out int userId))
            {
                result.SetError(ErrorCodeType.Unauthorized);
                return result;
            }
            var userInfo = await (
                from user in _context.User
                join role in _context.Role
                on (int)user.RoleId equals role.Id
                select new UserInfo
                {
                    UserId = userId,
                    UserName = user.Name,
                    Account = user.Account,
                    RoleId = role.Id,
                    RoleName = role.RoleName,
                })
                .FirstOrDefaultAsync();

            if (userInfo == null)
            {
                result.SetError(ErrorCodeType.UserNotFound);
            }
            result.Data = userInfo;
            return result;
        }
    }
}
