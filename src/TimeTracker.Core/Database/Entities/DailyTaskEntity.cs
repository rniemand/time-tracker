using System;

namespace TimeTracker.Core.Database.Entities
{
  public class DailyTaskEntity
  {
    public int TaskId { get; set; }
    public int UserId { get; set; }
    public int ClientId { get; set; }
    public bool Deleted { get; set; }
    public DateTime DateCreatedUtc { get; set; }
    public DateTime? DateModifiedUtc { get; set; }
    public string TaskName { get; set; }

    public DailyTaskEntity()
    {
      // TODO: [TESTS] (DailyTaskEntity) Add tests
      TaskId = 0;
      UserId = 0;
      ClientId = 0;
      Deleted = false;
      DateCreatedUtc = DateTime.UtcNow;
      DateModifiedUtc = null;
      TaskName = string.Empty;
    }
  }
}
