using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.DependencyInjection;
using NetCoreTemplate.Application.Users.Commands.CreateUser;
using NetCoreTemplate.WebApi.Filters;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using System;

namespace NetCoreTemplate.WebApi.Configuration {
  public static class WebApiExtensions {
    public static void ConfigureApi(this IServiceCollection services) {
      services.AddAntiforgery(options => options.HeaderName = "X-XSRF-TOKEN");
      services.AddRouting(options => options.LowercaseUrls = true);

      services.AddCors((options => options.AddPolicy("AllowAllOrigins",
        builder => {
          builder.AllowAnyOrigin()
          .AllowAnyHeader()
          .AllowAnyMethod()
          .SetPreflightMaxAge(TimeSpan.FromSeconds(2250));
        })));

      services.AddMvc(o => {
        o.Filters.AddService(typeof(UserExceptionFilterAttribute));
        o.ModelValidatorProviders.Clear();

        var policy = new AuthorizationPolicyBuilder()
          .RequireAuthenticatedUser()
          .Build();
        o.Filters.Add(new AuthorizeFilter(policy));
      })
      .AddJsonOptions(options => {
        var settings = options.SerializerSettings;

        var camelCasePropertyNamesContractResolver = new CamelCasePropertyNamesContractResolver();

        settings.ContractResolver = camelCasePropertyNamesContractResolver;
        settings.Converters = new JsonConverter[] {
          new IsoDateTimeConverter(),
          new StringEnumConverter(true)
        };
      })
      .SetCompatibilityVersion(CompatibilityVersion.Version_2_1)
      .AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<CreateUserCommandValidator>());
    }

    public static void ConfigureAssets(this IApplicationBuilder app) {
      app.UseFileServer();
      app.UseStaticFiles();
      app.UseCors("AllowAllOrigins");
    }
  }
}
