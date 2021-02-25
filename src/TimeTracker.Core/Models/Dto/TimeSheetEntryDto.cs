using System;
using System.Linq.Expressions;
using TimeTracker.Core.Database.Entities;

namespace TimeTracker.Core.Models.Dto
{
  public class TimeSheetEntryDto
  {
    public long EntryId { get; set; }
    public int UserId { get; set; }
    public int ClientId { get; set; }
    public int ProductId { get; set; }
    public int ProjectId { get; set; }
    public DateTime EntryDate { get; set; }
    public int EntryVersion { get; set; }
    public int EntryTimeMin { get; set; }

    public TimeSheetEntryDto()
    {
      // TODO: [TESTS] (TimeSheetEntryDto) Add tests
      EntryId = 0;
      UserId = 0;
      ClientId = 0;
      ProductId = 0;
      ProjectId = 0;
      EntryDate = DateTime.Now;
      EntryVersion = 1;
      EntryTimeMin = 0;
    }

    public static Expression<Func<TimeSheetEntry, TimeSheetEntryDto>> Projection
    {
      get
      {
        return entity => new TimeSheetEntryDto
        {
          UserId = entity.UserId,
          ClientId = entity.ClientId,
          ProductId = entity.ProductId,
          ProjectId = entity.ProjectId,
          EntryDate = entity.EntryDate,
          EntryId = entity.EntryId,
          EntryTimeMin = entity.EntryTimeMin,
          EntryVersion = entity.EntryVersion
        };
      }
    }

    public static TimeSheetEntryDto FromEntity(TimeSheetEntry entity)
    {
      // TODO: [TESTS] (TimeSheetEntryDto.FromEntity) Add tests
      return entity == null ? null : Projection.Compile()(entity);
    }
  }
}
