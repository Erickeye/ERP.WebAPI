using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace ERP.Library.Extensions
{
    public static class ModelStateExtensions
    {
        public static List<string> GetAllErrorMessages(this ModelStateDictionary modelState)
        {
            return modelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .ToList();
        }

        public static string GetAllErrorMessagesAsString(this ModelStateDictionary modelState, string separator = "; ")
        {
            return string.Join(separator, modelState.GetAllErrorMessages());
        }

        public static Dictionary<string, List<string>> GetErrorsDictionary(this ModelStateDictionary modelState)
        {
            return modelState
                .Where(kvp => kvp.Value.Errors.Any())
                .ToDictionary(
                    kvp => kvp.Key, // 欄位名稱
                    kvp => kvp.Value.Errors
                               .Select(e => e.ErrorMessage)
                               .ToList()
                );
        }
    }
}
