using ERP.Data;
using ERP.Library.Enums;
using ERP.Library.Helpers;
using ERP.Library.ViewModels;
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
        Task<ResultModel<string>> Create(t_user data);
        Task<ResultModel<string>> Edit(t_user data);
    }
    public class UserService : IUserService
    {        
        private readonly AMSContext _context;

        public UserService(AMSContext context)
        {
            _context = context;
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
            if (user != null) { 
                _context.Entry(user).CurrentValues.SetValues(data);
            }
            result.SetSuccess("資料成功修改");
            return result;
        }
    }
}
