using Newtonsoft.Json;

namespace TimeTracker.Core.Models.Configuration
{
  public class HangfireConfiguration
  {
    [JsonProperty("IgnoreAntiforgeryToken")]
    public bool IgnoreAntiforgeryToken { get; set; }

    [JsonProperty("PathMatch")]
    public string PathMatch { get; set; }

    [JsonProperty("DashboardTitle")]
    public string DashboardTitle { get; set; }

    public HangfireConfiguration()
    {
      // TODO: [TESTS] (HangfireConfiguration) Add tests
      IgnoreAntiforgeryToken = false;
      PathMatch = "/hangfire";
      DashboardTitle = "Time Tracker";
    }
  }
}
