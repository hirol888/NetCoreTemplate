﻿using Microsoft.IdentityModel.Tokens;
using NetCoreTemplate.Application.Interfaces.Auth;
using Serilog;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace NetCoreTemplate.Application.Auth {
  internal sealed class JwtTokenHandler : IJwtTokenHandler {
    private readonly JwtSecurityTokenHandler _jwtSecurityTokenHandler;

    internal JwtTokenHandler() {
      if (_jwtSecurityTokenHandler == null) {
        _jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
      }
    }

    public string WriteToken(JwtSecurityToken jwt) {
      return _jwtSecurityTokenHandler.WriteToken(jwt);
    }

    public ClaimsPrincipal ValidateToken(string token, TokenValidationParameters tokenValidationParameters) {
      try {
        var principal = _jwtSecurityTokenHandler.ValidateToken(token, tokenValidationParameters, out var securityToken);
        if (!(securityToken is JwtSecurityToken jwtSecurityToken) || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase)) {
          throw new SecurityTokenException("Invalid token");
        }

        return principal;
      } catch (Exception e) {
        Log.Error(e, "Token validation failed");
        return null;
      }
    }
  }
}
