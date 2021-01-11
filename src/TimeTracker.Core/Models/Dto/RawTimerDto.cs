using System;
using System.Linq.Expressions;
using FluentValidation;
using FluentValidation.Results;
using TimeTracker.Core.Database.Entities;
using TimeTracker.Core.Enums;

namespace TimeTracker.Core.Models.Dto
{
  public class RawTimerDto
  {
    public long RawTimerId { get; set; }
    public long ParentTimerId { get; set; }
    public long RootTimerId { get; set; }
    public int ClientId { get; set; }
    public int ProductId { get; set; }
    public int ProjectId { get; set; }
    public int UserId { get; set; }
    public bool Deleted { get; set; }
    public bool Running { get; set; }
    public bool Completed { get; set; }
    public bool Processed { get; set; }
    public EntryRunningState EntryState { get; set; }
    public int EntryRunningTimeSec { get; set; }
    public DateTime EntryStartTimeUtc { get; set; }
    public DateTime? EntryEndTimeUtc { get; set; }
    public string TimerNotes { get; set; }

    // Optional Properties
    public string ProductName { get; set; }
    public string ProjectName { get; set; }
    public string ClientName { get; set; }

    public static Expression<Func<RawTimerEntity, RawTimerDto>> Projection
    {
      get
      {
        return entity => new RawTimerDto
        {
          UserId = entity.UserId,
          ClientId = entity.ClientId,
          Deleted = entity.Deleted,
          ProductId = entity.ProductId,
          ProjectId = entity.ProjectId,
          EntryEndTimeUtc = entity.EntryEndTimeUtc,
          EntryState = entity.EntryState,
          EntryRunningTimeSec = entity.EntryRunningTimeSec,
          EntryStartTimeUtc = entity.EntryStartTimeUtc,
          RawTimerId = entity.RawTimerId,
          ParentTimerId = entity.ParentTimerId,
          RootTimerId = entity.RootTimerId,
          Running = entity.Running,
          ProductName = entity.ProductName,
          ProjectName = entity.ProjectName,
          ClientName = entity.ClientName,
          Completed = entity.Completed,
          Processed = entity.Processed,
          TimerNotes = entity.TimerNotes
        };
      }
    }

    public static RawTimerDto FromEntity(RawTimerEntity entity)
    {
      // TODO: [TESTS] (RawTimerDto.FromEntity) Add tests
      return entity == null ? null : Projection.Compile()(entity);
    }

    public RawTimerDto()
    {
      // TODO: [TESTS] (RawTimerDto) Add tests
      RawTimerId = 0;
      ParentTimerId = 0;
      RootTimerId = 0;
      ClientId = 0;
      ProductId = 0;
      ProjectId = 0;
      UserId = 0;
      Deleted = false;
      Running = true;
      Completed = false;
      Processed = false;
      EntryState = EntryRunningState.Running;
      EntryRunningTimeSec = 0;
      EntryStartTimeUtc = DateTime.UtcNow;
      TimerNotes = string.Empty;
      EntryEndTimeUtc = null;
      ProductName = string.Empty;
      ProjectName = string.Empty;
      ClientName = string.Empty;
    }

    public RawTimerEntity AsEntity(int userIdOverride = 0)
    {
      // TODO: [TESTS] (RawTimerDto.AsEntity) Add tests
      return new RawTimerEntity
      {
        UserId = userIdOverride > 0 ? userIdOverride : UserId,
        ClientId = ClientId,
        Deleted = Deleted,
        ProductId = ProductId,
        ProjectId = ProjectId,
        EntryEndTimeUtc = EntryEndTimeUtc,
        EntryState = EntryState,
        EntryRunningTimeSec = EntryRunningTimeSec,
        EntryStartTimeUtc = EntryStartTimeUtc,
        RawTimerId = RawTimerId,
        ParentTimerId = ParentTimerId,
        RootTimerId = RootTimerId,
        Running = Running,
        ProductName = ProductName,
        ProjectName = ProjectName,
        ClientName = ClientName,
        Completed = Completed,
        Processed = Processed,
        TimerNotes = TimerNotes
      };
    }
  }

  public class RawTimerDtoValidator : AbstractValidator<RawTimerDto>
  {
    public RawTimerDtoValidator()
    {
      RuleSet("StartNew", () =>
      {
        RuleFor(x => x.ParentTimerId).GreaterThanOrEqualTo(0);
        RuleFor(x => x.ClientId).GreaterThan(0);
        RuleFor(x => x.ProductId).GreaterThan(0);
        RuleFor(x => x.ProjectId).GreaterThan(0);
        RuleFor(x => x.UserId).GreaterThan(0);
        RuleFor(x => x.EntryState).IsInEnum();
        RuleFor(x => x.Running).Equal(true);
      });

      RuleSet("UpdateTimerDuration", () =>
      {
        RuleFor(x => x.EntryStartTimeUtc).NotNull();
        RuleFor(x => x.EntryRunningTimeSec).GreaterThan(0);
        RuleFor(x => x.TimerNotes).NotNull().MinimumLength(3);
        RuleFor(x => x.RawTimerId).GreaterThan(0);
      });
    }

    public static ValidationResult StartNew(RawTimerDto timer)
    {
      return new RawTimerDtoValidator().Validate(timer,
        options => options.IncludeRuleSets("StartNew")
      );
    }

    public static ValidationResult UpdateTimerDuration(RawTimerDto timer)
    {
      return new RawTimerDtoValidator().Validate(timer,
        options => options.IncludeRuleSets("UpdateTimerDuration")
      );
    }
  }
}
