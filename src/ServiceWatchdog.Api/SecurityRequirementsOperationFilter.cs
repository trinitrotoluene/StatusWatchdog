using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace ServiceWatchdog.Api
{
    public class SecurityRequirementsOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var isAuthed =
            (context.MethodInfo.DeclaringType.GetCustomAttributes(true).Any(x => x is AuthorizeAttribute)
            || context.MethodInfo.GetCustomAttributes(true).Any(x => x is AuthorizeAttribute))
            && !context.MethodInfo.GetCustomAttributes(true).Any(x => x is AllowAnonymousAttribute);

        if (isAuthed)
        {
            operation.Responses.Add("401", new OpenApiResponse { Description = "Unauthorized" });

            var authScheme = new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "apikey" }
            };

            operation.Security = new List<OpenApiSecurityRequirement>
            {
                new OpenApiSecurityRequirement
                {
                    [ authScheme ] = new[] { "apikey" }
                }
            };
        }
    }
}
}
