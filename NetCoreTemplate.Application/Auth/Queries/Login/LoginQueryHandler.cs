using Microsoft.AspNetCore.Identity;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using NetCoreTemplate.Persistence.Data;
using NetCoreTemplate.Persistence.Identity;
using NetCoreTemplate.Application.Interfaces.Auth;

namespace NetCoreTemplate.Application.Auth.Queries.Login {
  public class LoginQueryHandler {
    private readonly UserManager<AppUser> _userManager;
    private readonly IJwtFactory _jwtFactory;
    private readonly ITokenFactory _tokenFactory;

    public LoginQueryHandler(UserManager<AppUser> userManager, IJwtFactory jwtFactory, ITokenFactory tokenFactory) {
      _userManager = userManager;
      _jwtFactory = jwtFactory;
      _tokenFactory = tokenFactory;
    }

    public async Task<LoginResponseViewModel> Handle(LoginQuery request, CancellationToken cancellationToken) {
      var appUser = await _userManager.FindByEmailAsync(request.Email);
      if (appUser != null) {
        if (await _userManager.CheckPasswordAsync(appUser, request.Password)) {
          var refreshToken = _tokenFactory.GenerateToken();
          appUser.AddRefreshToken()
        }
      }

      return null;
    }
  }
}
