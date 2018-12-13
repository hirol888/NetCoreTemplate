using System.Collections.Generic;
using System.Linq;
using System.Net;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using NetCoreTemplate.Application.Exceptions;
using NetCoreTemplate.WebApi.Services;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace NetCoreTemplate.WebApi.Controllers {
  [ProducesResponseType(typeof(void), (int)HttpStatusCode.NotFound)]
  [ProducesResponseType(typeof(ValidationError[]), (int)HttpStatusCode.BadRequest)]
  [ProducesResponseType(typeof(ApiError), (int)HttpStatusCode.InternalServerError)]
  [Route("api/[controller]")]
  public abstract class ApiBaseController : Controller {
    protected ApiBaseController(IMapper mapper, IServiceInvoker serviceInvoker) {
      Mapper = mapper;
      ServiceInvoker = serviceInvoker;
    }

    protected IMapper Mapper { get; }
    protected IServiceInvoker ServiceInvoker { get; }

    public override void OnActionExecuting(ActionExecutingContext context) {
      if (!context.ModelState.IsValid)
        if (!ModelState.IsValid) {
          var errorList = ModelState.ToDictionary(
              kvp => kvp.Key,
              kvp => kvp.Value.Errors.Select(e =>
                  string.IsNullOrEmpty(e.ErrorMessage)
                      ? e.Exception?.GetBaseException().Message
                      : e.ErrorMessage).ToArray()
          );

          throw new ApiException<IDictionary<string, string[]>>("Invalid request", errorList);
        }

      base.OnActionExecuting(context);
    }
  }
}