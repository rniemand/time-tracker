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
  public class DailyTasksController : BaseController<ClientsController>
  {
    private readonly IDailyTasksService _tasksService;

    public DailyTasksController(
      ILoggerAdapter<ClientsController> logger,
      IMetricService metrics,
      IUserService userService,
      IDailyTasksService tasksService)
      : base(logger, metrics, userService)
    {
      _tasksService = tasksService;
    }

    [HttpGet, Route("client/{clientId}"), Authorize]
    public async Task<ActionResult<List<DailyTaskDto>>> GetClientTasks(
      [FromRoute] int clientId,
      [OpenApiIgnore] CoreApiRequest request)
    {
      // TODO: [TESTS] (DailyTasksController.GetClientTasks) Add tests
      var response = new BaseResponse<List<DailyTaskDto>>()
        .WithResponse(await _tasksService.ListClientTasks(request.UserId, clientId));

      return ProcessResponse(response);
    }


    [HttpPost, Route("task/add"), Authorize]
    public async Task<ActionResult<bool>> AddDailyTask(
      [FromBody] DailyTaskDto taskDto,
      [OpenApiIgnore] CoreApiRequest request)
    {
      // TODO: [TESTS] (DailyTasksController.AddDailyTask) Add tests
      var response = new BaseResponse<bool>()
        .WithValidation(DailyTaskDtoValidator.Add(taskDto));

      if (response.PassedValidation)
        response.WithResponse(await _tasksService.AddDailyTask(request.UserId, taskDto));

      return ProcessResponse(response);
    }

    [HttpGet, Route("task/{taskId}"), Authorize]
    public async Task<ActionResult<DailyTaskDto>> GetTaskById(
      [FromRoute] int taskId,
      [OpenApiIgnore] CoreApiRequest request)
    {
      // TODO: [TESTS] (DailyTasksController.GetTaskById) Add tests
      var response = new BaseResponse<DailyTaskDto>()
        .WithValidation(new AdHockValidator().GreaterThanZero(nameof(taskId), taskId));

      if (response.PassedValidation)
        response.WithResponse(await _tasksService.GetTaskById(request.UserId, taskId));

      return ProcessResponse(response);
    }

    [HttpPost, Route("task/update"), Authorize]
    public async Task<ActionResult<bool>> UpdateTask(
      [FromBody] DailyTaskDto taskDto,
      [OpenApiIgnore] CoreApiRequest request)
    {
      // TODO: [TESTS] (DailyTasksController.UpdateTask) Add tests
      var response = new BaseResponse<bool>()
        .WithValidation(DailyTaskDtoValidator.Update(taskDto));

      if (response.PassedValidation)
        response.WithResponse(await _tasksService.UpdateTask(request.UserId, taskDto));

      return ProcessResponse(response);
    }
  }
}
