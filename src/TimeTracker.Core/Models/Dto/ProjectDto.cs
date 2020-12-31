using System;
using System.Linq.Expressions;
using TimeTracker.Core.Database.Entities;

namespace TimeTracker.Core.Models.Dto
{
  public class ProjectDto
  {
    public int ProjectId { get; set; }
    public int ClientId { get; set; }
    public int ProductId { get; set; }
    public int UserId { get; set; }
    public bool Deleted { get; set; }
    public DateTime DateCreatedUtc { get; set; }
    public DateTime? DateModifiedUtc { get; set; }
    public string ProjectName { get; set; }

    public static Expression<Func<ProjectEntity, ProjectDto>> Projection
    {
      get
      {
        return entity => new ProjectDto
        {
          UserId = entity.UserId,
          DateCreatedUtc = entity.DateCreatedUtc,
          ClientId = entity.ClientId,
          DateModifiedUtc = entity.DateModifiedUtc,
          Deleted = entity.Deleted,
          ProductId = entity.ProductId,
          ProjectId = entity.ProjectId,
          ProjectName = entity.ProjectName
        };
      }
    }

    public static ProjectDto FromEntity(ProjectEntity entity)
    {
      // TODO: [TESTS] (ProjectDto.FromEntity) Add tests
      return entity == null ? null : Projection.Compile()(entity);
    }

    public ProjectDto()
    {
      // TODO: [TESTS] (ProjectDto) Add tests
      ProjectId = 0;
      ClientId = 0;
      ProductId = 0;
      UserId = 0;
      Deleted = false;
      DateCreatedUtc = DateTime.UtcNow;
      DateModifiedUtc = null;
      ProjectName = string.Empty;
    }

    public ProjectEntity AsProjectEntity()
    {
      // TODO: [TESTS] (ProjectDto.AsProjectEntity) Add tests
      return new ProjectEntity
      {
        UserId = UserId,
        DateCreatedUtc = DateCreatedUtc,
        ClientId = ClientId,
        DateModifiedUtc = DateModifiedUtc,
        Deleted = Deleted,
        ProductId = ProductId,
        ProjectId = ProjectId,
        ProjectName = ProjectName
      };
    }
  }
}
