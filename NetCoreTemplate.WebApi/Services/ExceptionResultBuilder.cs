using System;
using NetCoreTemplate.WebApi.Models.ViewModels;
using NetCoreTemplate.Common.Models.Exceptions;
using Autofac.Core;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace NetCoreTemplate.WebApi.Services {
  public class ExceptionResultBuilder : IExceptionResultBuilder {
    private readonly IHostingEnvironment _hostingEnvironment;
    private readonly ILogger<ExceptionResultBuilder> _logger;

    public ExceptionResultBuilder(IHostingEnvironment hostingEnvironment, ILogger<ExceptionResultBuilder> logger) {
      _hostingEnvironment = hostingEnvironment;
      _logger = logger;
    }

    public IActionResult Build(Exception exception) {

    }
  }
}
