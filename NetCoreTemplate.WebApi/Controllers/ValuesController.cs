using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NetCoreTemplate.WebApi.Services;

namespace NetCoreTemplate.WebApi.Controllers {
  [Route("api/[controller]")]
  [ApiController]
  public class ValuesController : ApiBaseController {
    public ValuesController(IMapper mapper, IServiceInvoker serviceInvoker) : base(mapper, serviceInvoker) { }
    // GET api/values
    [HttpGet]
    [AllowAnonymous]
    public ActionResult<IEnumerable<string>> Get() {
      return new string[] { "value1", "value2" };
    }

    // GET api/values/5
    [HttpGet("{id}")]
    public ActionResult<string> Get(int id) {
      return "value";
    }

    // POST api/values
    [HttpPost]
    public void Post([FromBody] string value) {
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
