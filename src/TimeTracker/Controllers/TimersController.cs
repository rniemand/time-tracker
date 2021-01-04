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

    [HttpGet, Route("pause-timer/{entryId}"), Authorize]
    public async Task<ActionResult<RawTimerDto>> PauseTimer(
      [FromRoute] long entryId,
      [OpenApiIgnore] CoreApiRequest request)
    {
      // TODO: [TESTS] (TimersController.PauseTimer) Add tests
      return Ok(await _rawTimerService.PauseTimer(request.UserId, entryId));
    }

    [HttpGet, Route("resume-timer/{entryId}"), Authorize]
    public async Task<ActionResult<bool>> ResumeTimer(
      [FromRoute] long entryId,
      [OpenApiIgnore] CoreApiRequest request)
    {
      // TODO: [TESTS] (TimersController.ResumeTimer) Add tests
      return Ok(await _rawTimerService.ResumeTimer(request.UserId, entryId));
    }
  }
}
