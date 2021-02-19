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
    public DateTime DateAddedUtc { get; set; }
    public DateTime? DateUpdatedUtc { get; set; }
    public DateTime? DateDeletedUtc { get; set; }
    public string TaskName { get; set; }

    public DailyTaskDto()
    {
      // TODO: [TESTS] (DailyTaskDto) Add tests
      TaskId = 0;
      UserId = 0;
      ClientId = 0;
      Deleted = false;
      DateAddedUtc = DateTime.UtcNow;
      DateUpdatedUtc = null;
      DateDeletedUtc = null;
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
          DateAddedUtc = entity.DateAddedUtc,
          DateUpdatedUtc = entity.DateUpdatedUtc,
          TaskId = entity.TaskId,
          TaskName = entity.TaskName,
          DateDeletedUtc = entity.DateDeletedUtc
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
        DateAddedUtc = DateAddedUtc,
        DateUpdatedUtc = DateUpdatedUtc,
        TaskId = TaskId,
        TaskName = TaskName,
        DateDeletedUtc = DateDeletedUtc
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

      RuleSet("Update", () =>
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

    public static ValidationResult Update(DailyTaskDto taskDto)
    {
      return new DailyTaskDtoValidator().Validate(taskDto,
        options => options.IncludeRuleSets("Update")
      );
    }
  }
}
