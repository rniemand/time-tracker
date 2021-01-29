using System;
using System.Linq.Expressions;
using TimeTracker.Core.Database.Entities;

namespace TimeTracker.Core.Models.Dto
{
  public class ToDoCategoryDto
  {
    public int CategoryId { get; set; }
    public int UserId { get; set; }
    public bool Deleted { get; set; }
    public DateTime DateCreatedUtc { get; set; }
    public DateTime? DateModifiedUtc { get; set; }
    public string Category { get; set; }

    public ToDoCategoryDto()
    {
      // TODO: [TESTS] (ToDoCategoryDto) Add tests
      CategoryId = 0;
      UserId = 0;
      Deleted = false;
      DateCreatedUtc = DateTime.UtcNow;
      DateModifiedUtc = null;
      Category = string.Empty;
    }

    public static Expression<Func<ToDoCategoryEntity, ToDoCategoryDto>> Projection
    {
      get
      {
        return entity => new ToDoCategoryDto
        {
          CategoryId = entity.CategoryId,
          DateModifiedUtc = entity.DateModifiedUtc,
          Deleted = entity.Deleted,
          DateCreatedUtc = entity.DateCreatedUtc,
          Category = entity.Category,
          UserId = entity.UserId
        };
      }
    }
  }
}
