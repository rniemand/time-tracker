using TimeTracker.Core.WebApi.Models;

namespace TimeTracker
{
  public class DerivedBaseApiRequest : BaseApiRequest
  {
    public string Test { get; set; }

    public DerivedBaseApiRequest()
    {
      Test = string.Empty;
    }
  }
}
