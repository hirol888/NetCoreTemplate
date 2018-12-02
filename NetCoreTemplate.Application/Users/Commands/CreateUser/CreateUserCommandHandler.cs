using System.Threading;
using System.Threading.Tasks;
using MediatR;
using NetCoreTemplate.Application.Interfaces;
using NetCoreTemplate.Domain.Entities;
using NetCoreTemplate.Persistence;

namespace NetCoreTemplate.Application.Users.Commands.CreateUser {
  public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, Unit> {
    private readonly NetCoreTemplateDbContext _context;
    private readonly INotificationService _notificationService;

    public CreateUserCommandHandler(
      NetCoreTemplateDbContext context,
      INotificationService notificationService
      ) {
      _context = context;
      _notificationService = notificationService;
    }

    public async Task<Unit> Handle(CreateUserCommand request, CancellationToken cancellationToken) {
      var entity = new User {
        Id = request.Id,
        Password = request.Password
      };

      _context.Users.Add(entity);

      await _context.SaveChangesAsync(cancellationToken);

      return Unit.Value;
    }
  }
}
