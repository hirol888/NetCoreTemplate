using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NetCoreTemplate.Application.Auth.Commands.Register;
using NetCoreTemplate.Application.Auth.Queries.Login;
using NetCoreTemplate.WebApi.Services;

namespace NetCoreTemplate.WebApi.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class AuthController : ApiBaseController {
    public AuthController(IMapper mapper, IServiceInvoker serviceInvoker) : base(mapper, serviceInvoker) { }

    //[HttpPost("register")]
    //[AllowAnonymous]
    //public async Task<ActionResult> Register([FromBody] RegisterCommand value) {
    //  await Mediator.Send(value);

    //  return NoContent();
    //}

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult> Login([FromBody] LoginQuery value) {
      return Ok(await Mediator.Send(value));
    }
  }
}