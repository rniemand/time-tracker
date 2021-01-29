using System;

namespace TimeTracker.Core.Database.Entities
{
  public class ToDoEntity
  {
    public long ToDoId { get; set; }
    public int CategoryId { get; set; }
    public int SubCategoryId { get; set; }
    public int UserId { get; set; }
    public bool Deleted { get; set; }
    public DateTime DateAddedUtc { get; set; }
    public DateTime? DateModifiedUtc { get; set; }
    public string ToDo { get; set; }

    public ToDoEntity()
    {
      // TODO: [TESTS] (ToDoEntity) Add tests
      ToDoId = 0;
      CategoryId = 0;
      SubCategoryId = 0;
      UserId = 0;
      Deleted = false;
      DateAddedUtc = DateTime.UtcNow;
      DateModifiedUtc = null;
      ToDo = string.Empty;
    }
	}
}
