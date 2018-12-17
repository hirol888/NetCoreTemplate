using MediatR;

namespace NetCoreTemplate.Application.Auth.Queries.Login {
  public class LoginQuery : IRequest<LoginResponseViewModel> {
    public string Email { get; }
    public string Password { get; }
    public string RemoteIpAddress { get; }

    public LoginQuery(string email, string password, string remoteIpAddress) {
      Email = email;
      Password = password;
      RemoteIpAddress = remoteIpAddress;
    }
  }
}
