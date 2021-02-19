using System;
using System.Linq.Expressions;
using TimeTracker.Core.Database.Entities;

namespace TimeTracker.Core.Models.Dto
{
  public class ToDoDto
  {
    public long ToDoId { get; set; }
    public int CategoryId { get; set; }
    public int SubCategoryId { get; set; }
    public int UserId { get; set; }
    public bool Deleted { get; set; }
    public DateTime DateAddedUtc { get; set; }
    public DateTime? DateUpdatedUtc { get; set; }
    public DateTime? DateDeletedUtc { get; set; }
    public string ToDo { get; set; }

    public ToDoDto()
    {
      // TODO: [TESTS] (ToDoDto) Add tests
      ToDoId = 0;
      CategoryId = 0;
      SubCategoryId = 0;
      UserId = 0;
      Deleted = false;
      DateAddedUtc = DateTime.UtcNow;
      DateUpdatedUtc = null;
      DateDeletedUtc = null;
      ToDo = string.Empty;
    }

    public static Expression<Func<ToDoEntity, ToDoDto>> Projection
    {
      get
      {
        return entity => new ToDoDto
        {
          DateUpdatedUtc = entity.DateUpdatedUtc,
          Deleted = entity.Deleted,
          UserId = entity.UserId,
          SubCategoryId = entity.SubCategoryId,
          DateAddedUtc = entity.DateAddedUtc,
          ToDo = entity.ToDo,
          ToDoId = entity.ToDoId,
          CategoryId = entity.SubCategoryId,
          DateDeletedUtc = entity.DateDeletedUtc
        };
      }
    }
  }
}
