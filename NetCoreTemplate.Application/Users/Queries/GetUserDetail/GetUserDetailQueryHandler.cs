using MediatR;
using NetCoreTemplate.Application.Exceptions;
using NetCoreTemplate.Domain.Entities;
using NetCoreTemplate.Persistence;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NetCoreTemplate.Application.Users.Queries.GetUserDetail {
  public class GetUserDetailQueryHandler : IRequestHandler<GetUserDetailQuery, UserDetailModel> {
    private readonly NetCoreTemplateDbContext _context;

    public GetUserDetailQueryHandler(NetCoreTemplateDbContext context) {
      _context = context;
    }

    public async Task<UserDetailModel> Handle(GetUserDetailQuery request, CancellationToken cancellationToken) {
      var entity = await _context.User
                .FindAsync(request.Id);

      if (entity == null) {
        throw new NotFoundException(nameof(User), request.Id);
      }

      return new UserDetailModel {
        Id = entity.Id,
        Password = entity.Password,
        Email = entity.Email,
        Active = entity.Active,
        Deleted = entity.Deleted,
        LastIpAddress = entity.LastIpAddress,
        CreateAtUtc = entity.CreateAtUtc,
        LastLoginDateUtc = entity.LastLoginDateUtc,
        FirstName = entity.FirstName,
        LastName = entity.LastName,
        Mobile = entity.Mobile
      };
    }
  }
}
