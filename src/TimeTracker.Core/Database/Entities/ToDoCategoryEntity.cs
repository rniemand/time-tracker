using System;

namespace TimeTracker.Core.Database.Entities
{
  public class ToDoCategoryEntity
  {
    public int CategoryId { get; set; }
    public int UserId { get; set; }
    public bool Deleted { get; set; }
    public DateTime DateCreatedUtc { get; set; }
    public DateTime? DateModifiedUtc { get; set; }
    public string Category { get; set; }

    public ToDoCategoryEntity()
    {
      // TODO: [TESTS] (ToDoCategoryEntity) Add tests
      CategoryId = 0;
      UserId = 0;
      Deleted = false;
      DateCreatedUtc = DateTime.UtcNow;
      DateModifiedUtc = null;
      Category = string.Empty;
    }
  }
}
