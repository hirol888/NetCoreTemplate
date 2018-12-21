using Microsoft.AspNetCore.Identity;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using NetCoreTemplate.Persistence.Data;
using NetCoreTemplate.Persistence.Identity;
using NetCoreTemplate.Application.Interfaces.Auth;
using Microsoft.EntityFrameworkCore;
using NetCoreTemplate.Application.Exceptions;
using MediatR;
using System.Linq;

namespace NetCoreTemplate.Application.Auth.Queries.Login {
  public class LoginQueryHandler : IRequestHandler<LoginQuery, LoginResponseViewModel> {
    private readonly UserManager<AppUser> _userManager;
    private readonly IJwtFactory _jwtFactory;
    private readonly ITokenFactory _tokenFactory;
    private readonly IMapper _mapper;
    private readonly NetCoreTemplateDbContext _context;

    public LoginQueryHandler(UserManager<AppUser> userManager, IJwtFactory jwtFactory,
      ITokenFactory tokenFactory, IMapper mapper, NetCoreTemplateDbContext context) {
      _userManager = userManager;
      _jwtFactory = jwtFactory;
      _tokenFactory = tokenFactory;
      _mapper = mapper;
      _context = context;
    }

    public async Task<LoginResponseViewModel> Handle(LoginQuery request, CancellationToken cancellationToken) {
      var appUser = await _userManager.FindByNameAsync(request.UserName);
      var user = appUser == null ? null : _mapper.Map(appUser, await _context.User.Where(u => u.UserName.Equals(appUser.UserName)).FirstOrDefaultAsync());
      if (user != null) {
        if (await _userManager.CheckPasswordAsync(appUser, request.Password)) {
          var refreshToken = _tokenFactory.GenerateToken();
          user.AddRefreshToken(refreshToken, user.Id, request.RemoteIpAddress);
          _context.Entry(user).State = EntityState.Modified;
          await _context.SaveChangesAsync();

          var a = new LoginResponseViewModel(
            await _jwtFactory.GenerateEncodedToken(user.IdentityId, user.UserName),
            refreshToken,
            true);
          return a;
        }
      }

      throw new System.Exception();
    }
  }
}
