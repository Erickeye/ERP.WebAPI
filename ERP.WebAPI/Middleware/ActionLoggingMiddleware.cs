using ERP.Data;
using ERP.EntityModels.Models;
using ERP.EntityModels.Models._1000Company;
using ERP.WebAPI.CustomAttributes;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

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
                    var ip = context.Connection.RemoteIpAddress?.ToString();
                    var date = DateTime.Now;
                    var controller = context.Request.Path.Value;
                    var method = context.Request.Method;
                    var controllerId = context.Request.Query["id"].FirstOrDefault(); //依需求改
                    var describe = $"Request to {controller} [{method}]";

                    var logEntity = new t_1710ActionInfo
                    {
                        f_ActionInfo_Account = userAccount,
                        f_ActionInfo_IP = ip,
                        f_ActionInfo_Date = date,
                        f_ActionInfo_Controller = controller,
                        f_ActionInfo_Function = actionLogAttr.ActionCode.ToString(),
                        f_ActionInfo_ControllerID = controllerId,
                        f_ActionInfo_describe = $"{actionLogAttr.Description}[${method}]"
                    };

                    dbContext.t_1710ActionInfo.Add(logEntity);
                    await dbContext.SaveChangesAsync();

                    // Optional: Console log
                    _logger.LogInformation("Logged action: {Controller} {Method} by {User}", controller, method, userAccount);
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
