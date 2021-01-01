using System;
using System.Linq.Expressions;
using TimeTracker.Core.Database.Entities;
using TimeTracker.Core.Enums;

namespace TimeTracker.Core.Models.Dto
{
  public class RawTrackedTimeDto
  {
    public long EntryId { get; set; }
    public long ParentEntryId { get; set; }
    public long RootParentEntryId { get; set; }
    public int ClientId { get; set; }
    public int ProductId { get; set; }
    public int ProjectId { get; set; }
    public int UserId { get; set; }
    public bool Deleted { get; set; }
    public EntryRunningState EntryState { get; set; }
    public int EntryRunningTimeSec { get; set; }
    public DateTime EntryStartTimeUtc { get; set; }
    public DateTime? EntryEndTimeUtc { get; set; }

    public static Expression<Func<RawTrackedTimeEntity, RawTrackedTimeDto>> Projection
    {
      get
      {
        return entity => new RawTrackedTimeDto
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
          EntryId = entity.EntryId,
          ParentEntryId = entity.ParentEntryId,
          RootParentEntryId = entity.RootParentEntryId
        };
      }
    }

    public static RawTrackedTimeDto FromEntity(RawTrackedTimeEntity entity)
    {
      // TODO: [TESTS] (RawTrackedTimeDto.FromEntity) Add tests
      return entity == null ? null : Projection.Compile()(entity);
    }

    public RawTrackedTimeDto()
    {
      // TODO: [TESTS] (RawTrackedTimeDto) Add tests
      EntryId = 0;
      ParentEntryId = 0;
      RootParentEntryId = 0;
      ClientId = 0;
      ProductId = 0;
      ProjectId = 0;
      UserId = 0;
      Deleted = false;
      EntryState = EntryRunningState.Running;
      EntryRunningTimeSec = 0;
      EntryStartTimeUtc = DateTime.UtcNow;
      EntryEndTimeUtc = null;
    }

    public RawTrackedTimeEntity AsEntity()
    {
      // TODO: [TESTS] (RawTrackedTimeDto.AsEntity) Add tests
      return new RawTrackedTimeEntity
      {
        UserId = UserId,
        ClientId = ClientId,
        Deleted = Deleted,
        ProductId = ProductId,
        ProjectId = ProjectId,
        EntryEndTimeUtc = EntryEndTimeUtc,
        EntryState = EntryState,
        EntryRunningTimeSec = EntryRunningTimeSec,
        EntryStartTimeUtc = EntryStartTimeUtc,
        EntryId = EntryId,
        ParentEntryId = ParentEntryId,
        RootParentEntryId = RootParentEntryId
      };
    }
  }
}
