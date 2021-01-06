using Microsoft.AspNetCore.Mvc;
using Rn.NetCore.Common.Logging;
using Rn.NetCore.Common.Metrics;
using TimeTracker.Core.Models;
using TimeTracker.Core.Models.Dto;
using TimeTracker.Core.Models.Responses;
using TimeTracker.Core.Services;

namespace TimeTracker.Controllers
{
  public class BaseController<TController> : ControllerBase
  {
    public ILoggerAdapter<TController> Logger { get; }
    public IMetricService Metrics { get; }
    public IUserService UserService { get; }

    public BaseController(
      ILoggerAdapter<TController> logger,
      IMetricService metrics,
      IUserService userService)
    {
      // TODO: [TESTS] (BaseController) Add tests
      Logger = logger;
      Metrics = metrics;
      UserService = userService;
    }

    protected ActionResult<TResponse> ProcessResponse<TResponse>(BaseResponse<TResponse> response)
    {
      // TODO: [TESTS] (BaseController.ProcessResponse) Add tests
      if (response.FailedValidation)
      {
        return BadRequest(new ValidationError(response.ValidationResult));
      }

      ExtendUserSession();

      return Ok(response.Response);
    }

    // Internal methods
    private void ExtendUserSession()
    {
      // TODO: [TESTS] (BaseController.ExtendUserSession) Add tests
      if (!HttpContext.Items.ContainsKey("User"))
        return;

      if (!(HttpContext.Items["User"] is UserDto user))
        return;

      var token = UserService.ExtendUserSession(user.UserId);
      if (string.IsNullOrWhiteSpace(token))
        return;

      HttpContext.Response.Headers.Add("x-tt-session", token);
    }
  }
}
