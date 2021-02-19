using System;

namespace TimeTracker.Core.Models.Requests
{
  public class GetTimeSheetRequest
  {
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }

    public GetTimeSheetRequest()
    {
      // TODO: [TESTS] (GetTimeSheetRequest.GetTimeSheetRequest) Add tests
      StartDate = DateTime.Now;
      EndDate = StartDate.AddDays(7);
    }
  }
}
