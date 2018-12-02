using Microsoft.IdentityModel.Tokens;

namespace NetCoreTemplate.Common.Models.Security {
  public class JwtSigningKey : SymmetricSecurityKey {
    public JwtSigningKey(byte[] key) : base(key) { }
  }
}
