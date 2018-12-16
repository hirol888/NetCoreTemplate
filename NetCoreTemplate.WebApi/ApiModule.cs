using Autofac;
using Microsoft.AspNetCore.Http;
using MediatR;
using MediatR.Pipeline;
using NetCoreTemplate.Application.Users.Commands.CreateUser;
using System.Reflection;
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
    }
  }
}
