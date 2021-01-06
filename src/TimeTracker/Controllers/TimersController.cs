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

    [HttpGet, Route("timers/active"), Authorize]
    public async Task<ActionResult<List<RawTimerDto>>> GetActiveTimers(
      [OpenApiIgnore] CoreApiRequest request)
    {
      // TODO: [TESTS] (TimersController.GetActiveTimers) Add tests
      var response = new BaseResponse<List<RawTimerDto>>()
        .WithResponse(await _rawTimerService.GetActiveTimers(request.UserId));

      return ProcessResponse(response);
    }

    [HttpPost, Route("timer/start-new"), Authorize]
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
        response.WithResponse(await _rawTimerService.PauseTimer(
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
        response.WithResponse(await _rawTimerService.ResumeTimer(
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
        response.WithResponse(await _rawTimerService.StopTimer(
          request.UserId,
          rawTimerId
        ));

      return ProcessResponse(response);
    }

    [HttpGet, Route("timer/{rootTimerId}/series"), Authorize]
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
        response.WithResponse(await _rawTimerService.UpdateNotes(
          request.UserId,
          rawTimerId,
          notes
        ));

      return ProcessResponse(response);
    }

    [HttpPost, Route("timer/{rawTimerId}/update-duration"), Authorize]
    public async Task<ActionResult<bool>> UpdateTimerDuration(
      [FromRoute] long rawTimerId,
      [FromBody] RawTimerDto rawTimerDto,
      [OpenApiIgnore] CoreApiRequest request)
    {
      // TODO: [TESTS] (TimersController.UpdateTimerDuration) Add tests
      var response = new BaseResponse<bool>()
        .WithValidation(RawTimerDtoValidator.UpdateTimerDuration(rawTimerDto));

      if (response.PassedValidation)
        response.WithResponse(await _rawTimerService.UpdateTimerDuration(
          request.UserId,
          rawTimerDto
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
        response.WithResponse(await _rawTimerService.ResumeSingleTimer(
          request.UserId,
          rawTimerId
        ));

      return ProcessResponse(response);
    }
  }
}
