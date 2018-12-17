using Microsoft.AspNetCore.Identity;
using NetCoreTemplate.Persistence;
using System.Threading;
using System.Threading.Tasks;
using NetCoreTemplate.Infrastructure.Identity;
using AutoMapper;

namespace NetCoreTemplate.Application.Auth.Queries.Login {
  public class LoginQueryHandler {
    private readonly NetCoreTemplateDbContext _context;
    private readonly UserManager<AppUser> _userManager;
    private readonly IMapper _mapper;

    public LoginQueryHandler(NetCoreTemplateDbContext context) {
      _context = context;
    }

    public async Task<LoginResponseViewModel> Handle(LoginQuery request, CancellationToken cancellationToken) {
      var appUser = await _userManager.FindByNameAsync(request.Email);
      return appUser == null ? null : _mapper.Map(appUser, await GetSingleBySpec(new UserSpecification(appUser.Id)));
    }
  }
}
