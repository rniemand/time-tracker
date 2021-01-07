using System.Collections.Generic;
using Newtonsoft.Json;

namespace TimeTracker.DevConsole.Setup.Config
{
  public class AppSettingsLogging
  {
    [JsonProperty("LogLevel")]
    public Dictionary<string, string> LogLevel { get; set; }

    public AppSettingsLogging()
    {
      // TODO: [TESTS] (AppSettingsLogging) Add tests
      LogLevel = new Dictionary<string, string>
      {
        {"Default", "Warning"}
      };
    }
  }
}