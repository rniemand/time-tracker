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

    
    // Timer methods (GLOBAL)
    [HttpPost, Route("timer/start-new"), Authorize]
    public async Task<ActionResult<bool>> StartNew(
      [FromBody] TrackedTimeDto timer,
      [OpenApiIgnore] CoreApiRequest request)
    {
      // TODO: [TESTS] (TimersController.StartNew) Add tests
      var response = new BaseResponse<bool>()
        .WithValidation(TrackedTimeDtoValidator.StartNew(timer));

      if (response.PassedValidation)
        response.WithResponse(await _timerService.StartNew(request.UserId, timer));

      return ProcessResponse(response);
    }

    
    // Timer methods (entryId)
    [HttpPost, Route("timer/{entryId}/update-duration"), Authorize]
    public async Task<ActionResult<bool>> UpdateTimerDuration(
      [FromRoute] long entryId,
      [FromBody] TrackedTimeDto timer,
      [OpenApiIgnore] CoreApiRequest request)
    {
      // TODO: [TESTS] (TimersController.UpdateTimerDuration) Add tests
      var response = new BaseResponse<bool>()
        .WithValidation(TrackedTimeDtoValidator.UpdateDuration(timer));

      if (response.PassedValidation)
        response.WithResponse(await _timerService.UpdateDuration(request.UserId, timer));

      return ProcessResponse(response);
    }

    [HttpPost, Route("timer/{entryId}/resume-single"), Authorize]
    public async Task<ActionResult<bool>> ResumeSingleTimer(
      [FromRoute] long entryId,
      [OpenApiIgnore] CoreApiRequest request)
    {
      // TODO: [TESTS] (TimersController.ResumeSingleTimer) Add tests
      var response = new BaseResponse<bool>()
        .WithValidation(new AdHockValidator().GreaterThanZero(nameof(entryId), entryId));

      if (response.PassedValidation)
        response.WithResponse(await _timerService.ResumeSingle(request.UserId, entryId));

      return ProcessResponse(response);
    }

    [HttpPost, Route("timer/{entryId}/stop"), Authorize]
    public async Task<ActionResult<bool>> StopTimer(
      [FromRoute] long entryId,
      [OpenApiIgnore] CoreApiRequest request)
    {
      // TODO: [TESTS] (TimersController.StopTimer) Add tests
      var response = new BaseResponse<bool>()
        .WithValidation(new AdHockValidator().GreaterThanZero(nameof(entryId), entryId));

      if (response.PassedValidation)
        response.WithResponse(await _timerService.Stop(request.UserId, entryId));

      return ProcessResponse(response);
    }

    [HttpPost, Route("timer/{entryId}/resume"), Authorize]
    public async Task<ActionResult<bool>> ResumeTimer(
      [FromRoute] long entryId,
      [OpenApiIgnore] CoreApiRequest request)
    {
      // TODO: [TESTS] (TimersController.ResumeTimer) Add tests
      var response = new BaseResponse<bool>()
        .WithValidation(new AdHockValidator().GreaterThanZero(nameof(entryId), entryId));

      if (response.PassedValidation)
        response.WithResponse(await _timerService.Resume(request.UserId, entryId));

      return ProcessResponse(response);
    }

    [HttpPost, Route("timer/{entryId}/pause"), Authorize]
    public async Task<ActionResult<bool>> PauseTimer(
      [FromRoute] long entryId,
      [OpenApiIgnore] CoreApiRequest request)
    {
      // TODO: [TESTS] (TimersController.PauseTimer) Add tests
      var response = new BaseResponse<bool>()
        .WithValidation(new AdHockValidator().GreaterThanZero(nameof(entryId), entryId));

      if (response.PassedValidation)
        response.WithResponse(await _timerService.Pause(
          request.UserId,
          entryId,
          TimerEndReason.UserPaused,
          "user-paused"
        ));

      return ProcessResponse(response);
    }


    // Timer(s) methods
    [HttpGet, Route("timers/project/{projectId}"), Authorize]
    public async Task<ActionResult<List<TrackedTimeDto>>> GetProjectEntries(
      [FromRoute] int projectId,
      [OpenApiIgnore] CoreApiRequest request)
    {
      // TODO: [TESTS] (TimersController.GetProjectEntries) Add tests
      var response = new BaseResponse<List<TrackedTimeDto>>()
        .WithValidation(new AdHockValidator().GreaterThanZero(nameof(projectId), projectId));

      if (response.PassedValidation)
        response.WithResponse(await _timerService.GetProjectEntries(request.UserId, projectId));

      return ProcessResponse(response);
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
  }
}
