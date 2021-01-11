using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Rn.NetCore.Common.Logging;
using Rn.NetCore.Common.Metrics;
using Rn.NetCore.Common.Metrics.Builders;
using TimeTracker.Core.Database.Entities;
using TimeTracker.Core.Database.Repos;
using TimeTracker.Core.Enums;
using TimeTracker.Core.Models.Dto;

namespace TimeTracker.Core.Services
{
  public interface IProjectService
  {
    Task<List<ProjectDto>> GetAllForProduct(int userId, int productId);
    Task<ProjectDto> GetById(int userId, int projectId);
    Task<bool> AddProject(int userId, ProjectDto projectDto);
    Task<bool> UpdateProject(int userId, ProjectDto projectDto);
    Task<List<IntListItem>> GetProjectsAsList(int userId, int productId);
  }

  public class ProjectService : IProjectService
  {
    private readonly ILoggerAdapter<ProjectService> _logger;
    private readonly IMetricService _metrics;
    private readonly IProjectRepo _projectRepo;

    public ProjectService(
      ILoggerAdapter<ProjectService> logger,
      IMetricService metrics,
      IProjectRepo projectRepo)
    {
      _projectRepo = projectRepo;
      _logger = logger;
      _metrics = metrics;
    }

    public async Task<List<ProjectDto>> GetAllForProduct(int userId, int productId)
    {
      // TODO: [TESTS] (ProjectService.GetAllForProduct) Add tests
      // TODO: [VALIDATION] (ProjectService.GetAllForProduct) Ensure that the current user can see these
      var builder = new ServiceMetricBuilder(nameof(ProjectService), nameof(GetAllForProduct))
        .WithCategory(MetricCategory.Project, MetricSubCategory.GetAll)
        .WithCustomInt1(userId)
        .WithCustomInt2(productId)
        .WithCustomInt3(0);

      try
      {
        using (builder.WithTiming())
        {
          List<ProjectEntity> dbEntries;
          using (builder.WithCustomTiming1())
          {
            builder.IncrementQueryCount();
            dbEntries = await _projectRepo.GetAllForProduct(productId);
            builder.WithResultCount(dbEntries.Count);
          }

          return dbEntries
            .AsQueryable()
            .Select(ProjectDto.Projection)
            .ToList();
        }
      }
      catch (Exception ex)
      {
        _logger.LogUnexpectedException(ex);
        builder.WithException(ex);
        return new List<ProjectDto>();
      }
      finally
      {
        await _metrics.SubmitPointAsync(builder);
      }
    }

    public async Task<ProjectDto> GetById(int userId, int projectId)
    {
      // TODO: [TESTS] (ProjectService.GetById) Add tests
      var builder = new ServiceMetricBuilder(nameof(ProjectService), nameof(GetById))
        .WithCategory(MetricCategory.Project, MetricSubCategory.GetById)
        .WithCustomInt1(userId)
        .WithCustomInt2(0)
        .WithCustomInt3(projectId);

      try
      {
        using (builder.WithTiming())
        {
          ProjectEntity dbEntry;
          using (builder.WithCustomTiming1())
          {
            builder.IncrementQueryCount();
            dbEntry = await _projectRepo.GetById(projectId);
            builder.CountResult(dbEntry);
          }

          if (dbEntry == null)
          {
            // TODO: [HANDLE] (ProjectService.GetById) Handle this
            return null;
          }

          builder.WithCustomInt2(dbEntry.ProductId);
          if (dbEntry.UserId != userId)
          {
            // TODO: [HANDLE] (ProjectService.GetById) Handle this
            return null;
          }

          return ProjectDto.FromEntity(dbEntry);
        }
      }
      catch (Exception ex)
      {
        _logger.LogUnexpectedException(ex);
        builder.WithException(ex);
        return null;
      }
      finally
      {
        await _metrics.SubmitPointAsync(builder);
      }
    }

    public async Task<bool> AddProject(int userId, ProjectDto projectDto)
    {
      // TODO: [TESTS] (ProjectService.AddProject) Add tests
      // TODO: [VALIDATION] (ProjectService.AddProject) Add validation
      var builder = new ServiceMetricBuilder(nameof(ProjectService), nameof(AddProject))
        .WithCategory(MetricCategory.Project, MetricSubCategory.Add)
        .WithCustomInt1(userId)
        .WithCustomInt2(projectDto.ProductId)
        .WithCustomInt3(0);

      try
      {
        using (builder.WithTiming())
        {
          var projectEntity = projectDto.AsProjectEntity();
          projectEntity.UserId = userId;

          using (builder.WithCustomTiming1())
          {
            builder.IncrementQueryCount();
            if (await _projectRepo.Add(projectEntity) <= 0)
              return false;

            builder.WithResultCount(1);
            return true;
          }
        }
      }
      catch (Exception ex)
      {
        _logger.LogUnexpectedException(ex);
        builder.WithException(ex);
        return false;
      }
      finally
      {
        await _metrics.SubmitPointAsync(builder);
      }
    }

    public async Task<bool> UpdateProject(int userId, ProjectDto projectDto)
    {
      // TODO: [TESTS] (ProjectService.UpdateProject) Add tests
      // TODO: [VALIDATE] (ProjectService.UpdateProject) Ensure that the user can edit this
      var builder = new ServiceMetricBuilder(nameof(ProjectService), nameof(UpdateProject))
        .WithCategory(MetricCategory.Project, MetricSubCategory.Update)
        .WithCustomInt1(userId)
        .WithCustomInt2(projectDto.ProductId)
        .WithCustomInt3(projectDto.ProjectId);

      try
      {
        using (builder.WithTiming())
        {
          ProjectEntity dbEntry;
          using (builder.WithCustomTiming1())
          {
            builder.IncrementQueryCount();
            dbEntry = await _projectRepo.GetById(projectDto.ProjectId);
            builder.CountResult(dbEntry);
          }

          if (dbEntry == null)
          {
            // TODO: [HANDLE] (ProjectService.UpdateProject) Handle this
            return false;
          }

          if (dbEntry.UserId != userId)
          {
            // TODO: [HANDLE] (ProjectService.UpdateProject) Handle this
            return false;
          }

          var projectEntity = projectDto.AsProjectEntity();
          projectEntity.UserId = userId;

          using (builder.WithCustomTiming2())
          {
            builder.IncrementQueryCount();
            if (await _projectRepo.Update(projectEntity) <= 0)
              return false;

            builder.IncrementResultCount();
            return true;
          }
        }
      }
      catch (Exception ex)
      {
        _logger.LogUnexpectedException(ex);
        builder.WithException(ex);
        return false;
      }
      finally
      {
        await _metrics.SubmitPointAsync(builder);
      }
    }

    public async Task<List<IntListItem>> GetProjectsAsList(int userId, int productId)
    {
      // TODO: [TESTS] (ProjectService.GetProjectsAsList) Add tests
      var builder = new ServiceMetricBuilder(nameof(ProjectService), nameof(GetProjectsAsList))
        .WithCategory(MetricCategory.Project, MetricSubCategory.GetList)
        .WithCustomInt1(userId)
        .WithCustomInt2(productId)
        .WithCustomInt3(0);

      try
      {
        using (builder.WithTiming())
        {
          List<ProjectEntity> dbEntries;
          using (builder.WithCustomTiming1())
          {
            builder.IncrementQueryCount();
            dbEntries = await _projectRepo.GetAllForProduct(productId);
            builder.WithResultCount(dbEntries?.Count ?? 0);
          }

          if (dbEntries == null || dbEntries.Count <= 0)
          {
            // TODO: [HANDLE] (ProjectService.GetProjectsAsList) Handle this
            return new List<IntListItem>();
          }

          if (dbEntries.First().UserId != userId)
          {
            // TODO: [HANDLE] (ProjectService.GetProjectsAsList) Handle this
            return new List<IntListItem>();
          }

          return dbEntries
            .AsQueryable()
            .Select(project => new IntListItem
            {
              Name = project.ProjectName,
              Value = project.ProjectId
            })
            .ToList();
        }
      }
      catch (Exception ex)
      {
        _logger.LogUnexpectedException(ex);
        builder.WithException(ex);
        return new List<IntListItem>();
      }
      finally
      {
        await _metrics.SubmitPointAsync(builder);
      }
    }
  }
}
