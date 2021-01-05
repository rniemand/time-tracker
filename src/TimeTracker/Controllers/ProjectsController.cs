﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using Rn.NetCore.Common.Logging;
using Rn.NetCore.Common.Metrics;
using TimeTracker.Core.Models.Dto;
using TimeTracker.Core.Services;
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
      IProjectService projectService
    ) : base(logger, metrics)
    {
      _projectService = projectService;
    }

    [HttpGet, Route("projects/product/{productId}"), Authorize]
    public async Task<ActionResult<List<ProjectDto>>> GetAllForProduct(
      [FromRoute] int productId,
      [OpenApiIgnore] CoreApiRequest request)
    {
      // TODO: [TESTS] (ProjectsController.GetAllForProduct) Add tests
      return Ok(await _projectService.GetAllForProduct(request.UserId, productId));
    }

    [HttpGet, Route("projects/list/product/{productId}"), Authorize]
    public async Task<ActionResult<List<IntListItem>>> GetProjectsAsList(
      [FromRoute] int productId,
      [OpenApiIgnore] CoreApiRequest request)
    {
      // TODO: [TESTS] (ProjectsController.GetProjectsAsList) Add tests
      return Ok(await _projectService.GetProjectsAsList(request.UserId, productId));
    }


    [HttpGet, Route("project/{projectId}"), Authorize]
    public async Task<ActionResult<ProjectDto>> GetById(
      [FromRoute] int projectId,
      [OpenApiIgnore] CoreApiRequest request)
    {
      // TODO: [TESTS] (ProjectsController.GetById) Add tests
      return Ok(await _projectService.GetById(request.UserId, projectId));
    }

    [HttpPost, Route("project/add"), Authorize]
    public async Task<ActionResult<ProjectDto>> AddProject(
      [FromBody] ProjectDto project,
      [OpenApiIgnore] CoreApiRequest request)
    {
      // TODO: [TESTS] (ProjectsController.AddProject) Add tests
      return Ok(await _projectService.AddProject(request.UserId, project));
    }

    [HttpPut, Route("project/update"), Authorize]
    public async Task<ActionResult<ProjectDto>> UpdateProject(
      [FromBody] ProjectDto project,
      [OpenApiIgnore] CoreApiRequest request)
    {
      // TODO: [TESTS] (ProjectsController.UpdateProject) Add tests
      return Ok(await _projectService.UpdateProject(request.UserId, project));
    }
  }
}
