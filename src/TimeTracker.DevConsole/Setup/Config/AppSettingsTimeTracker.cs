using Newtonsoft.Json;
using TimeTracker.Core.Models.Configuration;

namespace TimeTracker.DevConsole.Setup.Config
{
  public class AppSettingsTimeTracker
  {
    [JsonProperty("Authentication")]
    public AuthenticationConfig Authentication { get; set; }

    public AppSettingsTimeTracker()
    {
      // TODO: [TESTS] (AppSettingsTimeTracker) Add tests
      Authentication = new AuthenticationConfig();
    }
  }
}