using System;
using System.Diagnostics;

namespace TimeTracker.Core.Database.Entities
{
  [DebuggerDisplay("{ClientId}: {EntryDate}")]
  public class TimeSheetDate
  {
    public int DateId { get; set; }
    public int UserId { get; set; }
    public int ClientId { get; set; }
    public bool Deleted { get; set; }
    public DateTime DateAddedUtc { get; set; }
    public DateTime? DateUpdatedUtc { get; set; }
    public DateTime? DateDeletedUtc { get; set; }
    public DayOfWeek DayOfWeek { get; set; }
    public DateTime EntryDate { get; set; }

    public TimeSheetDate()
    {
      // TODO: [TESTS] (TimeSheetDate) Add tests
      DateId = 0;
      UserId = 0;
      ClientId = 0;
      Deleted = false;
      DateAddedUtc = DateTime.UtcNow;
      DateUpdatedUtc = null;
      DateDeletedUtc = null;
      DayOfWeek = DayOfWeek.Sunday;
      EntryDate = DateTime.Now;
    }
  }
}
