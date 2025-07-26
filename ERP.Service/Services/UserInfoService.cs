using ERP.Data;
using ERP.Library.ViewModels.UserInfo;
using ERP.Models.AMS;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLRIMOA.Library.Enums;
using TLRIMOA.Library.ViewModels;
using static ERP.Service.Services.UserService;

namespace ERP.Service.Services
{
    public interface IUserService
    {
        Task<ResultModel<UserInfo>> GetUserInfo(int userId);
    }
    public class UserService : IUserService
    {
        private readonly AMSContext _context;

        public UserService(AMSContext context)
        {
            _context = context;
        }
        public async Task<ResultModel<UserInfo>> GetUserInfo(int userId)
        {
            var result = new ResultModel<UserInfo>();

            var userInfo = await (
                from user in _context.t_user
                join role in _context.t_role
                on (int)user.f_role equals role.f_id
                where (user.f_id == role.f_id)
                select new UserInfo
                {
                    UserId = userId,
                    UserName = user.f_name,
                    Account = user.f_account,
                    RoleId = role.f_id,
                    RoleName = role.f_roleName,
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
