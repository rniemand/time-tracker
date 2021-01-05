using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Rn.NetCore.Common.Logging;
using Rn.NetCore.Common.Metrics;
using TimeTracker.Core.Models.Requests;
using TimeTracker.Core.Models.Responses;
using TimeTracker.Core.Services;
using TimeTracker.Core.WebApi.Attributes;

namespace TimeTracker.Controllers
{
  [ApiController, Route("api/[controller]")]
  public class AuthController : BaseController<AuthController>
  {
    private readonly IUserService _userService;

    public AuthController(
      ILoggerAdapter<AuthController> logger,
      IMetricService metrics,
      IUserService userService
    ) : base(logger, metrics)
    {
      _userService = userService;
    }

    [HttpPost, Route("authenticate")]
    public async Task<ActionResult<AuthenticationResponse>> Authenticate([FromBody] AuthenticationRequest request)
    {
      // TODO: [TESTS] (AuthController.Authenticate) Add tests
      var response = await _userService.Authenticate(request);

      if (response == null)
        return BadRequest(new
        {
          message = "Username or password is incorrect"
        });

      return Ok(response);
    }

    [HttpGet, Route("locked"), Authorize]
    public ActionResult<string> Locked()
    {
      return Ok("woot");
    }

    [HttpGet, Route("testing")]
    public async Task<ActionResult<RichardResponse>> Test()
    {
      await Task.CompletedTask;

      var baseResponse = new BaseResponse<RichardResponse>()
        .AsFailure()
        .WithResponse(new RichardResponse("hello world"))
        .WithValidationError("issue 1")
        .WithValidationError("issue 2");

      return ProcessResponse(baseResponse);
    }
  }
}
