﻿using System.Text;
using NetCoreTemplate.Common.Models.Options;
using NetCoreTemplate.Common.Models.Security;
using Autofac;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using NetCoreTemplate.Persistence;

namespace NetCoreTemplate.Common {
  public class CommonModule : Module {
    protected override void Load(ContainerBuilder builder) {
      builder.RegisterAssemblyTypes(ThisAssembly).AsImplementedInterfaces();

      builder.RegisterType<NetCoreTemplateDbContext>().As<typeof(NetCoreTemplateDbContext)>().InstancePerLifetimeScope();

      builder.Register(context => {
        var configuration = context.Resolve<IConfiguration>();
        var issuerOptions = configuration.GetSection("jwtIssuerOptions").Get<JwtIssuerOptions>();

        var keyString = issuerOptions.Audience;
        var keyBytes = Encoding.Unicode.GetBytes(keyString);

        var key = new JwtSigningKey(keyBytes);

        issuerOptions.SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);

        return new OptionsWrapper<JwtIssuerOptions>(issuerOptions);
      }).As<IOptions<JwtIssuerOptions>>().InstancePerLifetimeScope();
    }
  }
}