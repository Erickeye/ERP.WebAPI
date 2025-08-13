using ERP.Library.Enums;
using ERP.Library.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Text.Json;

namespace ERP.WebAPI.CustomAttributes
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true)]
    public class PermissionAuthorizeAttribute : Attribute, IAuthorizationFilter
    {
        private readonly int[] _requiredPermissions;

        public PermissionAuthorizeAttribute(params int[] permissions)
        {
            _requiredPermissions = permissions;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var result = new ResultModel<string>();

            var permissionJson = context.HttpContext.User.FindFirst("permissions")?.Value;
            if (permissionJson == "[]" || string.IsNullOrEmpty(permissionJson))
            {
                result.SetError(ErrorCodeType.PermissionDenied, "沒有權限，請先設定該角色權限");
                context.Result = new JsonResult(result)
                {
                    StatusCode = StatusCodes.Status403Forbidden
                };
                return;
            }

            List<int> userPermissions = JsonSerializer.Deserialize<List<int>>(permissionJson!) ?? new List<int>();


            // 檢查是否有任何一個必須的權限
            if (!_requiredPermissions.Any(rp => userPermissions.Contains(rp)))
            {
                result.SetError(ErrorCodeType.PermissionDenied);

                context.Result = new JsonResult(result)
                {
                    StatusCode = StatusCodes.Status403Forbidden
                };
                return;
            }
        }
    }
}
