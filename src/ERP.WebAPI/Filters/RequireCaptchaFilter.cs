using ERP.Service.API;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ERP.WebAPI.Filters
{
    /// <summary>
    /// 驗證公開查詢 API 的驗證碼。
    /// </summary>
    public sealed class RequireCaptchaFilter : IAsyncActionFilter
    {
        public const string CaptchaIdHeaderName = "X-Captcha-Id";
        public const string CaptchaCodeHeaderName = "X-Captcha-Code";

        private readonly ICaptchaService _captchaService;

        public RequireCaptchaFilter(ICaptchaService captchaService)
        {
            _captchaService = captchaService;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var request = context.HttpContext.Request;
            var captchaId = ReadValue(request, CaptchaIdHeaderName, "captchaId");
            var captchaCode = ReadValue(request, CaptchaCodeHeaderName, "captchaCode");

            var verificationResult = await _captchaService.VerifyAsync(captchaId, captchaCode);
            if (verificationResult)
            {
                await next();
                return;
            }

            context.Result = new BadRequestObjectResult(verificationResult);
        }

        private static string? ReadValue(HttpRequest request, string headerName, string queryStringName)
        {
            var headerValue = request.Headers[headerName].FirstOrDefault();
            if (!string.IsNullOrWhiteSpace(headerValue))
            {
                return headerValue;
            }

            var queryValue = request.Query[queryStringName].FirstOrDefault();
            return string.IsNullOrWhiteSpace(queryValue) ? null : queryValue;
        }
    }
}
