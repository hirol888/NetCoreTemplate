using Microsoft.AspNetCore.Mvc.Authorization;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Collections.Generic;
using System.Linq;

namespace NetCoreTemplate.WebApi.Filters {
  public class AuthorizationHeaderParameterOperationFilter : IOperationFilter {
    public void Apply(Operation operation, OperationFilterContext context) {
      var filterPipeline = context.ApiDescription.ActionDescriptor.FilterDescriptors;
      var isAuthorized = filterPipeline.Select(fi => fi.Filter).Any(f => f is AuthorizeFilter);
      var allowAnonymouse = filterPipeline.Select(fi => fi.Filter).Any(f => f is IAllowAnonymousFilter);

      if (isAuthorized && !allowAnonymouse) {
        if (operation.Parameters == null) {
          operation.Parameters = new List<IParameter>();
        }

        operation.Parameters.Add(new NonBodyParameter {
          Name = "Authorization",
          In = "header",
          Description = "access token",
          Required = true,
          Type = "string"
        });
      }
    }
  }
}
