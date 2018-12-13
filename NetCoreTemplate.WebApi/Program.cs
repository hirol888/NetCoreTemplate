using Microsoft.AspNetCore.Hosting;
using System;
using System.IO;
using Microsoft.Extensions.Configuration;
using System.Reflection;
using Serilog;
using Serilog.Events;
using Microsoft.AspNetCore;
using System.Net;

namespace NetCoreTemplate.WebApi {
  public class Program {
    public static int Main(string[] args) {
      var baseDir = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
      var logPath = Path.Combine(baseDir, "logs");
      if (!Directory.Exists(logPath)) {
        Directory.CreateDirectory(logPath);
      }

      Log.Logger = new LoggerConfiguration()
        .MinimumLevel.Debug()
        .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
        .Enrich.FromLogContext()
        .WriteTo
        .RollingFile($@"{logPath}\{{Date}}.txt", retainedFileCountLimit: 10, shared: true)
        .WriteTo.ColoredConsole()
        .CreateLogger();
      
      try {
        BuildWebHost(args).Run();
        return 0;
      } catch (Exception exception) {
        Log.Fatal(exception, "Site terminated.");
        return 1;
      } finally {
        Log.CloseAndFlush();
      }

      //var host = CreateWebHostBuilder(args).Build();

      //using (var scope = host.Services.CreateScope()) {
      //  try {
      //    var context = scope.ServiceProvider.GetService<NetCoreTemplateDbContext>();
      //    context.Database.Migrate();
      //  } catch (Exception ex) {
      //    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
      //    logger.LogError(ex, "An error occurred while migrating or initializing the database.");
      //  }
      //}
    }

    public static IWebHost BuildWebHost(string[] args) {
      // Pulls in environment from cli args or env vars
      var builder = new ConfigurationBuilder()
        .AddEnvironmentVariables()
        .AddCommandLine(args);

      var configuration = builder.Build();

      return WebHost.CreateDefaultBuilder(args)
        .UseKestrel()
        .UseContentRoot(Directory.GetCurrentDirectory())
        .ConfigureAppConfiguration((hostingContext, config) => {
          var env = hostingContext.HostingEnvironment;
          config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .AddJsonFile("appsettings.Local.json", optional: true, reloadOnChange: true)
            .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true);
          config.AddEnvironmentVariables();
        })
        .UseConfiguration(configuration)
        .UseStartup<Startup>()
        .UseSerilog(Log.Logger)
        .Build();
    }

    //public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
    //    new WebHostBuilder()
    //  .UseKestrel()
    //  .UseContentRoot(Directory.GetCurrentDirectory())
    //  .ConfigureAppConfiguration((hostingContext, config) => {
    //    var env = hostingContext.HostingEnvironment;
    //    config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    //      .AddJsonFile("appsettings.Local.json", optional: true, reloadOnChange: true)
    //      .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true);
    //    config.AddEnvironmentVariables();
    //  })
    //  .ConfigureLogging((hostingContext, logging) => {
    //    logging.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
    //    logging.AddConsole();
    //    logging.AddDebug();
    //  })
    //  .UseStartup<Startup>();
  }
}
