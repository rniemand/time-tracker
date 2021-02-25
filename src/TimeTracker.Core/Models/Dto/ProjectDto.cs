using System;
using System.Linq.Expressions;
using FluentValidation;
using FluentValidation.Results;
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
    public DateTime DateAddedUtc { get; set; }
    public DateTime? DateUpdatedUtc { get; set; }
    public DateTime? DateDeletedUtc { get; set; }
    public string ProjectName { get; set; }

    public static Expression<Func<ProjectEntity, ProjectDto>> Projection
    {
      get
      {
        return entity => new ProjectDto
        {
          UserId = entity.UserId,
          DateAddedUtc = entity.DateAddedUtc,
          ClientId = entity.ClientId,
          DateUpdatedUtc = entity.DateUpdatedUtc,
          Deleted = entity.Deleted,
          ProductId = entity.ProductId,
          ProjectId = entity.ProjectId,
          ProjectName = entity.ProjectName,
          DateDeletedUtc = entity.DateDeletedUtc
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
      DateAddedUtc = DateTime.UtcNow;
      DateUpdatedUtc = null;
      DateDeletedUtc = null;
      ProjectName = string.Empty;
    }

    public ProjectEntity AsProjectEntity()
    {
      // TODO: [TESTS] (ProjectDto.AsProjectEntity) Add tests
      return new ProjectEntity
      {
        UserId = UserId,
        DateAddedUtc = DateAddedUtc,
        ClientId = ClientId,
        DateUpdatedUtc = DateUpdatedUtc,
        Deleted = Deleted,
        ProductId = ProductId,
        ProjectId = ProjectId,
        ProjectName = ProjectName,
        DateDeletedUtc = DateDeletedUtc
      };
    }
  }

  public class ProjectDtoValidator : AbstractValidator<ProjectDto>
  {
    public ProjectDtoValidator()
    {
      RuleSet("Add", () =>
      {
        RuleFor(x => x.ClientId).GreaterThan(0);
        RuleFor(x => x.ProductId).GreaterThan(0);
        RuleFor(x => x.UserId).GreaterThan(0);
        RuleFor(x => x.ProjectName).NotNull().MinimumLength(3);
      });

      RuleSet("Update", () =>
      {
        RuleFor(x => x.ProjectName).NotNull().MinimumLength(3);
        RuleFor(x => x.ProjectId).GreaterThan(0);
      });
    }

    public static ValidationResult Add(ProjectDto projectDto)
    {
      return new ProjectDtoValidator().Validate(projectDto,
        options => options.IncludeRuleSets("Add")
      );
    }

    public static ValidationResult Update(ProjectDto projectDto)
    {
      return new ProjectDtoValidator().Validate(projectDto,
        options => options.IncludeRuleSets("Update")
      );
    }
  }
}
