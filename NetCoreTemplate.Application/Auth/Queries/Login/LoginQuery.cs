using MediatR;

namespace NetCoreTemplate.Application.Auth.Queries.Login {
  public class LoginQuery : IRequest<LoginResponseViewModel> {
    public string UserName { get; set; }
    public string Password { get; set; }
    public string RemoteIpAddress { get; set; }

    public LoginQuery(string userName, string password, string remoteIpAddress) {
      UserName = userName;
      Password = password;
      RemoteIpAddress = remoteIpAddress;
    }
  }
}
