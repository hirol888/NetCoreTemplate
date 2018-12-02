using System;
using NetCoreTemplate.WebApi.Services;
using Microsoft.AspNetCore.Mvc.Filters;

namespace NetCoreTemplate.WebApi.Filters {
  public class GlobalExceptionFilter : IExceptionFilter, IDisposable {
    private readonly IExceptionResultBuilder _exceptionResultBuilder;
  }
}
