using FluentValidation.AspNetCore;
using MediatR;
using MediatR.Pipeline;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Cors.Internal;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.PlatformAbstractions;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using NetCoreTemplate.Application.Users.Commands.CreateUser;
using NetCoreTemplate.Application.Infrastructure;
using NetCoreTemplate.Application.Interfaces;
using NetCoreTemplate.Infrastructure;
using NetCoreTemplate.Persistence;
using NetCoreTemplate.WebApi.Filters;
using System.Reflection;
using System;
using Swashbuckle.AspNetCore.Swagger;
using System.IO;
using System.Collections.Generic;
using Autofac.Extensions.DependencyInjection;
using Autofac;
using Microsoft.Extensions.Logging;
using NetCoreTemplate.WebApi.Configuration;
using NetCoreTemplate.Common;
using NetCoreTemplate.Common.Models.Options;
using NetCoreTemplate.Common.Models.Security;
using System.Text;

namespace NetCoreTemplate.WebApi {
  public class Startup {
    private readonly IConfiguration _configuration;
    private readonly ILogger<Startup> _logger;

    public Startup(IConfiguration configuration, ILogger<Startup> logger, IHostingEnvironment hostingEnvironment) {
      _configuration = configuration;
      _logger = logger;

      _logger.LogInformation($"Constructing for environment: {hostingEnvironment.EnvironmentName}");
    }

    protected IContainer ApplicationContainer { get; private set; }

    // This method gets called by the runtime. Use this method to add services to the container.
    public IServiceProvider ConfigureServices(IServiceCollection services) {
      _logger.LogInformation("Starting: Configure Services");

      services.ConfigureLogging();

      services.AddOptions();

      services.ConfigureJwt(_configuration, ConfigureSecurityKey);

      services.ConfigureAutomapper(Configuration => { });

      services.ConfigureApi();

      services.ConfigureCompression();

      services.ConfigureSwagger();

      services.AddMemoryCache();

      ApplicationContainer = services.ConfigureAutofacContainer(_configuration, b => { }, new CommonModule(), new ApiModule());

      var provider = new AutofacServiceProvider(ApplicationContainer);

      _logger.LogInformation("Completing: Configure Services");

      return provider;
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IHostingEnvironment env, IConfiguration configuration,
      ILoggerFactory loggerFactory, IApplicationLifetime appLifetime) {
      _logger.LogInformation("Starting: Configure");

      env.ConfigureLogger(loggerFactory, configuration);

      app.ConfigureJwt();

      app.ConfigureAssets();

      app.ConfigureSwagger();

      app.ConfigureCompression();

      app.UseMvc();

      _logger.LogInformation("Completing: Configure");
    }

    protected virtual SecurityKey ConfigureSecurityKey(JwtIssuerOptions issuerOptions) {
      var keyString = issuerOptions.Audience;
      var keyBytes = Encoding.UTF8.GetBytes(keyString);
      var signingKey = new JwtSigningKey(keyBytes);

      return signingKey;
    }
  }
}
