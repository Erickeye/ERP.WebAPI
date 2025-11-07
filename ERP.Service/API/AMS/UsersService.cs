using ERP.EntityModels.Context;
using ERP.Library.Enums;
using ERP.Library.Enums.Login;
using ERP.Library.Helpers;
using ERP.Library.ViewModels;
using ERP.Library.ViewModels.AMS;
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
    public interface IUserService
    {
        Task<ResultModel<ListResult<UserViewModel>>> Index(string inputUser);
        Task<ResultModel<string>> Create(User data);
        Task<ResultModel<string>> Edit(User data);
        Task<ResultModel<string>> Delete(int id);
        Task<ResultModel<UserInfo>> GetUserInfo();
    }
    public class UserService : IUserService
    {
        private readonly ERPContext _context;
        private readonly ICurrentUserService _currentUserService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserService(ERPContext context, ICurrentUserService currentUserService, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _currentUserService = currentUserService;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<ResultModel<ListResult<UserViewModel>>> Index(string? inputUser)
        {
            var userName = _currentUserService.UserName;
            var userRoleId = _currentUserService.RoleId;

            //當不是管理者時只顯示自己的帳號
            var userQuery = _context.User!.AsQueryable();
            if (userRoleId != 0)
            {
                userQuery = _context.User.Where(c => c.Name == userName);
            }
            //搜尋inputUser
            if (!string.IsNullOrEmpty(inputUser))
            {
                userQuery = _context.User.Where(c => c.Name.Contains(inputUser));
            }
            var viewModel = await userQuery.Join(
                    _context.Role!,
                    user => (int)user.RoleId,
                    role => role.Id,
                    (user, role) => new UserViewModel
                    {
                        Id = user.Id,
                        Name = user.Name,
                        Account = user.Account,
                        RoleName = role.RoleName
                    })
                .ToListAsync();

            return ResultModel.Ok(viewModel);
        }

        public async Task<ResultModel<string>> Create(User data)
        {
            data.CreateDate = DateTime.Now;
            data.Pwd = PasswordHelper.HashPassword(data.Account);

            var checkAccount = await _context.User.FirstOrDefaultAsync(c => c.Account == data.Account);
            //重複註冊
            if (checkAccount != null)
            {
                return ResultModel.Error(ErrorCodeType.UserAlreadyExists);
            }
            _context.Add(data);
            await _context.SaveChangesAsync();
            return ResultModel.Ok("資料成功新增");
        }
        public async Task<ResultModel<string>> Edit(User data)
        {
            var user = await _context.User.FirstOrDefaultAsync(c => c.Id == data.Id);
            if (user != null)
            {
                _context.Entry(user).CurrentValues.SetValues(data);
                await _context.SaveChangesAsync();
            }
            return ResultModel.Ok("資料成功修改");
        }
        public async Task<ResultModel<string>> Delete(int id)
        {
            var user = await _context.User.FirstOrDefaultAsync(c => c.Id == id);
            if (user == null)
            {
                return ResultModel.Error(ErrorCodeType.NotFoundData);
            }
            if (user!.Name == "管理員")
            {
                return ResultModel.Error(0, "無法刪除管理員");
            }
            _context.Remove(user);
            await _context.SaveChangesAsync();
            return ResultModel.Ok("刪除成功");
        }

        public async Task<ResultModel<UserInfo>> GetUserInfo()
        {
            var userIdString = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdString, out int userId))
            {
                return ResultModel.Error(ErrorCodeType.Unauthorized);
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
                return ResultModel.Error(ErrorCodeType.UserNotFound);
            }
            return ResultModel.Ok(userInfo);
        }
    }
}
