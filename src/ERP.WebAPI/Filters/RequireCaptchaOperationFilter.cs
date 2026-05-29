using ERP.WebAPI.CustomAttributes;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace ERP.WebAPI.Filters
{
    /// <summary>
    /// 自動為需要驗證碼的 API 補上 Swagger 標頭說明。
    /// </summary>
    public sealed class RequireCaptchaOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var hasRequireCaptcha =
                context.MethodInfo.GetCustomAttributes(typeof(RequireCaptchaAttribute), true).Any() ||
                context.MethodInfo.DeclaringType?.GetCustomAttributes(typeof(RequireCaptchaAttribute), true).Any() == true;

            if (!hasRequireCaptcha)
            {
                return;
            }

            operation.Parameters ??= new List<OpenApiParameter>();

            AddHeaderParameter(
                operation.Parameters,
                RequireCaptchaFilter.CaptchaIdHeaderName,
                "驗證碼識別碼，請先呼叫 Captcha API 取得。");

            AddHeaderParameter(
                operation.Parameters,
                RequireCaptchaFilter.CaptchaCodeHeaderName,
                "使用者輸入的驗證碼內容。");
        }

        private static void AddHeaderParameter(IList<OpenApiParameter> parameters, string name, string description)
        {
            if (parameters.Any(parameter => string.Equals(parameter.Name, name, StringComparison.OrdinalIgnoreCase)))
            {
                return;
            }

            parameters.Add(new OpenApiParameter
            {
                Name = name,
                In = ParameterLocation.Header,
                Required = false,
                Description = description,
                Schema = new OpenApiSchema
                {
                    Type = "string"
                }
            });
        }
    }
}
