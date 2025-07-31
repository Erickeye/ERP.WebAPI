using ERP.Library.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ERP.Data;
using ERP.Library.ViewModels.AMS;
using Microsoft.EntityFrameworkCore;
using ERP.Models.AMS;
using ERP.Library.Enums;
using ERP.Library.Enums.Login;

namespace ERP.Service.API.AMS
{
    public class RoleService
    {
        private readonly AMSContext _context;

        public RoleService(AMSContext context)
        {
            _context = context;
        }

        public async Task<ResultModel<ListResult<RoleViewModel>>> Index()
        {
            var result = new ResultModel<ListResult<RoleViewModel>>();
            var roleList = await _context.t_role
                        .AsNoTracking()
                        .Select( c => new RoleViewModel
                        {
                            Id = c.f_id,
                            RoleName = c.f_roleName
                        })
                        .ToListAsync();
            var listResult = new ListResult<RoleViewModel> { Items = roleList};
            result.Data = listResult;

            return result;
        }
        public async Task<ResultModel<string>> Create(t_user data)
        {
            var result = new ResultModel<string>();
            _context.Add(data);
            await _context.SaveChangesAsync();
            //建立該腳色權限
            //foreach(PermissionType permissionType in Enum.GetValues(typeof(PermissionType)))
            //{
            //    _context.t_permission?.Add(new t_permission
            //    {
            //        f_roleId = data.f_id,
            //        f_pageId = permissionType,
            //        f_type = false,
            //    });
            //}
            result.SetSuccess("角色新增成功");
            return result;
        }
        public async Task<ResultModel<string>> Edit(t_role data)
        {
            var reuslt = new ResultModel<string>();
            var role = await _context.t_role.FirstOrDefaultAsync(c => c.f_id == data.f_id);
            if (role != null)
            {
                _context.Entry(role).CurrentValues.SetValues(data);
                await _context.SaveChangesAsync();
            }

            return reuslt;
        }
        public async Task<ResultModel<string>> Delete(int id)
        {
            var result = new ResultModel<string>();
            var role = await _context.t_role.FirstOrDefaultAsync(c => c.f_id == id);
            if(role == null)
            {
                result.SetError(ErrorCodeType.NotFoundData);
                return result;
            }
            _context.Remove(role);
            await _context.SaveChangesAsync();
            result.SetSuccess("成功刪除角色");

            return result;
        }
    }
}
