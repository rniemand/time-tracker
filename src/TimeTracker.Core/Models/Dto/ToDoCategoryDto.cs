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
    public DateTime DateAddedUtc { get; set; }
    public DateTime? DateUpdatedUtc { get; set; }
    public DateTime? DateDeletedUtc { get; set; }
    public string Category { get; set; }

    public ToDoCategoryDto()
    {
      // TODO: [TESTS] (ToDoCategoryDto) Add tests
      CategoryId = 0;
      UserId = 0;
      Deleted = false;
      DateAddedUtc = DateTime.UtcNow;
      DateUpdatedUtc = null;
      DateDeletedUtc = null;
      Category = string.Empty;
    }

    public static Expression<Func<ToDoCategoryEntity, ToDoCategoryDto>> Projection
    {
      get
      {
        return entity => new ToDoCategoryDto
        {
          CategoryId = entity.CategoryId,
          DateUpdatedUtc = entity.DateUpdatedUtc,
          Deleted = entity.Deleted,
          DateAddedUtc = entity.DateAddedUtc,
          Category = entity.Category,
          UserId = entity.UserId,
          DateDeletedUtc = entity.DateDeletedUtc
        };
      }
    }
  }
}
