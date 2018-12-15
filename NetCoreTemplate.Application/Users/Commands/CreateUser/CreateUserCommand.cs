using MediatR;
using System;

namespace NetCoreTemplate.Application.Users.Commands.CreateUser {
  public class CreateUserCommand : IRequest {
    public string Password { get; set; }
    public string Email { get; set; }
    public bool? Active { get; set; }
    public bool? Deleted { get; set; }
    public string LastIpAddress { get; set; }
    public DateTime? CreateAtUtc { get; set; }
    public DateTime? LastLoginDateUtc { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Mobile { get; set; }
  }
}
