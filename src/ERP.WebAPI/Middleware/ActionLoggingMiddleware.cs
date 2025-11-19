using ERP.EntityModels.Context;
using ERP.EntityModels.Models;
using ERP.WebAPI.CustomAttributes;

namespace ERP.WebAPI.Middleware
{
    public class ActionLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ActionLoggingMiddleware> _logger;

        public ActionLoggingMiddleware(RequestDelegate next, ILogger<ActionLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }
        public async Task InvokeAsync(HttpContext context, ERPContext dbContext)
        {
            var endpoint = context.GetEndpoint();

            // 取得 ActionLogAttribute
            var actionLogAttr = endpoint?.Metadata?.GetMetadata<LogAttribute>();

            if (actionLogAttr != null) {
                try
                {
                    // 取得基本資訊
                    var userAccount = context.User.Claims.FirstOrDefault(c => c.Type == "account")?.Value ?? "Anonymous";
                    var ip = context.Connection.RemoteIpAddress?.ToString() ?? "";
                    var date = DateTime.Now;
                    var location = context.Request.Path.Value;
                    var method = context.Request.Method;
                    var keyId = context.Request.Query["id"].FirstOrDefault(); //依需求改
                    var describe = $"Request to {location}";

                    var logEntity = new t_1710ActionInfo
                    {
                        Account = userAccount,
                        IpAddress = ip,
                        CrateDate = date,
                        Location = location,
                        ActionType = (int)actionLogAttr.ActionCode,
                        KeyId = keyId,
                        Memo = $"{actionLogAttr.Description}[{method}]"
                    };

                    dbContext.t_1710ActionInfo.Add(logEntity);
                    await dbContext.SaveChangesAsync();

                    // Optional: Console log
                    //_logger.LogInformation("Logged action: {Controller} {Method} by {User}", controller, method, userAccount);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error saving action log");
                }
            }

            await _next(context);
        }

    }
}
