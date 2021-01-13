using Microsoft.AspNetCore.Mvc;
using Rn.NetCore.Common.Logging;
using Rn.NetCore.Common.Metrics;
using TimeTracker.Core.Services;

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
  }
}
