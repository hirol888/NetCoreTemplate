using System;
using Microsoft.AspNetCore.Mvc;

namespace NetCoreTemplate.WebApi.Services {
  public interface IExceptionResultBuilder {
    IActionResult Build(Exception exception);
  }
}