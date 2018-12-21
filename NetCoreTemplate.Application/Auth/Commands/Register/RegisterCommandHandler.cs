using MediatR;
using Microsoft.AspNetCore.Identity;
using NetCoreTemplate.Domain.Entities;
using NetCoreTemplate.Persistence.Data;
using NetCoreTemplate.Persistence.Identity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NetCoreTemplate.Application.Auth.Commands.Register {
  public class RegisterCommandHandler : IRequestHandler<RegisterCommand, Unit> {
    private readonly UserManager<AppUser> _userManager;
    private NetCoreTemplateDbContext _context;

    public RegisterCommandHandler(UserManager<AppUser> userManager, NetCoreTemplateDbContext context) {
      _userManager = userManager;
      _context = context;
    }
    public async Task<Unit> Handle(RegisterCommand request, CancellationToken cancellationToken) {
      var appUser = new AppUser {
        Email = request.Email,
        UserName = request.UserName
      };
      var identityResult = await _userManager.CreateAsync(appUser, request.Password);

      if (!identityResult.Succeeded) {
        throw new Exception();
      }

      var user = new User(request.FirstName, request.LastName, appUser.Id, appUser.UserName);
      _context.User.Add(user);
      await _context.SaveChangesAsync();

      return Unit.Value;
    }
  }
}
