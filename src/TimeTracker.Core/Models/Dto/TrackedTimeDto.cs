using System;
using System.Linq.Expressions;
using TimeTracker.Core.Database.Entities;
using TimeTracker.Core.Enums;

namespace TimeTracker.Core.Models.Dto
{
  public class TrackedTimeDto
  {
    public long TrackedTimeId { get; set; }
    public int ClientId { get; set; }
    public int ProductId { get; set; }
    public int ProjectId { get; set; }
    public int UserId { get; set; }
    public bool Deleted { get; set; }
    public bool EntryRunning { get; set; }
    public EntryEndReason EntryEndType { get; set; }
    public int EntryRunningTimeSec { get; set; }
    public DateTime EntryStartTimeUtc { get; set; }
    public DateTime? EntryEndTimeUtc { get; set; }

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
          EntryEndTimeUtc = entity.EntryEndTimeUtc,
          EntryEndType = entity.EntryEndType,
          EntryRunning = entity.EntryRunning,
          EntryRunningTimeSec = entity.EntryRunningTimeSec,
          EntryStartTimeUtc = entity.EntryStartTimeUtc,
          TrackedTimeId = entity.TrackedTimeId
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
      TrackedTimeId = 0;
      ClientId = 0;
      ProductId = 0;
      ProjectId = 0;
      UserId = 0;
      Deleted = false;
      EntryRunning = true;
      EntryEndType = EntryEndReason.Unspecified;
      EntryRunningTimeSec = 0;
      EntryStartTimeUtc = DateTime.UtcNow;
      EntryEndTimeUtc = null;
    }

    public TrackedTimeEntity AsTrackedTimeEntity()
    {
      // TODO: [TESTS] (TrackedTimeDto.AsTrackedTimeEntity) Add tests
      return new TrackedTimeEntity
      {
        UserId = UserId,
        ClientId = ClientId,
        Deleted = Deleted,
        ProductId = ProductId,
        ProjectId = ProjectId,
        EntryEndTimeUtc = EntryEndTimeUtc,
        EntryEndType = EntryEndType,
        EntryRunning = EntryRunning,
        EntryRunningTimeSec = EntryRunningTimeSec,
        EntryStartTimeUtc = EntryStartTimeUtc,
        TrackedTimeId = TrackedTimeId
      };
    }
  }
}
