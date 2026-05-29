using System.ComponentModel.DataAnnotations;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

public class DisplayNameSchemaFilter : ISchemaFilter
{
    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        if (schema.Properties == null)
        {
            return;
        }

        foreach (var property in context.Type.GetProperties())
        {
            var displayAttr = property.GetCustomAttributes(typeof(DisplayAttribute), false)
                                      .FirstOrDefault() as DisplayAttribute;

            if (displayAttr == null)
            {
                continue;
            }

            var propertyName = property.Name;

            // Swagger 預設可能 camelCase
            var swaggerProperty = schema.Properties
                .FirstOrDefault(x =>
                    x.Key.Equals(propertyName, StringComparison.OrdinalIgnoreCase));

            if (!swaggerProperty.Equals(default(KeyValuePair<string, OpenApiSchema>)))
            {
                swaggerProperty.Value.Description = displayAttr.Name;
            }
        }
    }
}
