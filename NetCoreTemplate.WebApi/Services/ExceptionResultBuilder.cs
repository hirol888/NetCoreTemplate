using System;
using Autofac.Core;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using NetCoreTemplate.Application.Exceptions;

namespace NetCoreTemplate.WebApi.Services {
  public class ExceptionResultBuilder : IExceptionResultBuilder {
    private readonly IHostingEnvironment _hostingEnvironment;
    private readonly ILogger _logger;

    public ExceptionResultBuilder(IHostingEnvironment hostingEnvironment, ILogger logger) {
      _hostingEnvironment = hostingEnvironment;
      _logger = logger;
    }

    public IActionResult Build(Exception exception) {
            var stackTrace = "No stack trace available";

            if (!string.Equals(_hostingEnvironment.EnvironmentName, "Production", StringComparison.OrdinalIgnoreCase))
                stackTrace = exception.GetBaseException().StackTrace;
            var statusCode = 500;
            string content = null;
            var message = exception.GetBaseException().Message;

            var dependencyResolutionException = exception as DependencyResolutionException;
            if (dependencyResolutionException != null)
                message = $"Dependency Exception: Please ensure that classes implement the interface: {message}";

            var notFoundException = exception as NotFoundException;
            if (notFoundException != null)
                return new NotFoundResult();

            var deleteFailureException = exception as DeleteFailureException;
            if (deleteFailureException != null)
                return new BadRequestResult();


            var apiException = exception as ApiException;

            if (apiException != null) {
                statusCode = (int)apiException.StatusCode;
                content = apiException.GetContent();
                if (!string.IsNullOrEmpty(apiException.Message))
                    message = apiException.GetBaseException().Message;
            }

            return CreateActionResult(content, message, stackTrace, statusCode, exception);
        }

        protected virtual IActionResult CreateActionResult(string content, string message, string stackTrace,
            int statusCode, Exception exception) {
            var apiError = new ApiError {
                Error = content ?? message
            };

            if (!string.IsNullOrEmpty(stackTrace))
                apiError.StackTrace = stackTrace;

            var objectResult = new ObjectResult(apiError) {
                StatusCode = statusCode
            };
            var eventId = new Microsoft.Extensions.Logging.EventId(statusCode);

            _logger.Error($"{eventId.Name}, {exception}, {message}");

            return objectResult;
        }
    }
}
