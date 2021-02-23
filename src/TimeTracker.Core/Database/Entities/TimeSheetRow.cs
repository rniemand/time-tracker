using System;

namespace TimeTracker.Core.Database.Entities
{
  public class TimeSheetRow
  {
    public long RowId { get; set; }
    public int DateId { get; set; }
    public int UserId { get; set; }
    public int ClientId { get; set; }
    public int ProductId { get; set; }
    public int ProjectId { get; set; }
    public bool Deleted { get; set; }
    public DateTime DateAddedUtc { get; set; }
    public DateTime? DateUpdatedUtc { get; set; }
    public DateTime? DateDeletedUtc { get; set; }
    public DateTime EntryDate { get; set; }

    public TimeSheetRow()
    {
      // TODO: [TESTS] (TimeSheetRow) Add tests
      RowId = 0;
      DateId = 0;
      UserId = 0;
      ClientId = 0;
      ProductId = 0;
      ProjectId = 0;
      Deleted = false;
      DateAddedUtc = DateTime.UtcNow;
      DateUpdatedUtc = null;
      DateDeletedUtc = null;
      EntryDate = DateTime.Now;
    }
  }
}
