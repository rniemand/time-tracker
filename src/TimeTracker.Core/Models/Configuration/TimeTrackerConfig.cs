using Newtonsoft.Json;

namespace TimeTracker.Core.Models.Configuration
{
  public class TimeTrackerConfig
  {
    [JsonProperty("Authentication")]
    public AuthenticationConfig Authentication { get; set; }

    [JsonProperty("Hangfire")]
    public HangfireConfiguration Hangfire { get; set; }

    public TimeTrackerConfig()
    {
      // TODO: [TESTS] (TimeTrackerConfig) Add tests
      Authentication = new AuthenticationConfig();
      Hangfire = new HangfireConfiguration();
    }
  }
}
