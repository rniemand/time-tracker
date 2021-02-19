using System;
using System.Collections.Generic;
using TimeTracker.Core.Models.Dto;

namespace TimeTracker.Core.Models.Responses
{
  public class GetTimeSheetResponse
  {
    public List<TimeSheetDateDto> Dates { get; set; }
    public List<TimeSheetRowDto> Rows { get; set; }
    public List<TimeSheetEntryDto> Entries { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }

    public GetTimeSheetResponse()
    {
      // TODO: [TESTS] (GetTimeSheetResponse) Add tests
      Dates = new List<TimeSheetDateDto>();
      Rows = new List<TimeSheetRowDto>();
      Entries = new List<TimeSheetEntryDto>();
      StartDate = DateTime.Now;
      EndDate = DateTime.Now;
    }
  }
}
