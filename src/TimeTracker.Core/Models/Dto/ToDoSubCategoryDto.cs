using System;
using System.Linq.Expressions;
using TimeTracker.Core.Database.Entities;

namespace TimeTracker.Core.Models.Dto
{
  public class ToDoSubCategoryDto
  {
    public int SubCategoryId { get; set; }
    public int CategoryId { get; set; }
    public int UserId { get; set; }
    public bool Deleted { get; set; }
    public DateTime DateAddedUtc { get; set; }
    public DateTime? DateUpdatedUtc { get; set; }
    public DateTime? DateDeletedUtc { get; set; }
    public string SubCategory { get; set; }

    public ToDoSubCategoryDto()
    {
      // TODO: [TESTS] (ToDoSubCategoryDto) Add tests
      SubCategoryId = 0;
      CategoryId = 0;
      UserId = 0;
      Deleted = false;
      DateAddedUtc = DateTime.UtcNow;
      DateUpdatedUtc = null;
      DateDeletedUtc = null;
      SubCategory = string.Empty;
    }

    public static Expression<Func<ToDoSubCategoryEntity, ToDoSubCategoryDto>> Projection
    {
      get
      {
        return entity => new ToDoSubCategoryDto
        {
          CategoryId = entity.CategoryId,
          Deleted = entity.Deleted,
          DateAddedUtc = entity.DateAddedUtc,
          DateUpdatedUtc = entity.DateUpdatedUtc,
          SubCategoryId = entity.SubCategoryId,
          SubCategory = entity.SubCategory,
          UserId = entity.UserId,
          DateDeletedUtc = entity.DateDeletedUtc
        };
      }
    }
  }
}
