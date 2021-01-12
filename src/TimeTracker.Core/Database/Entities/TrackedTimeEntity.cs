using System;
using TimeTracker.Core.Enums;

namespace TimeTracker.Core.Database.Entities
{
  public class TrackedTimeEntity
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

    public TrackedTimeEntity()
    {
      // TODO: [TESTS] (TrackedTimeEntity) Add tests
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
  }
}
