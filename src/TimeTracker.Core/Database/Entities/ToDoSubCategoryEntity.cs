using System;

namespace TimeTracker.Core.Database.Entities
{
  public class ToDoSubCategoryEntity
  {
    public int SubCategoryId { get; set; }
    public int CategoryId { get; set; }
    public int UserId { get; set; }
    public bool Deleted { get; set; }
    public DateTime DateAddedUtc { get; set; }
    public DateTime? DateUpdatedUtc { get; set; }
    public DateTime? DateDeletedUtc { get; set; }
    public string SubCategory { get; set; }

    public ToDoSubCategoryEntity()
    {
      // TODO: [TESTS] (ToDoSubCategoryEntity) Add tests
      SubCategoryId = 0;
      CategoryId = 0;
      UserId = 0;
      Deleted = false;
      DateAddedUtc = DateTime.UtcNow;
      DateUpdatedUtc = null;
      DateDeletedUtc = null;
      SubCategory = string.Empty;
    }
  }
}
