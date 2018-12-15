using System.Threading;
using System.Threading.Tasks;
using MediatR.Pipeline;
using Serilog;
using Serilog.Events;

namespace NetCoreTemplate.Application.Infrastructure {
  public class RequestLogger<TRequest> : IRequestPreProcessor<TRequest> {
    private readonly ILogger _logger;

    public RequestLogger(ILogger logger) {
      _logger = logger;
    }

    public Task Process(TRequest request, CancellationToken cancellationToken) {
      var name = typeof(TRequest).Name;

      _logger.Write(LogEventLevel.Information, $"NetCoreTemplate Request: {name} @{request}");

      return Task.CompletedTask;
    }
  }
}
