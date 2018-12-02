using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using Microsoft.AspNetCore.Builder;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using NetCoreTemplate.Common.Models.Options;

namespace NetCoreTemplate.WebApi.Configuration {
  public static class JwtExtensions {
    public static void ConfigureJwt(this IServiceCollection services, IConfiguration configuration,
      Func<JwtIssuerOptions, SecurityKey> signingKey) {
      var jwtAppSettingOptions = configuration.GetSection(nameof(JwtIssuerOptions)).Get<JwtIssuerOptions>();

      var tokenValidationParameters = new TokenValidationParameters {
        ValidateIssuer = true,
        ValidIssuer = jwtAppSettingOptions.Issuer,

        ValidateAudience = true,
        ValidAudience = jwtAppSettingOptions.Audience,

        ValidateIssuerSigningKey = true,
        IssuerSigningKey = signingKey(jwtAppSettingOptions),

        RequireExpirationTime = true,
        ValidateLifetime = true,

        ClockSkew = TimeSpan.Zero
      };

      services.AddAuthentication(options => {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
      })
      .AddJwtBearer(options => {
        options.RequireHttpsMetadata = false;
        options.TokenValidationParameters = tokenValidationParameters;
        options.Events = new JwtBearerEvents {
          OnMessageReceived = context => {
            var task = Task.Run(() => {
              if (context.Request.Query.TryGetValue("securityToken", out var securityToken)) {
                context.Token = securityToken.FirstOrDefault();
              }
            });

            return task;
          }
        };
      });
    }

    public static void ConfigureJwt(this IApplicationBuilder app) {
      app.UseAuthentication();
    }
  }
}
