using System;
using TimeTracker.Core.Enums;

namespace TimeTracker.Core.Database.Entities
{
  public class RawTimerEntity
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

    // Optional Properties
    public string ProductName { get; set; }
    public string ProjectName { get; set; }
    public string ClientName { get; set; }

    public RawTimerEntity()
    {
      // TODO: [TESTS] (RawTimerEntity) Add tests
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
      EntryEndTimeUtc = null;
      ProductName = string.Empty;
      ProjectName = string.Empty;
      ClientName = string.Empty;
    }
  }
}
