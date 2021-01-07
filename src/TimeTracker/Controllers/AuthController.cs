using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Rn.NetCore.Common.Logging;
using Rn.NetCore.Common.Metrics;
using TimeTracker.Core.Models.Requests;
using TimeTracker.Core.Models.Responses;
using TimeTracker.Core.Services;

namespace TimeTracker.Controllers
{
  [ApiController, Route("api/[controller]")]
  public class AuthController : BaseController<AuthController>
  {
    public AuthController(
      ILoggerAdapter<AuthController> logger,
      IMetricService metrics,
      IUserService userService
    ) : base(logger, metrics, userService)
    { }

    [HttpPost, Route("authenticate")]
    public async Task<ActionResult<AuthenticationResponse>> Authenticate([FromBody] AuthenticationRequest request)
    {
      // TODO: [TESTS] (AuthController.Authenticate) Add tests
      var response = await UserService.Authenticate(request);

      if (response == null)
        return BadRequest(new
        {
          message = "Username or password is incorrect"
        });

      return Ok(response);
    }
  }
}
