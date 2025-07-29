using ERP.Library.Enums;
using ERP.Library.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ERP.WebAPI.CustomAttributes
{
    public class ValidateModelAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.ModelState.IsValid)
            {
                var errors = context.ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                var result = new ResultModel<string>();
                result.SetError(ErrorCodeType.FieldValueIsInvalid, string.Join("；", errors));

                context.Result = new JsonResult(result) { StatusCode = StatusCodes.Status200OK };
            }
        }
    }

}
