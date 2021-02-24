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
  public class TimeSheetController : BaseController<TimeSheetController>
  {
    private readonly ITimeSheetService _timeSheetService;

    public TimeSheetController(
      ILoggerAdapter<TimeSheetController> logger,
      IMetricService metrics,
      IUserService userService,
      ITimeSheetService timeSheetService)
      : base(logger, metrics, userService)
    {
      _timeSheetService = timeSheetService;
    }

    [HttpPost, Route("get"), Authorize]
    public async Task<ActionResult<GetTimeSheetResponse>> GetTimeSheet(
      [FromBody] GetTimeSheetRequest timeSheetRequest)
    {
      // TODO: [TESTS] (TimeSheetController.GetTimeSheet) Add tests
      var response = new BaseResponse<GetTimeSheetResponse>()
        .WithResponse(await _timeSheetService.GetTimeSheet(timeSheetRequest));

      return ProcessResponse(response);
    }

    [HttpPost, Route("update-entry"), Authorize]
    public async Task<ActionResult<GetTimeSheetResponse>> UpdateEntry(
      [FromBody] AddTimeSheetEntryRequest addRequest)
    {
      // TODO: [TESTS] (TimeSheetController.UpdateEntry) Add tests
      var apiResponse = new BaseResponse<GetTimeSheetResponse>()
        .WithValidation(UpdateTimeSheetEntryRequestValidator.Default(addRequest));

      if (apiResponse.PassedValidation)
      {
        apiResponse.WithResponse(await _timeSheetService.UpdateEntry(addRequest));
      }

      return ProcessResponse(apiResponse);
    }
  }
}
