using MediatR;

namespace NetCoreTemplate.Application.Users.Commands.CreateUser {
  public class CreateUserCommand : IRequest {
    public int Id { get; set; }
    public string Password { get; set; }
  }
}
