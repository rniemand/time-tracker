using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using Rn.NetCore.Common.Logging;
using Rn.NetCore.Common.Metrics;
using TimeTracker.Core.Models.Requests;
using TimeTracker.Core.Models.Responses;
using TimeTracker.Core.Services;
using TimeTracker.Core.WebApi.Attributes;
using TimeTracker.Core.WebApi.Requests;

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
      [FromBody] GetTimeSheetRequest timeSheetRequest,
      [OpenApiIgnore] CoreApiRequest request)
    {
      // TODO: [TESTS] (TimeSheetController.GetTimeSheet) Add tests
      var response = new BaseResponse<GetTimeSheetResponse>()
        .WithResponse(await _timeSheetService.GetTimeSheet(timeSheetRequest, request.UserId));

      return ProcessResponse(response);
    }

    [HttpPost, Route("add-row"), Authorize]
    public async Task<ActionResult<GetTimeSheetResponse>> AddRow(
      [FromBody] AddTimeSheetRowRequest addRequest)
    {
      // TODO: [TESTS] (TimeSheetController.AddRow) Add tests
      var apiResponse = new BaseResponse<GetTimeSheetResponse>()
        .WithValidation(AddTimeSheetRowRequestValidator.Update(addRequest));

      if (apiResponse.PassedValidation)
      {
        apiResponse.WithResponse(await _timeSheetService.AddTimeSheetRow(addRequest));
      }

      return ProcessResponse(apiResponse);
    }
  }
}
