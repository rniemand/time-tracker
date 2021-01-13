using System;
using System.Linq.Expressions;
using FluentValidation;
using FluentValidation.Results;
using TimeTracker.Core.Database.Entities;
using TimeTracker.Core.Enums;

namespace TimeTracker.Core.Models.Dto
{
  public class TimerDto
  {
    public long EntryId { get; set; }
    public int ClientId { get; set; }
    public int ProductId { get; set; }
    public int ProjectId { get; set; }
    public int TaskId { get; set; }
    public int UserId { get; set; }
    public bool Deleted { get; set; }
    public bool Running { get; set; }
    public TimerState EntryState { get; set; }
    public TimerType EntryType { get; set; }
    public int TotalSeconds { get; set; }
    public DateTime StartTimeUtc { get; set; }
    public DateTime? EndTimeUtc { get; set; }
    public string Notes { get; set; }

    // Optional Properties
    public string ProductName { get; set; }
    public string ProjectName { get; set; }
    public string ClientName { get; set; }

    // Constructor
    public TimerDto()
    {
      // TODO: [TESTS] (TimerDto) Add tests
      EntryId = 0;
      ClientId = 0;
      ProductId = 0;
      ProjectId = 0;
      TaskId = 0;
      UserId = 0;
      Deleted = false;
      Running = true;
      EntryState = TimerState.Unknown;
      EntryType = TimerType.Unspecified;
      TotalSeconds = 0;
      StartTimeUtc = DateTime.UtcNow;
      Notes = string.Empty;
      EndTimeUtc = null;
      ProductName = string.Empty;
      ProjectName = string.Empty;
      ClientName = string.Empty;
    }


    // Helpers
    public static Expression<Func<TimerEntity, TimerDto>> Projection
    {
      get
      {
        return entity => new TimerDto
        {
          UserId = entity.UserId,
          ClientId = entity.ClientId,
          Deleted = entity.Deleted,
          ProductId = entity.ProductId,
          ProjectId = entity.ProjectId,
          EndTimeUtc = entity.EndTimeUtc,
          TotalSeconds = entity.TotalSeconds,
          StartTimeUtc = entity.StartTimeUtc,
          EntryId = entity.EntryId,
          Running = entity.Running,
          ProductName = entity.ProductName,
          ProjectName = entity.ProjectName,
          ClientName = entity.ClientName,
          Notes = entity.Notes,
          EntryState = entity.EntryState,
          EntryType = entity.EntryType,
          TaskId = entity.TaskId
        };
      }
    }

    public static TimerDto FromEntity(TimerEntity entity)
    {
      // TODO: [TESTS] (TimerDto.FromEntity) Add tests
      return entity == null ? null : Projection.Compile()(entity);
    }

    public TimerEntity AsEntity(int userIdOverride = 0)
    {
      // TODO: [TESTS] (TimerDto.AsEntity) Add tests
      return new TimerEntity
      {
        UserId = userIdOverride > 0 ? userIdOverride : UserId,
        ClientId = ClientId,
        Deleted = Deleted,
        ProductId = ProductId,
        ProjectId = ProjectId,
        EndTimeUtc = EndTimeUtc,
        TotalSeconds = TotalSeconds,
        StartTimeUtc = StartTimeUtc,
        EntryId = EntryId,
        Running = Running,
        ProductName = ProductName,
        ProjectName = ProjectName,
        ClientName = ClientName,
        Notes = Notes,
        EntryState = EntryState,
        EntryType = EntryType,
        TaskId = TaskId
      };
    }
  }

  public class TrackedTimeDtoValidator : AbstractValidator<TimerDto>
  {
    public TrackedTimeDtoValidator()
    {
      RuleSet("StartNew", () =>
      {
        // ProjectWork validation
        RuleFor(x => x.ClientId)
          .GreaterThan(0)
          .When(x => x.EntryType == TimerType.ProjectWork);

        RuleFor(x => x.ProductId)
          .GreaterThan(0)
          .When(x => x.EntryType == TimerType.ProjectWork);

        RuleFor(x => x.ProjectId)
          .GreaterThan(0)
          .When(x => x.EntryType == TimerType.ProjectWork);

        // DailyTask validation
        RuleFor(x => x.TaskId)
          .GreaterThan(0)
          .When(x => x.EntryType == TimerType.DailyTask);

        // General validation
        RuleFor(x => x.UserId).GreaterThan(0);
        RuleFor(x => x.Running).Equal(true);
        RuleFor(x => x.EntryType).NotEqual(TimerType.Unspecified);
      });

      RuleSet("UpdateDuration", () =>
      {
        RuleFor(x => x.StartTimeUtc).NotNull();
        RuleFor(x => x.TotalSeconds).GreaterThan(0);
        RuleFor(x => x.Notes).NotNull().MinimumLength(3);
        RuleFor(x => x.EntryId).GreaterThan(0);
      });
    }

    public static ValidationResult StartNew(TimerDto timer)
    {
      return new TrackedTimeDtoValidator().Validate(timer,
        options => options.IncludeRuleSets("StartNew")
      );
    }

    public static ValidationResult UpdateDuration(TimerDto timer)
    {
      return new TrackedTimeDtoValidator().Validate(timer,
        options => options.IncludeRuleSets("UpdateDuration")
      );
    }
  }
}
