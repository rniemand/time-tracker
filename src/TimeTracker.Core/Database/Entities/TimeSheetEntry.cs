using System;

namespace TimeTracker.Core.Database.Entities
{
  public class TimeSheetEntry
  {
    public long EntryId { get; set; }
    public int UserId { get; set; }
    public int ClientId { get; set; }
    public int ProductId { get; set; }
    public int ProjectId { get; set; }
    public bool Deleted { get; set; }
    public DateTime DateAddedUtc { get; set; }
    public DateTime? DateUpdatedUtc { get; set; }
    public DateTime? DateDeletedUtc { get; set; }
    public DateTime EntryDate { get; set; }
    public int EntryVersion { get; set; }
    public int EntryTimeMin { get; set; }

    public TimeSheetEntry()
    {
      // TODO: [TESTS] (TimeSheetEntry) Add tests
      EntryId = 0;
      UserId = 0;
      ClientId = 0;
      ProductId = 0;
      ProjectId = 0;
      Deleted = false;
      DateAddedUtc = DateTime.UtcNow;
      DateUpdatedUtc = null;
      DateDeletedUtc = null;
      EntryDate = DateTime.Now;
      EntryVersion = 1;
      EntryTimeMin = 0;
    }
  }
}
