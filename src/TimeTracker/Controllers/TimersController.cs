using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using Rn.NetCore.Common.Logging;
using Rn.NetCore.Common.Metrics;
using TimeTracker.Core.Enums;
using TimeTracker.Core.Models.Dto;
using TimeTracker.Core.Models.Responses;
using TimeTracker.Core.Services;
using TimeTracker.Core.WebApi;
using TimeTracker.Core.WebApi.Attributes;
using TimeTracker.Core.WebApi.Requests;

namespace TimeTracker.Controllers
{
  [ApiController, Route("api/[controller]")]
  public class TimersController : BaseController<TimersController>
  {
    private readonly ITrackedTimeService _timerService;

    public TimersController(
      ILoggerAdapter<TimersController> logger,
      IMetricService metrics,
      IUserService userService,
      ITrackedTimeService timerService
    ) : base(logger, metrics, userService)
    {
      _timerService = timerService;
    }

    [HttpGet, Route("timers/active"), Authorize]
    public async Task<ActionResult<List<TrackedTimeDto>>> GetActiveTimers(
      [OpenApiIgnore] CoreApiRequest request)
    {
      // TODO: [TESTS] (TimersController.GetActiveTimers) Add tests
      var response = new BaseResponse<List<TrackedTimeDto>>()
        .WithResponse(await _timerService.GetActive(request.UserId));

      return ProcessResponse(response);
    }

    [HttpPost, Route("timer/start-new"), Authorize]
    public async Task<ActionResult<bool>> StartNewTimer(
      [FromBody] TrackedTimeDto trackedTimeDto,
      [OpenApiIgnore] CoreApiRequest request)
    {
      // TODO: [TESTS] (TimersController.StartNewTimer) Add tests
      var response = new BaseResponse<bool>()
        .WithValidation(TrackedTimeDtoValidator.StartNew(trackedTimeDto));

      if (response.PassedValidation)
        response.WithResponse(await _timerService.StartNew(
          request.UserId,
          trackedTimeDto
        ));

      return ProcessResponse(response);
    }

    [HttpGet, Route("timer/{rawTimerId}/pause"), Authorize]
    public async Task<ActionResult<bool>> PauseTimer(
      [FromRoute] long rawTimerId,
      [OpenApiIgnore] CoreApiRequest request)
    {
      // TODO: [TESTS] (TimersController.PauseTimer) Add tests
      var response = new BaseResponse<bool>()
        .WithValidation(new AdHockValidator()
          .GreaterThanZero(nameof(rawTimerId), rawTimerId)
        );

      if (response.PassedValidation)
        response.WithResponse(await _timerService.Pause(
          request.UserId,
          rawTimerId,
          EntryRunningState.Paused,
          "user-paused"
        ));

      return ProcessResponse(response);
    }

    [HttpGet, Route("timer/{rawTimerId}/resume"), Authorize]
    public async Task<ActionResult<bool>> ResumeTimer(
      [FromRoute] long rawTimerId,
      [OpenApiIgnore] CoreApiRequest request)
    {
      // TODO: [TESTS] (TimersController.ResumeTimer) Add tests
      var response = new BaseResponse<bool>()
        .WithValidation(new AdHockValidator()
          .GreaterThanZero(nameof(rawTimerId), rawTimerId)
        );

      if (response.PassedValidation)
        response.WithResponse(await _timerService.Resume(
          request.UserId,
          rawTimerId
        ));

      return ProcessResponse(response);
    }

    [HttpGet, Route("timer/{rawTimerId}/stop"), Authorize]
    public async Task<ActionResult<bool>> StopTimer(
      [FromRoute] long rawTimerId,
      [OpenApiIgnore] CoreApiRequest request)
    {
      // TODO: [TESTS] (TimersController.StopTimer) Add tests
      var response = new BaseResponse<bool>()
        .WithValidation(new AdHockValidator()
          .GreaterThanZero(nameof(rawTimerId), rawTimerId)
        );

      if (response.PassedValidation)
        response.WithResponse(await _timerService.Stop(
          request.UserId,
          rawTimerId
        ));

      return ProcessResponse(response);
    }

    [HttpGet, Route("timer/{rootTimerId}/series"), Authorize]
    public async Task<ActionResult<List<TrackedTimeDto>>> GetTimerSeries(
      [FromRoute] long rootTimerId,
      [OpenApiIgnore] CoreApiRequest request)
    {
      // TODO: [TESTS] (TimersController.GetTimerSeries) Add tests
      var response = new BaseResponse<List<TrackedTimeDto>>()
        .WithValidation(new AdHockValidator()
          .GreaterThanZero(nameof(rootTimerId), rootTimerId)
        );

      if (response.PassedValidation)
        response.WithResponse(await _timerService.GetProjectEntries(
          request.UserId,
          rootTimerId
        ));

      return ProcessResponse(response);
    }

    [HttpPost, Route("timer/{rawTimerId}/update-notes"), Authorize]
    public async Task<ActionResult<bool>> UpdateNotes(
      [FromRoute] long rawTimerId,
      [FromBody] string notes,
      [OpenApiIgnore] CoreApiRequest request)
    {
      // TODO: [TESTS] (TimersController.UpdateNotes) Add tests
      var response = new BaseResponse<bool>()
        .WithValidation(new AdHockValidator()
          .GreaterThanZero(nameof(rawTimerId), rawTimerId)
          .NotNullOrWhiteSpace(nameof(notes), notes)
        );

      if (response.PassedValidation)
        response.WithResponse(await _timerService.UpdateNotes(
          request.UserId,
          rawTimerId,
          notes
        ));

      return ProcessResponse(response);
    }

    [HttpPost, Route("timer/{rawTimerId}/update-duration"), Authorize]
    public async Task<ActionResult<bool>> UpdateTimerDuration(
      [FromRoute] long rawTimerId,
      [FromBody] TrackedTimeDto trackedTimeDto,
      [OpenApiIgnore] CoreApiRequest request)
    {
      // TODO: [TESTS] (TimersController.UpdateTimerDuration) Add tests
      var response = new BaseResponse<bool>()
        .WithValidation(TrackedTimeDtoValidator.UpdateTimerDuration(trackedTimeDto));

      if (response.PassedValidation)
        response.WithResponse(await _timerService.UpdateDuration(
          request.UserId,
          trackedTimeDto
        ));

      return ProcessResponse(response);
    }

    [HttpGet, Route("timer/{rawTimerId}/resume-single"), Authorize]
    public async Task<ActionResult<bool>> ResumeSingleTimer(
      [FromRoute] long rawTimerId,
      [OpenApiIgnore] CoreApiRequest request)
    {
      // TODO: [TESTS] (TimersController.ResumeSingleTimer) Add tests
      var response = new BaseResponse<bool>()
        .WithValidation(new AdHockValidator().GreaterThanZero(nameof(rawTimerId), rawTimerId));

      if (response.PassedValidation)
        response.WithResponse(await _timerService.ResumeSingle(
          request.UserId,
          rawTimerId
        ));

      return ProcessResponse(response);
    }
  }
}
