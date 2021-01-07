using System.Collections.Generic;
using Newtonsoft.Json;

namespace TimeTracker.DevConsole.Setup.Config
{
  public class SampleAppSettings
  {
    [JsonProperty("Logging")]
    public AppSettingsLogging Logging { get; set; }

    [JsonProperty("ConnectionStrings")]
    public Dictionary<string, string> ConnectionStrings { get; set; }

    [JsonProperty("RnCore")]
    public AppSettingsRnCore RnCore { get; set; }

    [JsonProperty("TimeTracker")]
    public AppSettingsTimeTracker TimeTracker { get; set; }

    public SampleAppSettings()
    {
      // TODO: [TESTS] (SampleAppSettings) Add tests
      Logging = new AppSettingsLogging();
      RnCore = new AppSettingsRnCore();
      TimeTracker = new AppSettingsTimeTracker();

      ConnectionStrings = new Dictionary<string, string>
      {
        {"TimeTracker", "Server=<host>;Uid=<user>;Pwd=<pass>;Database=<database>;Allow User Variables=true"}
      };
    }
  }
}
