using System;
using TimeTracker.Core.Enums;

namespace TimeTracker.Core.Database.Entities
{
  public class TrackedTimeEntity
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

    public TrackedTimeEntity()
    {
      // TODO: [TESTS] (TrackedTimeEntity) Add tests
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
  }
}
