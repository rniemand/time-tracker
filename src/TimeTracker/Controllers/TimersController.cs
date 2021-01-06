using System.Collections.Generic;
using System.Threading.Tasks;
using FluentValidation.Results;
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
    private readonly IRawTimerService _rawTimerService;

    public TimersController(
      ILoggerAdapter<TimersController> logger,
      IMetricService metrics,
      IUserService userService,
      IRawTimerService rawTimerService
    ) : base(logger, metrics, userService)
    {
      _rawTimerService = rawTimerService;
    }

    [HttpPost, Route("start-new"), Authorize]
    public async Task<ActionResult<RawTimerDto>> StartNewTimer(
      [FromBody] RawTimerDto rawTimerDto,
      [OpenApiIgnore] CoreApiRequest request)
    {
      // TODO: [TESTS] (TimersController.StartNewTimer) Add tests
      var response = new BaseResponse<RawTimerDto>()
        .WithValidation(RawTimerDtoValidator.StartNew(rawTimerDto));

      if (response.PassedValidation)
        response.WithResponse(await _rawTimerService.StartNew(request.UserId, rawTimerDto));

      return ProcessResponse(response);
    }

    [HttpGet, Route("list-running"), Authorize]
    public async Task<ActionResult<List<RawTimerDto>>> GetRunningTimers(
      [OpenApiIgnore] CoreApiRequest request)
    {
      // TODO: [TESTS] (TimersController.GetRunningTimers) Add tests
      var response = new BaseResponse<List<RawTimerDto>>()
        .WithResponse(await _rawTimerService.GetRunningTimers(request.UserId));

      return ProcessResponse(response);
    }

    [HttpGet, Route("pause-timer/{rawTimerId}"), Authorize]
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
        response.WithResponse(await _rawTimerService.PauseTimer(
          request.UserId,
          rawTimerId,
          "user-paused"
        ));

      return ProcessResponse(response);
    }

    [HttpGet, Route("resume-timer/{rawTimerId}"), Authorize]
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
        response.WithResponse(await _rawTimerService.ResumeTimer(
          request.UserId,
          rawTimerId
        ));

      return ProcessResponse(response);
    }

    [HttpGet, Route("stop-timer/{rawTimerId}"), Authorize]
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
        response.WithResponse(await _rawTimerService.StopTimer(
          request.UserId,
          rawTimerId
        ));

      return ProcessResponse(response);
    }

    [HttpGet, Route("timer-series/{rootTimerId}"), Authorize]
    public async Task<ActionResult<List<RawTimerDto>>> GetTimerSeries(
      [FromRoute] long rootTimerId,
      [OpenApiIgnore] CoreApiRequest request)
    {
      // TODO: [TESTS] (TimersController.GetTimerSeries) Add tests
      var response = new BaseResponse<List<RawTimerDto>>()
        .WithValidation(new AdHockValidator()
          .GreaterThanZero(nameof(rootTimerId), rootTimerId)
        );

      if (response.PassedValidation)
        response.WithResponse(await _rawTimerService.GetTimerSeries(
          request.UserId,
          rootTimerId
        ));

      return ProcessResponse(response);
    }

    [HttpPost, Route("update-notes/{rawTimerId}"), Authorize]
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
        response.WithResponse(await _rawTimerService.UpdateNotes(
          request.UserId,
          rawTimerId,
          notes
        ));

      return ProcessResponse(response);
    }

    [HttpPost, Route("update-duration"), Authorize]
    public async Task<ActionResult<bool>> UpdateTimerDuration(
      [FromBody] RawTimerDto timerDto,
      [OpenApiIgnore] CoreApiRequest request)
    {
      // TODO: [TESTS] (TimersController.UpdateTimerDuration) Add tests
      return Ok(await _rawTimerService.UpdateTimerDuration(request.UserId, timerDto));
    }
  }
}
