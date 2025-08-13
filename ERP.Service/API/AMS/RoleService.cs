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
using Newtonsoft.Json;

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
        private readonly ERPContext _context;

        public RoleService(ERPContext context)
        {
            _context = context;
        }

        public async Task<ResultModel<ListResult<RoleViewModel>>> Index()
        {
            var result = new ResultModel<ListResult<RoleViewModel>>();
            var roleList = await _context.Role
                        .AsNoTracking()
                        .Select( c => new RoleViewModel
                        {
                            Id = c.Id,
                            RoleName = c.RoleName
                        })
                        .ToListAsync();
            var listResult = new ListResult<RoleViewModel> { Items = roleList};
            result.Data = listResult;

            return result;
        }
        public async Task<ResultModel<string>> Create(Role data)
        {
            var result = new ResultModel<string>();
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
            result.SetSuccess("角色新增成功");
            return result;
        }
        public async Task<ResultModel<string>> Edit(Role data)
        {
            var result = new ResultModel<string>();
            var role = await _context.Role.FirstOrDefaultAsync(c => c.Id == data.Id);
            if (role != null)
            {
                _context.Entry(role).CurrentValues.SetValues(data);
                await _context.SaveChangesAsync();
            }
            result.SetSuccess("角色修改成功");
            return result;
        }
        public async Task<ResultModel<RolePermissions>> GetRolePermissions(int roleId)
        {
            var result = new ResultModel<RolePermissions>();
            var role = await _context.Role.FirstOrDefaultAsync(c => c.Id == roleId);
            if (role == null)
            {
                result.SetError(ErrorCodeType.UserRoleAlreadyExists);
                return result;
            }
            var permissions = await _context.Permission.Where(c => c.RoleId == roleId).ToListAsync();

            var permissionsList = new RolePermissions
            {
                RoleId = roleId,
                RoleContent = permissions.Select(c => new RolePermissionItem()
                {
                    PermissionType = c.PageId,
                })
                .ToList(),
                Permission = role.PermissionLevel,
                Approval = role.ApprovalLevel,
                Quotation = role.QuotationLevel,
                Procurement = role.ProcurementLevel,
            };

            result.Data = permissionsList;

            return result;
        }
        public async Task<ResultModel<string>> PermissionsEdit(RolePermissions data)
        {
            var result = new ResultModel<string>();
            if (data.RoleContent == null || !data.RoleContent.Any())
            {
                result.SetError(ErrorCodeType.IncompleteInfo);
                return result;
            }

            var permissions = _context.Permission!.Where(p => p.RoleId == data.RoleId).ToList();
            foreach (var item in data.RoleContent)
            {
                var existingPerm = permissions.FirstOrDefault(p => p.PageId == item.PermissionType);
                if (item.HasPermission)
                {
                    // 如果要有權限但資料不存在 → 新增
                    if (existingPerm == null)
                    {
                        _context.Permission!.Add(new Permission
                        {
                            RoleId = data.RoleId,
                            PageId = item.PermissionType,
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
                    result.SetError(ErrorCodeType.NotFoundData);
                    return result;
                }
                level.PermissionLevel = data.Permission;
                level.ApprovalLevel = data.Approval;
                level.QuotationLevel = data.Quotation;
                level.ProcurementLevel = data.Procurement;

                _context.Permission!.UpdateRange();
                await _context.SaveChangesAsync();
                //資料序列化
                result.SetSuccess("角色權限已修正");
            }
            catch (Exception ex)
            {
                // 輸出錯誤紀錄檔
                result.SetError(ErrorCodeType.Exception, $"更新失敗，{ex.Message}");
                return result;
            }
            return result;
        }
        public async Task<ResultModel<string>> UpdatePermissionsAmount(List<Level> data)
        {
            var result = new ResultModel<string>();
            foreach (var level in data)
            {
                var hasLevel = await _context.Level.FirstOrDefaultAsync(c => c.PermissionLevel == level.PermissionLevel);
                if (hasLevel != null)
                {
                    hasLevel.LevelAmount = level.LevelAmount;
                }
            }
            result.SetSuccess("權限金額已修改成功");
            await _context.SaveChangesAsync();
            return result;
        }
        public async Task<ResultModel<string>> Delete(int id)
        {
            var result = new ResultModel<string>();
            var role = await _context.Role.FirstOrDefaultAsync(c => c.Id == id);
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
