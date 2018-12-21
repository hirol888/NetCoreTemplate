using AutoMapper;
using NetCoreTemplate.Domain.Entities;
using NetCoreTemplate.Persistence.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetCoreTemplate.Persistence.Data.Mapping {
  public class DataProfile : Profile {
    public DataProfile() {
      CreateMap<User, AppUser>().ConstructUsing(u => new AppUser { UserName = u.UserName, Email = u.Email })
        .ForMember(au => au.Id, opt => opt.Ignore());
      CreateMap<AppUser, User>().ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
        .ForMember(dest => dest.PasswordHash, opt => opt.MapFrom(src => src.PasswordHash))
        .ForAllOtherMembers(opt => opt.Ignore());
    }
  }
}
