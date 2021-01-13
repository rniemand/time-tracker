using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Rn.NetCore.Common.Logging;
using Rn.NetCore.Common.Metrics;
using TimeTracker.Core.Services;
using TimeTracker.Core.WebApi.Attributes;

namespace TimeTracker.Controllers
{
  [ApiController, Route("api/[controller]")]
  public class DailyTasksController : BaseController<ClientsController>
  {
    private readonly IDailyTasksService _tasksService;

    public DailyTasksController(
      ILoggerAdapter<ClientsController> logger,
      IMetricService metrics,
      IUserService userService,
      IDailyTasksService tasksService)
      : base(logger, metrics, userService)
    {
      _tasksService = tasksService;
    }

    [HttpGet, Route(""), Authorize]
    public async Task<ActionResult<string>> Get()
    {
      await Task.CompletedTask;
      return Ok("cool");
    }
  }
}
