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
  public class ProjectsController : BaseController<ProjectsController>
  {
    private readonly IProjectService _projectService;

    public ProjectsController(
      ILoggerAdapter<ProjectsController> logger,
      IMetricService metrics,
      IUserService userService,
      IProjectService projectService
    ) : base(logger, metrics, userService)
    {
      _projectService = projectService;
    }

    [HttpGet, Route("projects/product/{productId}"), Authorize]
    public async Task<ActionResult<List<ProjectDto>>> GetProductProjects(
      [FromRoute] int productId,
      [OpenApiIgnore] CoreApiRequest request)
    {
      // TODO: [TESTS] (ProjectsController.GetProductProjects) Add tests
      var response = new BaseResponse<List<ProjectDto>>()
        .WithValidation(new AdHockValidator()
          .GreaterThanZero(nameof(productId), productId)
        );

      if (response.PassedValidation)
        response.WithResponse(await _projectService.GetAllForProduct(request.UserId, productId));

      return ProcessResponse(response);
    }

    [HttpGet, Route("projects/product/{productId}/list"), Authorize]
    public async Task<ActionResult<List<IntListItem>>> ListProductProjects(
      [FromRoute] int productId,
      [OpenApiIgnore] CoreApiRequest request)
    {
      // TODO: [TESTS] (ProjectsController.ListProductProjects) Add tests
      var response = new BaseResponse<List<IntListItem>>()
        .WithValidation(new AdHockValidator()
          .GreaterThanZero(nameof(productId), productId)
        );

      if (response.PassedValidation)
        response.WithResponse(await _projectService.GetProjectsAsList(
          request.UserId,
          productId
        ));

      return ProcessResponse(response);
    }

    [HttpGet, Route("project/{projectId}"), Authorize]
    public async Task<ActionResult<ProjectDto>> GetProjectById(
      [FromRoute] int projectId,
      [OpenApiIgnore] CoreApiRequest request)
    {
      // TODO: [TESTS] (ProjectsController.GetProjectById) Add tests
      var response = new BaseResponse<ProjectDto>()
        .WithValidation(new AdHockValidator()
          .GreaterThanZero(nameof(projectId), projectId)
        );

      if (response.PassedValidation)
        response.WithResponse(await _projectService.GetById(
          request.UserId,
          projectId
        ));

      return ProcessResponse(response);
    }

    [HttpPost, Route("project/add"), Authorize]
    public async Task<ActionResult<bool>> AddProject(
      [FromBody] ProjectDto projectDto,
      [OpenApiIgnore] CoreApiRequest request)
    {
      // TODO: [TESTS] (ProjectsController.AddProject) Add tests
      var response = new BaseResponse<bool>()
        .WithValidation(ProjectDtoValidator.Add(projectDto));

      if (response.PassedValidation)
        response.WithResponse(await _projectService.AddProject(request.UserId, projectDto));

      return ProcessResponse(response);
    }

    [HttpPut, Route("project/update"), Authorize]
    public async Task<ActionResult<bool>> UpdateProject(
      [FromBody] ProjectDto projectDto,
      [OpenApiIgnore] CoreApiRequest request)
    {
      // TODO: [TESTS] (ProjectsController.UpdateProject) Add tests
      var response = new BaseResponse<bool>()
        .WithValidation(ProjectDtoValidator.Update(projectDto));

      if (response.PassedValidation)
        response.WithResponse(await _projectService.UpdateProject(request.UserId, projectDto));

      return ProcessResponse(response);
    }
  }
}
