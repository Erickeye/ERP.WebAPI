using ERP.Library.Enums;
using ERP.Library.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using ERP.Library.Extensions;

namespace ERP.WebAPI.CustomAttributes
{
    public class ValidateModelAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.ModelState.IsValid)
            {
                var result = new ResultModel<Dictionary<string, List<string>>>();

                result.ErrorCode = ErrorCodeType.FieldValueIsInvalid;
                result.Data = context.ModelState.GetErrorsDictionary();

                context.Result = new JsonResult(result)
                {
                    StatusCode = 200
                };
            }
        }
    }

}
