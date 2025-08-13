using ERP.Data;
using ERP.Library.Enums;
using ERP.Library.Enums.Login;
using ERP.Library.Helpers;
using ERP.Library.ViewModels;
using ERP.Library.ViewModels.AMS;
using ERP.Library.ViewModels.UserInfo;
using ERP.Models.AMS;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
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
    }
    public class UserService : IUserService
    {
        private readonly ERPContext _context;
        private readonly ICurrentUserService _currentUserService;

        public UserService(ERPContext context, ICurrentUserService currentUserService)
        {
            _context = context;
            _currentUserService = currentUserService;
        }

        public async Task<ResultModel<ListResult<UserViewModel>>> Index(string? inputUser)
        {
            var result = new ResultModel<ListResult<UserViewModel>>();

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
            var listResult = new ListResult<UserViewModel> { Items = viewModel };
            result.SetSuccess(listResult);

            return result;
        }

        public async Task<ResultModel<string>> Create(User data)
        {
            var result = new ResultModel<string>();

            data.CreateDate = DateTime.Now;
            data.Pwd = PasswordHelper.HashPassword(data.Account);

            var checkAccount = await _context.User.FirstOrDefaultAsync(c => c.Account == data.Account);
            //重複註冊
            if (checkAccount != null)
            {
                result.SetError(ErrorCodeType.UserAlreadyExists);
            }
            _context.Add(data);
            return result;
        }
        public async Task<ResultModel<string>> Edit(User data)
        {
            var result = new ResultModel<string>();

            var user = await _context.User.FirstOrDefaultAsync(c => c.Id == data.Id);
            if (user != null)
            {
                _context.Entry(user).CurrentValues.SetValues(data);
            }
            result.SetSuccess("資料成功修改");
            return result;
        }
        public async Task<ResultModel<string>> Delete(int id)
        {
            var result = new ResultModel<string>();
            var user = await _context.User.FirstOrDefaultAsync(c => c.Id == id);
            if (user == null)
            {
                result.SetError(ErrorCodeType.NotFoundData);
                return result;
            }
            if (user!.Name == "管理員")
            {
                result.SetError(0, "無法刪除管理員");
                return result;
            }
            _context.Remove(user);
            await _context.SaveChangesAsync();
            return result;
        }
    }
}
