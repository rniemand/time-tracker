using System.Collections.Generic;
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
  public class TimersController : ControllerBase
  {
    private readonly IRawTimerService _rawTimerService;

    public TimersController(IRawTimerService rawTimerService)
    {
      _rawTimerService = rawTimerService;
    }

    [HttpPost, Route("start-new"), Authorize]
    public async Task<ActionResult<RawTimerDto>> StartNewTimer(
      [FromBody] RawTimerDto entryDto,
      [OpenApiIgnore] CoreApiRequest request)
    {
      // TODO: [TESTS] (TimersController.StartNewTimer) Add tests
      return Ok(await _rawTimerService.StartNew(request.UserId, entryDto));
    }

    [HttpGet, Route("list-running"), Authorize]
    public async Task<ActionResult<List<RawTimerDto>>> GetRunningTimers(
      [OpenApiIgnore] CoreApiRequest request)
    {
      // TODO: [TESTS] (TimersController.GetRunningTimers) Add tests
      return Ok(await _rawTimerService.GetRunningTimers(request.UserId));
    }

    [HttpGet, Route("pause-timer/{rawTimerId}"), Authorize]
    public async Task<ActionResult<RawTimerDto>> PauseTimer(
      [FromRoute] long rawTimerId,
      [OpenApiIgnore] CoreApiRequest request)
    {
      // TODO: [TESTS] (TimersController.PauseTimer) Add tests
      return Ok(await _rawTimerService.PauseTimer(request.UserId, rawTimerId, "user-paused"));
    }

    [HttpGet, Route("resume-timer/{rawTimerId}"), Authorize]
    public async Task<ActionResult<bool>> ResumeTimer(
      [FromRoute] long rawTimerId,
      [OpenApiIgnore] CoreApiRequest request)
    {
      // TODO: [TESTS] (TimersController.ResumeTimer) Add tests
      return Ok(await _rawTimerService.ResumeTimer(request.UserId, rawTimerId));
    }

    [HttpGet, Route("stop-timer/{rawTimerId}"), Authorize]
    public async Task<ActionResult<bool>> StopTimer(
      [FromRoute] long rawTimerId,
      [OpenApiIgnore] CoreApiRequest request)
    {
      // TODO: [TESTS] (TimersController.StopTimer) Add tests
      return Ok(await _rawTimerService.StopTimer(request.UserId, rawTimerId));
    }

    [HttpGet, Route("timer-series/{rootTimerId}"), Authorize]
    public async Task<ActionResult<List<RawTimerDto>>> GetTimerSeries(
      [FromRoute] long rootTimerId,
      [OpenApiIgnore] CoreApiRequest request)
    {
      // TODO: [TESTS] (TimersController.GetTimerSeries) Add tests
      return Ok(await _rawTimerService.GetTimerSeries(request.UserId, rootTimerId));
    }

    [HttpPost, Route("update-notes/{rawTimerId}"), Authorize]
    public async Task<ActionResult<bool>> UpdateNotes(
      [FromRoute] long rawTimerId,
      [FromBody] string notes,
      [OpenApiIgnore] CoreApiRequest request)
    {
      // TODO: [TESTS] (TimersController.UpdateNotes) Add tests
      return Ok(await _rawTimerService.UpdateNotes(request.UserId, rawTimerId, notes));
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
