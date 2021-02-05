using System;

namespace TimeTracker.Core.Database.Entities
{
  public class ToDoSubCategoryEntity
  {
    public int SubCategoryId { get; set; }
    public int CategoryId { get; set; }
    public int UserId { get; set; }
    public bool Deleted { get; set; }
    public DateTime DateCreatedUtc { get; set; }
    public DateTime? DateModifiedUtc { get; set; }
    public string SubCategory { get; set; }

    public ToDoSubCategoryEntity()
    {
      // TODO: [TESTS] (ToDoSubCategoryEntity) Add tests
      SubCategoryId = 0;
      CategoryId = 0;
      UserId = 0;
      Deleted = false;
      DateCreatedUtc = DateTime.UtcNow;
      DateModifiedUtc = null;
      SubCategory = string.Empty;
    }
  }
}
