using System.IO;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.PlatformAbstractions;
using NetCoreTemplate.WebApi.Filters;
using Swashbuckle.AspNetCore.Swagger;

namespace NetCoreTemplate.WebApi.Configuration {
  public static class SwaggerExtensions {
    public static void ConfigureSwagger(this IServiceCollection services) {
      services.AddSwaggerGen(c => {
        c.SwaggerDoc("v1", new Info {
          Version = "v1",
          Title = "Net Core Template API",
          Description = "Net Core Template API"
        });

        var basePath = PlatformServices.Default.Application.ApplicationBasePath;
        var xmlPath = Path.Combine(basePath, "NetCoreTemplate.xml");
        c.IncludeXmlComments(xmlPath);
        c.OperationFilter<AuthorizationHeaderParameterOperationFilter>();
        c.OperationFilter<FormFileOperationFilter>();
      });
    }

    public static void ConfigureSwagger(this IApplicationBuilder app) {
      app.UseSwagger();

      app.UseSwaggerUI(c => {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Net Core Template API V1");
      });
    }
  }
}
