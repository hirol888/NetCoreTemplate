using System;
using System.Collections.Generic;
using System.Text;

namespace NetCoreTemplate.Domain.Entities {
  public class User {
    public int Id { get; set; }
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
