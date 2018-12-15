using System.Threading;
using System.Threading.Tasks;
using MediatR;
using NetCoreTemplate.Application.Interfaces;
using NetCoreTemplate.Domain.Entities;
using NetCoreTemplate.Persistence;

namespace NetCoreTemplate.Application.Users.Commands.CreateUser {
  public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, Unit> {
    private readonly NetCoreTemplateDbContext _context;

    public CreateUserCommandHandler(NetCoreTemplateDbContext context) {
      _context = context;
    }

    public async Task<Unit> Handle(CreateUserCommand request, CancellationToken cancellationToken) {
      var entity = new User {
        Password = request.Password,
        Email = request.Email,
        Active = request.Active,
        Deleted = request.Deleted,
        LastIpAddress = request.LastIpAddress,
        CreateAtUtc = request.CreateAtUtc,
        LastLoginDateUtc = request.LastLoginDateUtc,
        FirstName = request.FirstName,
        LastName = request.LastName,
        Mobile = request.Mobile
      };

      _context.User.Add(entity);

      await _context.SaveChangesAsync(cancellationToken);

      return Unit.Value;
    }
  }
}
