using System;
using System.Linq.Expressions;
using FluentValidation;
using FluentValidation.Results;
using TimeTracker.Core.Database.Entities;
using TimeTracker.Core.Enums;

namespace TimeTracker.Core.Models.Dto
{
  public class TrackedTimeDto
  {
    public long EntryId { get; set; }
    public int ClientId { get; set; }
    public int ProductId { get; set; }
    public int ProjectId { get; set; }
    public int UserId { get; set; }
    public bool Deleted { get; set; }
    public bool Running { get; set; }
    public TimerEndReason EndReason { get; set; }
    public int TotalSeconds { get; set; }
    public DateTime StartTimeUtc { get; set; }
    public DateTime? EndTimeUtc { get; set; }
    public string Notes { get; set; }

    // Optional Properties
    public string ProductName { get; set; }
    public string ProjectName { get; set; }
    public string ClientName { get; set; }

    public static Expression<Func<TrackedTimeEntity, TrackedTimeDto>> Projection
    {
      get
      {
        return entity => new TrackedTimeDto
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
          EndReason = entity.EndReason
        };
      }
    }

    public static TrackedTimeDto FromEntity(TrackedTimeEntity entity)
    {
      // TODO: [TESTS] (TrackedTimeDto.FromEntity) Add tests
      return entity == null ? null : Projection.Compile()(entity);
    }

    public TrackedTimeDto()
    {
      // TODO: [TESTS] (TrackedTimeDto) Add tests
      EntryId = 0;
      ClientId = 0;
      ProductId = 0;
      ProjectId = 0;
      UserId = 0;
      Deleted = false;
      Running = true;
      EndReason = TimerEndReason.Unknown;
      TotalSeconds = 0;
      StartTimeUtc = DateTime.UtcNow;
      Notes = string.Empty;
      EndTimeUtc = null;
      ProductName = string.Empty;
      ProjectName = string.Empty;
      ClientName = string.Empty;
    }

    public TrackedTimeEntity AsEntity(int userIdOverride = 0)
    {
      // TODO: [TESTS] (TrackedTimeDto.AsEntity) Add tests
      return new TrackedTimeEntity
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
        EndReason = EndReason
      };
    }
  }

  public class TrackedTimeDtoValidator : AbstractValidator<TrackedTimeDto>
  {
    public TrackedTimeDtoValidator()
    {
      RuleSet("StartNew", () =>
      {
        RuleFor(x => x.ClientId).GreaterThan(0);
        RuleFor(x => x.ProductId).GreaterThan(0);
        RuleFor(x => x.ProjectId).GreaterThan(0);
        RuleFor(x => x.UserId).GreaterThan(0);
        RuleFor(x => x.Running).Equal(true);
      });

      RuleSet("UpdateDuration", () =>
      {
        RuleFor(x => x.StartTimeUtc).NotNull();
        RuleFor(x => x.TotalSeconds).GreaterThan(0);
        RuleFor(x => x.Notes).NotNull().MinimumLength(3);
        RuleFor(x => x.EntryId).GreaterThan(0);
      });
    }

    public static ValidationResult StartNew(TrackedTimeDto timer)
    {
      return new TrackedTimeDtoValidator().Validate(timer,
        options => options.IncludeRuleSets("StartNew")
      );
    }

    public static ValidationResult UpdateDuration(TrackedTimeDto timer)
    {
      return new TrackedTimeDtoValidator().Validate(timer,
        options => options.IncludeRuleSets("UpdateDuration")
      );
    }
  }
}
