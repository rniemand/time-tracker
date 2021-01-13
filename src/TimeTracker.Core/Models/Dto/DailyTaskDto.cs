using System;
using System.Linq.Expressions;
using FluentValidation;
using FluentValidation.Results;
using TimeTracker.Core.Database.Entities;

namespace TimeTracker.Core.Models.Dto
{
  public class DailyTaskDto
  {
    public int TaskId { get; set; }
    public int UserId { get; set; }
    public int ClientId { get; set; }
    public bool Deleted { get; set; }
    public DateTime DateCreatedUtc { get; set; }
    public DateTime? DateModifiedUtc { get; set; }
    public string TaskName { get; set; }

    public DailyTaskDto()
    {
      // TODO: [TESTS] (DailyTaskDto) Add tests
      TaskId = 0;
      UserId = 0;
      ClientId = 0;
      Deleted = false;
      DateCreatedUtc = DateTime.UtcNow;
      DateModifiedUtc = null;
      TaskName = string.Empty;
    }

    // Helpers
    public static Expression<Func<DailyTaskEntity, DailyTaskDto>> Projection
    {
      get
      {
        return entity => new DailyTaskDto
        {
          UserId = entity.UserId,
          ClientId = entity.ClientId,
          Deleted = entity.Deleted,
          DateCreatedUtc = entity.DateCreatedUtc,
          DateModifiedUtc = entity.DateModifiedUtc,
          TaskId = entity.TaskId,
          TaskName = entity.TaskName
        };
      }
    }

    public static DailyTaskDto FromEntity(DailyTaskEntity entity)
    {
      // TODO: [TESTS] (DailyTaskDto.FromEntity) Add tests
      return entity == null ? null : DailyTaskDto.Projection.Compile()(entity);
    }

    public DailyTaskEntity AsEntity(int userIdOverride = 0)
    {
      // TODO: [TESTS] (DailyTaskDto.AsEntity) Add tests
      return new DailyTaskEntity
      {
        UserId = userIdOverride > 0 ? userIdOverride : UserId,
        ClientId = ClientId,
        Deleted = Deleted,
        DateCreatedUtc = DateCreatedUtc,
        DateModifiedUtc = DateModifiedUtc,
        TaskId = TaskId,
        TaskName = TaskName
      };
    }
  }

  public class DailyTaskDtoValidator : AbstractValidator<DailyTaskDto>
  {
    public DailyTaskDtoValidator()
    {
      RuleSet("Add", () =>
      {
        RuleFor(x => x.UserId).GreaterThan(0);
        RuleFor(x => x.ClientId).GreaterThan(0);
        RuleFor(x => x.TaskName).NotNull().MinimumLength(3).MaximumLength(128);
      });
    }

    public static ValidationResult Add(DailyTaskDto taskDto)
    {
      return new DailyTaskDtoValidator().Validate(taskDto,
        options => options.IncludeRuleSets("Add")
      );
    }
  }
}
