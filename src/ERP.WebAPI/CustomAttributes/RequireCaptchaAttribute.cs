using ERP.WebAPI.Filters;
using Microsoft.AspNetCore.Mvc;

namespace ERP.WebAPI.CustomAttributes
{
    /// <summary>
    /// 標示該 API 需要通過驗證碼驗證。
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public sealed class RequireCaptchaAttribute : TypeFilterAttribute
    {
        public RequireCaptchaAttribute() : base(typeof(RequireCaptchaFilter))
        {
        }
    }
}
