using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using NetCoreTemplate.Application.Users.Commands.CreateUser;
using NetCoreTemplate.Persistence;
using NetCoreTemplate.WebApi.Filters;
using System;
using System.IO;
using Autofac.Extensions.DependencyInjection;
using Autofac;
using Microsoft.Extensions.Logging;
using NetCoreTemplate.Common;
using NetCoreTemplate.Common.Models.Options;
using NetCoreTemplate.Common.Models.Security;
using System.Text;
using Serilog.Core;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Microsoft.AspNetCore.ResponseCompression;
using System.IO.Compression;
using Serilog.Events;
using Serilog;
using Microsoft.Extensions.PlatformAbstractions;
using Swashbuckle.AspNetCore.Swagger;
using AutoMapper;
using MediatR;
using MediatR.Pipeline;
using NetCoreTemplate.Application.Infrastructure;
using System.Reflection;
using NetCoreTemplate.Application.Users.Queries.GetUserList;

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

      // Configure Logging
      services.AddLogging();
      services.AddSingleton(new LoggingLevelSwitch());

      services.AddOptions();

      #region Config Jwt
      var jwtAppSettingOptions = _configuration.GetSection(nameof(JwtIssuerOptions)).Get<JwtIssuerOptions>();

      var tokenValidationParameters = new TokenValidationParameters {
        ValidateIssuer = true,
        ValidIssuer = jwtAppSettingOptions.Issuer,

        ValidateAudience = true,
        ValidAudience = jwtAppSettingOptions.Audience,

        ValidateIssuerSigningKey = true,
        IssuerSigningKey = ConfigureSecurityKey(jwtAppSettingOptions),

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
      #endregion
      
      services.AddAutoMapper();

      //services.AddAntiforgery(options => options.HeaderName = "X-XSRF-TOKEN");
      services.AddRouting(options => options.LowercaseUrls = true);

      #region Config CORS
      services.AddCors((options => options.AddPolicy("AllowAllOrigins",
    builder => {
      builder.AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod()
            .SetPreflightMaxAge(TimeSpan.FromSeconds(2250));
    })));
      #endregion

      services.AddDbContext<NetCoreTemplateDbContext>(options =>
          options.UseSqlServer(_configuration.GetConnectionString("NetCoreTemplateDatabase")));

      #region AddMvc
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
                      new StringEnumConverter(new DefaultNamingStrategy(), true)
        };
      })
      .SetCompatibilityVersion(CompatibilityVersion.Version_2_1)
      .AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<CreateUserCommandValidator>());
      #endregion

      services.AddMediatR(typeof(GetUsersListQueryHandler).GetTypeInfo().Assembly);

      #region Config Compression
      services.Configure<GzipCompressionProviderOptions>(options => options.Level = CompressionLevel.Fastest);
      services.AddResponseCompression(options => { options.Providers.Add<GzipCompressionProvider>(); });
      #endregion

      #region Config Swagger
      services.AddSwaggerGen(c => {
        c.SwaggerDoc("v1", new Info {
          Version = "v1",
          Title = "Net Core Template API",
          Description = "Net Core Template API"
        });

        var basePath = PlatformServices.Default.Application.ApplicationBasePath;
        var xmlPath = Path.Combine(basePath, "NetCoreTemplate.WebApi.xml");
        c.IncludeXmlComments(xmlPath);
        c.OperationFilter<AuthorizationHeaderParameterOperationFilter>();
        c.OperationFilter<FormFileOperationFilter>();
      });
      #endregion

      services.AddMemoryCache();

      #region Autofac
      var autofacBuilder = new ContainerBuilder();

      autofacBuilder.Register(ctx => _configuration).As<IConfiguration>();
      autofacBuilder.RegisterModule(new CommonModule());
      autofacBuilder.RegisterModule(new ApiModule());
      //autofacBuilder.RegisterType<NetCoreTemplateDbContext>().AsSelf().InstancePerLifetimeScope();
      //autofacBuilder.RegisterAssemblyTypes(AppDomain.CurrentDomain.GetAssemblies()).AsClosedTypesOf(typeof(IPipelineBehavior<,>));
      autofacBuilder.RegisterGeneric(typeof(RequestPreProcessorBehavior<,>)).As(typeof(IPipelineBehavior<,>));
      autofacBuilder.RegisterGeneric(typeof(RequestPerformanceBehaviour<,>)).As(typeof(IPipelineBehavior<,>));
      autofacBuilder.RegisterGeneric(typeof(RequestValidationBehaviour<,>)).As(typeof(IPipelineBehavior<,>));
      //autofacBuilder.RegisterAssemblyTypes(AppDomain.CurrentDomain.GetAssemblies()).AsClosedTypesOf(typeof(IRequestHandler<,>));
      //autofacBuilder.RegisterAssemblyTypes(typeof(IMediator).GetTypeInfo().Assembly).AsImplementedInterfaces();

      autofacBuilder.Populate(services);

      ApplicationContainer = autofacBuilder.Build();

      var provider = new AutofacServiceProvider(ApplicationContainer);
      #endregion

      _logger.LogInformation("Completing: Configure Services");

      return provider;
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IHostingEnvironment env, IConfiguration configuration,
      ILoggerFactory loggerFactory, IApplicationLifetime appLifetime) {
      _logger.LogInformation("Starting: Configure");

      #region Logging
      var baseDir = env.ContentRootPath;
      var logPath = Path.Combine(baseDir, "logs");
      if (!Directory.Exists(logPath)) {
        Directory.CreateDirectory(logPath);
      }

      LogEventLevel logLevel;

      if (!Enum.TryParse(configuration["logging:logLevel:system"], true, out logLevel)) {
        logLevel = LogEventLevel.Verbose;
      }

      var logEventSwitch = new LoggingLevelSwitch();

      logEventSwitch.MinimumLevel = logLevel;

      var loggingConfiguration = new LoggerConfiguration()
        .Enrich.FromLogContext();

      loggingConfiguration
        .MinimumLevel.ControlledBy(logEventSwitch)
        .WriteTo
        .RollingFile($@"{logPath}\{{Date}}.txt", logLevel, retainedFileCountLimit: 10, shared: true)
        .WriteTo
        .ColoredConsole();

      var logger = loggingConfiguration.CreateLogger();

      Log.Logger = logger;

      Log.Write(LogEventLevel.Information, "Logging has started");

      loggerFactory.AddConsole(configuration.GetSection("Logging"));
      loggerFactory.AddDebug();
      loggerFactory.AddSerilog(logger);
      #endregion

      app.UseAuthentication();
      app.UseFileServer();
      app.UseStaticFiles();
      app.UseCors("AllowAllOrigins");

      app.UseSwagger();
      app.UseSwaggerUI(c => {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Net Core Template API V1");
      });

      app.UseResponseCompression();

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
