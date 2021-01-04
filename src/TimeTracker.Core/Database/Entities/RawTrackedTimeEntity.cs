using System;
using TimeTracker.Core.Enums;

namespace TimeTracker.Core.Database.Entities
{
  public class RawTrackedTimeEntity
  {
    public long EntryId { get; set; }
    public long ParentEntryId { get; set; }
    public long RootParentEntryId { get; set; }
    public int ClientId { get; set; }
    public int ProductId { get; set; }
    public int ProjectId { get; set; }
    public int UserId { get; set; }
    public bool Deleted { get; set; }
    public bool Running { get; set; }
    public EntryRunningState EntryState { get; set; }
    public int EntryRunningTimeSec { get; set; }
    public DateTime EntryStartTimeUtc { get; set; }
    public DateTime? EntryEndTimeUtc { get; set; }

    // Optional Properties
    public string ProductName { get; set; }
    public string ProjectName { get; set; }
    public string ClientName { get; set; }

    public RawTrackedTimeEntity()
    {
      // TODO: [TESTS] (RawTrackedTimeEntity) Add tests
      EntryId = 0;
      ParentEntryId = 0;
      RootParentEntryId = 0;
      ClientId = 0;
      ProductId = 0;
      ProjectId = 0;
      UserId = 0;
      Deleted = false;
      Running = true;
      EntryState = EntryRunningState.Running;
      EntryRunningTimeSec = 0;
      EntryStartTimeUtc = DateTime.UtcNow;
      EntryEndTimeUtc = null;
      ProductName = string.Empty;
      ProjectName = string.Empty;
      ClientName = string.Empty;
    }
  }
}
