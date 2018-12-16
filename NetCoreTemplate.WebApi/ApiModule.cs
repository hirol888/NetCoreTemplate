using System.Text;
using NetCoreTemplate.WebApi.Filters;
using NetCoreTemplate.Common.Models.Options;
using Autofac;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MediatR;
using MediatR.Pipeline;
using NetCoreTemplate.Application.Users.Commands.CreateUser;
using System.Reflection;
using AutofacSerilogIntegration;
using NetCoreTemplate.Application.Infrastructure;
using NetCoreTemplate.Application.Users.Queries.GetUserDetail;

namespace NetCoreTemplate.WebApi {
  public class ApiModule : Autofac.Module {
    protected override void Load(ContainerBuilder builder) {
      builder.RegisterAssemblyTypes(ThisAssembly).AsImplementedInterfaces();
      builder.RegisterType<HttpContextAccessor>().As<IHttpContextAccessor>().SingleInstance();

      #region MediatR
      builder.RegisterAssemblyTypes(typeof(IMediator).GetTypeInfo().Assembly).AsImplementedInterfaces();

      var mediatrOpenTypes = new[]
        {
            typeof(IRequestHandler<,>),
            typeof(IRequestHandler<>)
        };

      foreach (var mediatrOpenType in mediatrOpenTypes) {
        // Register all command handler in the same assembly as WriteLogMessageCommandHandler
        builder
            .RegisterAssemblyTypes(typeof(CreateUserCommandHandler).GetTypeInfo().Assembly)
            .AsClosedTypesOf(mediatrOpenType)
            .AsImplementedInterfaces();

        // Register all QueryHandlers in the same assembly as GetExternalLoginQueryHandler
        builder
            .RegisterAssemblyTypes(typeof(GetUserDetailQueryHandler).GetTypeInfo().Assembly)
            .AsClosedTypesOf(mediatrOpenType)
            .AsImplementedInterfaces();
      }

      builder.RegisterGeneric(typeof(RequestPostProcessorBehavior<,>)).As(typeof(IPipelineBehavior<,>));
      builder.RegisterGeneric(typeof(RequestPreProcessorBehavior<,>)).As(typeof(IPipelineBehavior<,>));
      builder.RegisterGeneric(typeof(RequestLogger<>)).As(typeof(IRequestPreProcessor<>));

      builder.Register<ServiceFactory>(ctx => {
        var c = ctx.Resolve<IComponentContext>();
        return t => {
          object o;
          return c.TryResolve(t, out o) ? o : null;
        };
      });
      #endregion

      #region Jwt
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
      #endregion
    }

    protected SecurityKey ConfigureSecurityKey(JwtIssuerOptions issuerOptions) {
      var keyString = issuerOptions.Audience;
      var keyBytes = Encoding.UTF8.GetBytes(keyString);
      var signingKey = new SymmetricSecurityKey(keyBytes);
      return signingKey;
    }
  }
}
