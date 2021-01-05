using Microsoft.AspNetCore.Mvc;
using Rn.NetCore.Common.Logging;
using Rn.NetCore.Common.Metrics;
using TimeTracker.Core.Models.Responses;

namespace TimeTracker.Controllers
{
  public class BaseController<TController> : ControllerBase
  {
    public ILoggerAdapter<TController> Logger { get; set; }
    public IMetricService Metrics { get; set; }

    public BaseController(
      ILoggerAdapter<TController> logger,
      IMetricService metrics)
    {
      // TODO: [TESTS] (BaseController) Add tests
      Logger = logger;
      Metrics = metrics;
    }

    protected ActionResult<TResponse> ProcessResponse<TResponse>(BaseResponse<TResponse> response)
    {
      // TODO: [TESTS] (BaseController.ProcessResponse) Add tests
      if (response.Validation.FailedValidation())
      {

      }

      return Ok(response.Response);
    }
  }
}
