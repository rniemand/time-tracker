using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using TimeTracker.Core.Models.Dto;
using TimeTracker.Core.Services;
using TimeTracker.Core.WebApi.Attributes;
using TimeTracker.Core.WebApi.Requests;

namespace TimeTracker.Controllers
{
  [ApiController, Route("api/[controller]")]
  public class TrackedTimeController : ControllerBase
  {
    private readonly ITrackedTimeService _trackedTimeService;

    public TrackedTimeController(ITrackedTimeService trackedTimeService)
    {
      _trackedTimeService = trackedTimeService;
    }

    [HttpPost, Route("start-new"), Authorize]
    public async Task<ActionResult<RawTrackedTimeDto>> StartNewTimer(
      [FromBody] RawTrackedTimeDto entryDto,
      [OpenApiIgnore] CoreApiRequest request)
    {
      // TODO: [TESTS] (TrackedTimeController.StartNewTimer) Add tests
      return Ok(await _trackedTimeService.StartNew(request.UserId, entryDto));
    }
  }
}
