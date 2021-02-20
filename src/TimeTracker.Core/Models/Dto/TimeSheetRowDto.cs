using System;
using System.Linq.Expressions;
using TimeTracker.Core.Database.Entities;

namespace TimeTracker.Core.Models.Dto
{
  public class TimeSheetRowDto
  {
    public long RowId { get; set; }
    public int DateId { get; set; }
    public int UserId { get; set; }
    public int ClientId { get; set; }
    public int ProductId { get; set; }
    public int ProjectId { get; set; }

    public TimeSheetRowDto()
    {
      // TODO: [TESTS] (TimeSheetRowDto) Add tests
      RowId = 0;
      DateId = 0;
      UserId = 0;
      ClientId = 0;
      ProductId = 0;
      ProjectId = 0;
    }

    public static Expression<Func<TimeSheetRow, TimeSheetRowDto>> Projection
    {
      get
      {
        return entity => new TimeSheetRowDto
        {
          UserId = entity.UserId,
          ClientId = entity.ClientId,
          ProductId = entity.ProductId,
          ProjectId = entity.ProjectId,
          DateId = entity.DateId,
          RowId = entity.RowId
        };
      }
    }

    public static TimeSheetRowDto FromEntity(TimeSheetRow entity)
    {
      // TODO: [TESTS] (TimeSheetRowDto.FromEntity) Add tests
      return entity == null ? null : Projection.Compile()(entity);
    }
  }
}
