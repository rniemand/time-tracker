using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TimeTracker.Core.Database.Repos;
using TimeTracker.Core.Models.Dto;

namespace TimeTracker.Core.Services
{
  public interface IProjectService
  {
    Task<List<ProjectDto>> GetAllForProduct(int userId, int productId);
    Task<ProjectDto> GetById(int userId, int projectId);
    Task<ProjectDto> AddProject(int userId, ProjectDto projectDto);
    Task<ProjectDto> UpdateProject(int userId, ProjectDto projectDto);
  }

  public class ProjectService : IProjectService
  {
    private readonly IProjectRepo _projectRepo;

    public ProjectService(IProjectRepo projectRepo)
    {
      _projectRepo = projectRepo;
    }

    public async Task<List<ProjectDto>> GetAllForProduct(int userId, int productId)
    {
      // TODO: [TESTS] (ProjectService.GetAllForProduct) Add tests
      // TODO: [VALIDATION] (ProjectService.GetAllForProduct) Ensure that the current user can see these

      var dbEntries = await _projectRepo.GetAllForProduct(productId);

      return dbEntries
        .AsQueryable()
        .Select(ProjectDto.Projection)
        .ToList();
    }

    public async Task<ProjectDto> GetById(int userId, int projectId)
    {
      // TODO: [TESTS] (ProjectService.GetById) Add tests

      var dbEntry = await _projectRepo.GetById(projectId);
      if (dbEntry == null)
      {
        // TODO: [HANDLE] (ProjectService.GetById) Handle this
        return null;
      }

      if (dbEntry.UserId != userId)
      {
        // TODO: [HANDLE] (ProjectService.GetById) Handle this
        return null;
      }

      return ProjectDto.FromEntity(dbEntry);
    }

    public async Task<ProjectDto> AddProject(int userId, ProjectDto projectDto)
    {
      // TODO: [TESTS] (ProjectService.AddProject) Add tests
      // TODO: [VALIDATION] (ProjectService.AddProject) Add validation

      var projectEntity = projectDto.AsProjectEntity();
      projectEntity.UserId = userId;

      await _projectRepo.Add(projectEntity);
      var dbEntry = await _projectRepo.GetByName(projectEntity.ProductId, projectEntity.ProjectName);

      return ProjectDto.FromEntity(dbEntry);
    }

    public async Task<ProjectDto> UpdateProject(int userId, ProjectDto projectDto)
    {
      // TODO: [TESTS] (ProjectService.UpdateProject) Add tests
      // TODO: [VALIDATE] (ProjectService.UpdateProject) Ensure that the user can edit this

      var dbEntry = await _projectRepo.GetById(projectDto.ProjectId);
      if (dbEntry == null)
      {
        // TODO: [HANDLE] (ProjectService.UpdateProject) Handle this
        return null;
      }

      if (dbEntry.UserId != userId)
      {
        // TODO: [HANDLE] (ProjectService.UpdateProject) Handle this
        return null;
      }

      var projectEntity = projectDto.AsProjectEntity();
      projectEntity.UserId = userId;
      await _projectRepo.Update(projectEntity);

      return ProjectDto.FromEntity(await _projectRepo.GetById(projectDto.ProjectId));
    }
  }
}
