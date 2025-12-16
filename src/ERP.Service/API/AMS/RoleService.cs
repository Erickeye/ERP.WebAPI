using ERP.EntityModels.Context;
using ERP.EntityModels.Models;
using ERP.Library.Enums;
using ERP.Library.Enums.Login;
using ERP.Library.ViewModels;
using ERP.Library.ViewModels.AMS;
using Microsoft.EntityFrameworkCore;

namespace ERP.Service.API.AMS
{
    public interface IRoleService
    {
        Task<ResultModel<ListResult<RoleViewModel>>> Index();
        Task<ResultModel<string>> Create(Role data);
        Task<ResultModel<string>> Edit(Role data);
        Task<ResultModel<RolePermissions>> GetRolePermissions(int roleId);
        Task<ResultModel<string>> PermissionsEdit(RolePermissions data);
        Task<ResultModel<string>> Delete(int id);
        Task<ResultModel<string>> UpdatePermissionsAmount(List<Level> data);
    }
    public class RoleService : IRoleService
    {
        private readonly ERPDbContext _context;

        public RoleService(ERPDbContext context)
        {
            _context = context;
        }

        public async Task<ResultModel<ListResult<RoleViewModel>>> Index()
        {
            var roleList = await _context.Role
                        .AsNoTracking()
                        .Select( c => new RoleViewModel
                        {
                            Id = c.Id,
                            RoleName = c.RoleName
                        })
                        .ToListAsync();
            var listResult = new ListResult<RoleViewModel> { Items = roleList};

            return ResultModel.Ok(listResult);
        }
        public async Task<ResultModel<string>> Create(Role data)
        {
            _context.Add(data);
            //建立該腳色權限
            //foreach (PermissionType permissionType in Enum.GetValues(typeof(PermissionType)))
            //{
            //    _context.t_permission?.Add(new t_permission
            //    {
            //        f_roleId = data.f_id,
            //        f_pageId = permissionType,
            //        f_type = false,
            //    });
            //}
            await _context.SaveChangesAsync();
            return ResultModel.Ok("角色新增成功");
        }
        public async Task<ResultModel<string>> Edit(Role data)
        {
            var role = await _context.Role.FirstOrDefaultAsync(c => c.Id == data.Id);
            if (role != null)
            {
                _context.Entry(role).CurrentValues.SetValues(data);
                await _context.SaveChangesAsync();
            }
            return ResultModel.Ok("角色修改成功");
        }
        public async Task<ResultModel<RolePermissions>> GetRolePermissions(int roleId)
        {
            var role = await _context.Role.FirstOrDefaultAsync(c => c.Id == roleId);
            if (role == null)
            {
                return ResultModel.Error(ErrorCodeType.UserRoleAlreadyExists);
            }
            var permissions = await _context.Permission.Where(c => c.RoleId == roleId).ToListAsync();

            var permissionsList = new RolePermissions
            {
                RoleId = roleId,
                RoleContent = permissions.Select(c => new RolePermissionItem()
                {
                    PermissionType = (PermissionType)c.PageId,
                })
                .ToList(),
                Permission = role.PermissionLevel,
                Approval = role.ApprovalLevel,
                Quotation = role.QuotationLevel,
                Procurement = role.ProcurementLevel,
            };

            return ResultModel.Ok(permissionsList);
        }
        public async Task<ResultModel<string>> PermissionsEdit(RolePermissions data)
        {
            if (data.RoleContent == null || !data.RoleContent.Any())
            {                
                return ResultModel.Error(ErrorCodeType.IncompleteInfo);
            }

            var permissions = _context.Permission!.Where(p => p.RoleId == data.RoleId).ToList();
            foreach (var item in data.RoleContent)
            {
                var existingPerm = permissions.FirstOrDefault(p => p.PageId == (int)item.PermissionType);
                if (item.HasPermission)
                {
                    // 如果要有權限但資料不存在 → 新增
                    if (existingPerm == null)
                    {
                        _context.Permission!.Add(new Permission
                        {
                            RoleId = data.RoleId,
                            PageId = (int)item.PermissionType,
                        });
                    }
                    // 若已有資料但權限為 false，則改成 true
                    //else if (!existingPerm.f_type)
                    //{
                    //    existingPerm.f_type = true;
                    //}
                }
                else
                {
                    // 如果不需要權限但資料存在 → 刪除
                    if (existingPerm != null)
                    {
                        _context.Permission!.Remove(existingPerm);
                    }
                }
            }

            try
            {
                var level = await _context.Role!.Where(c => c.Id == data.RoleId).FirstOrDefaultAsync();
                if (level == null) {
                    return ResultModel.Error(ErrorCodeType.NotFoundData);
                }
                level.PermissionLevel = data.Permission;
                level.ApprovalLevel = data.Approval;
                level.QuotationLevel = data.Quotation;
                level.ProcurementLevel = data.Procurement;

                _context.Permission!.UpdateRange();
                await _context.SaveChangesAsync();
                //資料序列化                
                return ResultModel.Ok("角色權限已修正");
            }
            catch (Exception ex)
            {
                // 輸出錯誤紀錄檔                
                return ResultModel.Error(ErrorCodeType.Exception, $"更新失敗，{ex.Message}");
            }
        }
        public async Task<ResultModel<string>> UpdatePermissionsAmount(List<Level> data)
        {
            foreach (var level in data)
            {
                var hasLevel = await _context.Level.FirstOrDefaultAsync(c => c.PermissionLevel == level.PermissionLevel);
                if (hasLevel != null)
                {
                    hasLevel.LevelAmount = level.LevelAmount;
                }
            }
            await _context.SaveChangesAsync();
            return ResultModel.Ok("權限金額已修改成功");
        }
        public async Task<ResultModel<string>> Delete(int id)
        {
            var role = await _context.Role.FirstOrDefaultAsync(c => c.Id == id);
            if(role == null)
            {
                return ResultModel.Error(ErrorCodeType.NotFoundData);
            }
            _context.Remove(role);

            await _context.SaveChangesAsync();            
            return ResultModel.Ok("成功刪除角色");
        }
    }
}
