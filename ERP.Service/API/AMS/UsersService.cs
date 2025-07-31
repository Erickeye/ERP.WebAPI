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
        Task<ResultModel<string>> Create(t_user data);
        Task<ResultModel<string>> Edit(t_user data);
        Task<ResultModel<string>> Delete(int id);
    }
    public class UserService : IUserService
    {
        private readonly AMSContext _context;
        private readonly ICurrentUserService _currentUserService;

        public UserService(AMSContext context, ICurrentUserService currentUserService)
        {
            _context = context;
            _currentUserService = currentUserService;
        }

        public async Task<ResultModel<ListResult<UserViewModel>>> Index(string? inputUser)
        {
            var result = new ResultModel<ListResult<UserViewModel>>();

            var userName = _currentUserService.UserName;
            var userRole = _currentUserService.Role;

            //當不是管理者時只顯示自己的帳號
            var userQuery = _context.t_user!.AsQueryable();
            if (userRole != nameof(RoleType.管理者))
            {
                userQuery = _context.t_user.Where(c => c.f_name == userName);
            }
            //搜尋inputUser
            if (!string.IsNullOrEmpty(inputUser))
            {
                userQuery = _context.t_user.Where(c => c.f_name.Contains(inputUser));
            }
            var viewModel = await userQuery.Join(
                    _context.t_role!,
                    user => (int)user.f_role,
                    role => role.f_id,
                    (user, role) => new UserViewModel
                    {
                        Id = user.f_id,
                        Name = user.f_name,
                        Account = user.f_account,
                        RoleName = role.f_roleName
                    })
                .ToListAsync();
            var listResult = new ListResult<UserViewModel> { Items =  viewModel };
            result.SetSuccess(listResult);

            return result;
        }

        public async Task<ResultModel<string>> Create(t_user data)
        {
            var result = new ResultModel<string>();

            data.f_createDate = DateTime.Now;
            data.f_pwd = PasswordHelper.HashPassword(data.f_account);

            var checkAccount = await _context.t_user.FirstOrDefaultAsync(c => c.f_account == data.f_account);
            //重複註冊
            if (checkAccount != null)
            {
                result.SetError(ErrorCodeType.UserAlreadyExists);
            }
            _context.Add(data);
            return result;
        }
        public async Task<ResultModel<string>> Edit(t_user data)
        {
            var result = new ResultModel<string>();

            var user = await _context.t_user.FirstOrDefaultAsync();
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
            var user = _context.t_user.FirstOrDefault(c => c.f_id == id);
            if (user != null) {
                if(user.f_name == "管理員")
                {
                    result.SetError(0,"無法刪除管理員");
                }
                _context.Remove(user);
                await _context.SaveChangesAsync();
            }
            else
            {
                result.SetError(ErrorCodeType.NotFoundData);
            }
            return result;
        } 
    }
}
