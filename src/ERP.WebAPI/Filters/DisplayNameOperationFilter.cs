using System.ComponentModel.DataAnnotations;
using System.Reflection;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace ERP.WebAPI.Filters
{
    public class DisplayNameOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            if (operation.Parameters == null || operation.Parameters.Count == 0)
            {
                return;
            }

            foreach (var parameter in operation.Parameters)
            {
                var property = FindProperty(context, parameter.Name);
                var displayName = property?.GetCustomAttribute<DisplayAttribute>()?.Name;

                if (!string.IsNullOrWhiteSpace(displayName))
                {
                    parameter.Description = displayName;
                }
            }
        }

        private static PropertyInfo? FindProperty(OperationFilterContext context, string parameterName)
        {
            foreach (var apiParameter in context.ApiDescription.ParameterDescriptions)
            {
                if (!string.Equals(apiParameter.Name, parameterName, StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                var containerType = apiParameter.ModelMetadata?.ContainerType;
                var propertyName = apiParameter.ModelMetadata?.PropertyName ?? parameterName;

                var property = containerType?.GetProperty(
                    propertyName,
                    BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase);

                if (property != null)
                {
                    return property;
                }

                var parameterInfo = apiParameter.ParameterInfo();
                if (parameterInfo?.ParameterType == null || parameterInfo.ParameterType == typeof(string))
                {
                    continue;
                }

                property = parameterInfo.ParameterType.GetProperty(
                    parameterName,
                    BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase);

                if (property != null)
                {
                    return property;
                }
            }

            return null;
        }
    }
}
