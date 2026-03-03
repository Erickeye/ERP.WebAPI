using ERP.Library.Enums;
using ERP.Library.Helpers;
using ERP.Library.ViewModels;
using ERP.Service.API.AMS;
using Microsoft.EntityFrameworkCore;

namespace ERP.WebAPI.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (DbUpdateException ex)
            {
                context.Response.ContentType = "application/json";

                //_logger.LogError(ex, $"資料儲存失敗-{ex.InnerException.Message}，請確認關聯資料是否存在。");
                var result = ResultModel.Error(ErrorCodeType.DbRelationError, $"資料儲存失敗，請確認關聯資料是否存在");

                await context.Response.WriteAsJsonAsync(result);
            }
            catch (Exception ex)
            {
                context.Response.ContentType = "application/json";

                //_logger.LogError(ex, $"發生未處理的例外狀況。");
                var result = ResultModel.Error(ErrorCodeType.Exception);

                await context.Response.WriteAsJsonAsync(result);
            }
        }
    }
}
