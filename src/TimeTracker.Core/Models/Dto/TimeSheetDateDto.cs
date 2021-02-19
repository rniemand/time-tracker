using System;
using System.Linq.Expressions;
using TimeTracker.Core.Database.Entities;

namespace TimeTracker.Core.Models.Dto
{
  public class TimeSheetDateDto
  {
    public int DateId { get; set; }
    public int UserId { get; set; }
    public int ClientId { get; set; }
    public DayOfWeek DayOfWeek { get; set; }
    public DateTime EntryDate { get; set; }

    public TimeSheetDateDto()
    {
      // TODO: [TESTS] (TimeSheetDateDto) Add tests
      DateId = 0;
      UserId = 0;
      ClientId = 0;
      DayOfWeek = DayOfWeek.Sunday;
      EntryDate = DateTime.Now;
    }

    public static Expression<Func<TimeSheetDate, TimeSheetDateDto>> Projection
    {
      get
      {
        return entity => new TimeSheetDateDto
        {
          UserId = entity.UserId,
          ClientId = entity.ClientId,
          DayOfWeek = entity.DayOfWeek,
          EntryDate = entity.EntryDate,
          DateId = entity.DateId
        };
      }
    }

    public static TimeSheetDateDto FromEntity(TimeSheetDate entity)
    {
      // TODO: [TESTS] (TimeSheetDateDto.FromEntity) Add tests
      return entity == null ? null : Projection.Compile()(entity);
    }
  }
}
