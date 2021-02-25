using System;

namespace TimeTracker.Core.Database.Entities
{
  public class ToDoCategoryEntity
  {
    public int CategoryId { get; set; }
    public int UserId { get; set; }
    public bool Deleted { get; set; }
    public DateTime DateAddedUtc { get; set; }
    public DateTime? DateUpdatedUtc { get; set; }
    public DateTime? DateDeletedUtc { get; set; }
    public string Category { get; set; }

    public ToDoCategoryEntity()
    {
      // TODO: [TESTS] (ToDoCategoryEntity) Add tests
      CategoryId = 0;
      UserId = 0;
      Deleted = false;
      DateAddedUtc = DateTime.UtcNow;
      DateUpdatedUtc = null;
      DateDeletedUtc = null;
      Category = string.Empty;
    }
  }
}
