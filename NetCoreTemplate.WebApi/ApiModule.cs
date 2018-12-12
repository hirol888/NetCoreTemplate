using System.Text;
using NetCoreTemplate.WebApi.Filters;
using NetCoreTemplate.Common.Models.Options;
using Autofac;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace NetCoreTemplate.WebApi {
  public class ApiModule : Module {
    protected override void Load(ContainerBuilder builder) {
      builder.RegisterAssemblyTypes(ThisAssembly).AsImplementedInterfaces();
      builder.RegisterType<HttpContextAccessor>().As<IHttpContextAccessor>().SingleInstance();
      builder.RegisterType<GlobalExceptionFilter>().AsSelf().InstancePerLifetimeScope();
            builder.RegisterType<UserExceptionFilterAttribute>().AsSelf().InstancePerLifetimeScope();

      builder.Register(context => {
        var configurationRoot = context.Resolve<IConfiguration>();
        var issuerOptions = configurationRoot.GetSection("jwtIssuerOptions").Get<JwtIssuerOptions>();

        var key = ConfigureSecurityKey(issuerOptions);

        return key;
      }).As<SecurityKey>().SingleInstance();

      builder.Register(context => {
        var key = context.Resolve<SecurityKey>();

        return new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);
      }).As<SigningCredentials>().SingleInstance();

      builder.Register(context => {
        var configurationRoot = context.Resolve<IConfiguration>();
        var issuerOptions = configurationRoot.GetSection("jwtIssuerOptions").Get<JwtIssuerOptions>();
        var signingCredentials = context.Resolve<SigningCredentials>();

        issuerOptions.SigningCredentials = signingCredentials;

        return new OptionsWrapper<JwtIssuerOptions>(issuerOptions);
      }).As<IOptions<JwtIssuerOptions>>().InstancePerLifetimeScope();
    }

    protected virtual SecurityKey ConfigureSecurityKey(JwtIssuerOptions issuerOptions) {
      var keyString = issuerOptions.Audience;
      var keyBytes = Encoding.UTF8.GetBytes(keyString);
      var signingKey = new SymmetricSecurityKey(keyBytes);
      return signingKey;
    }
  }
}
