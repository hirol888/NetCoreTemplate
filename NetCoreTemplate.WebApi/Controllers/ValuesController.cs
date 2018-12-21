using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NetCoreTemplate.Application.Auth.Commands.Register;
using NetCoreTemplate.Application.Users.Queries.GetUserDetail;
using NetCoreTemplate.Application.Users.Queries.GetUserList;
using NetCoreTemplate.WebApi.Services;
using Serilog;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace NetCoreTemplate.WebApi.Controllers {
  [Route("api/[controller]")]
  [ApiController]
  public class ValuesController : ApiBaseController {
    public ValuesController(IMapper mapper, IServiceInvoker serviceInvoker) : base(mapper, serviceInvoker) { }
    // GET api/values
    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<UsersListViewModel>> Get() {
      return Ok(await Mediator.Send(new GetUsersListQuery()));
    }

    // GET api/values/5
    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<ActionResult<UserDetailModel>> Get(int id) {
      return Ok(await Mediator.Send(new GetUserDetailQuery { Id = id }));
    }

    // POST api/values
    [HttpPost]
    [AllowAnonymous]
    public async Task<ActionResult> Create([FromBody] RegisterCommand value) {
      await Mediator.Send(value);

      return NoContent();
    }

    // PUT api/values/5
    [HttpPut("{id}")]
    public void Put(int id, [FromBody] string value) {
    }

    // DELETE api/values/5
    [HttpDelete("{id}")]
    public void Delete(int id) {
    }
  }
}
