using System;
using System.Collections.Generic;
using TimeTracker.Core.Models.Dto;

namespace TimeTracker.Core.Models.Responses
{
  public class GetTimeSheetResponse
  {
    public List<ProjectDto> Projects { get; set; }
    public List<ProductDto> Products { get; set; }
    public List<TimeSheetEntryDto> Entries { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int DayCount { get; set; }

    public GetTimeSheetResponse()
    {
      // TODO: [TESTS] (GetTimeSheetResponse) Add tests
      Projects = new List<ProjectDto>();
      Products = new List<ProductDto>();
      Entries = new List<TimeSheetEntryDto>();
      StartDate = DateTime.Now;
      EndDate = DateTime.Now;
      DayCount = 1;
    }
  }
}
