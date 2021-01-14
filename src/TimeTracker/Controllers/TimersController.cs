using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using Rn.NetCore.Common.Logging;
using Rn.NetCore.Common.Metrics;
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
    private readonly ITimerService _timerService;

    public TimersController(
      ILoggerAdapter<TimersController> logger,
      IMetricService metrics,
      IUserService userService,
      ITimerService timerService
    ) : base(logger, metrics, userService)
    {
      _timerService = timerService;
    }


    // Timer methods (GLOBAL)
    [HttpPost, Route("timer/start-new"), Authorize]
    public async Task<ActionResult<bool>> StartNew(
      [FromBody] TimerDto timerDto,
      [OpenApiIgnore] CoreApiRequest request)
    {
      // TODO: [TESTS] (TimersController.StartNew) Add tests
      var response = new BaseResponse<bool>()
        .WithValidation(TrackedTimeDtoValidator.StartNew(timerDto));

      if (response.PassedValidation)
        response.WithResponse(await _timerService.StartTimer(request.UserId, timerDto));

      return ProcessResponse(response);
    }


    // Timer methods (entryId)
    [HttpPost, Route("timer/{entryId}/update-duration"), Authorize]
    public async Task<ActionResult<bool>> UpdateTimerDuration(
      [FromRoute] long entryId,
      [FromBody] TimerDto timerDto,
      [OpenApiIgnore] CoreApiRequest request)
    {
      // TODO: [TESTS] (TimersController.UpdateTimerDuration) Add tests
      var response = new BaseResponse<bool>()
        .WithValidation(TrackedTimeDtoValidator.UpdateDuration(timerDto));

      if (response.PassedValidation)
        response.WithResponse(await _timerService.UpdateTimerDuration(request.UserId, timerDto));

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
        response.WithResponse(await _timerService.ResumeSingleTimer(request.UserId, entryId));

      return ProcessResponse(response);
    }

    [HttpPost, Route("timer/{entryId}/complete"), Authorize]
    public async Task<ActionResult<bool>> CompleteTimer(
      [FromRoute] long entryId,
      [OpenApiIgnore] CoreApiRequest request)
    {
      // TODO: [TESTS] (TimersController.CompleteTimer) Add tests
      var response = new BaseResponse<bool>()
        .WithValidation(new AdHockValidator().GreaterThanZero(nameof(entryId), entryId));

      if (response.PassedValidation)
        response.WithResponse(await _timerService.CompleteTimer(request.UserId, entryId));

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
        response.WithResponse(await _timerService.ResumeTimer(request.UserId, entryId));

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
        response.WithResponse(await _timerService.PauseTimer(request.UserId, entryId));

      return ProcessResponse(response);
    }


    // Timer(s) methods
    [HttpGet, Route("timers/active"), Authorize]
    public async Task<ActionResult<List<TimerDto>>> GetActiveTimers(
      [OpenApiIgnore] CoreApiRequest request)
    {
      // TODO: [TESTS] (TimersController.GetActiveTimers) Add tests
      var response = new BaseResponse<List<TimerDto>>()
        .WithResponse(await _timerService.GetActiveTimers(request.UserId));

      return ProcessResponse(response);
    }

    [HttpGet, Route("timers/project/{projectId}"), Authorize]
    public async Task<ActionResult<List<TimerDto>>> GetProjectEntries(
      [FromRoute] int projectId,
      [OpenApiIgnore] CoreApiRequest request)
    {
      // TODO: [TESTS] (TimersController.GetProjectEntries) Add tests
      var response = new BaseResponse<List<TimerDto>>()
        .WithValidation(new AdHockValidator().GreaterThanZero(nameof(projectId), projectId));

      if (response.PassedValidation)
        response.WithResponse(await _timerService.GetProjectTimers(request.UserId, projectId));

      return ProcessResponse(response);
    }

    [HttpGet, Route("timers/daily-task/{taskId}"), Authorize]
    public async Task<ActionResult<List<TimerDto>>> GetDailyTaskEntries(
      [FromRoute] int taskId,
      [OpenApiIgnore] CoreApiRequest request)
    {
      // TODO: [TESTS] (TimersController.GetDailyTaskEntries) Add tests
      var response = new BaseResponse<List<TimerDto>>()
        .WithValidation(new AdHockValidator().GreaterThanZero(nameof(taskId), taskId));

      if (response.PassedValidation)
        response.WithResponse(await _timerService.GetDailyTaskTimers(request.UserId, taskId));

      return ProcessResponse(response);
    }
  }
}
