using Newtonsoft.Json;
using Rn.NetCore.Common.Encryption;

namespace TimeTracker.DevConsole.Setup.Config
{
  public class AppSettingsRnCore
  {
    [JsonProperty("Encryption")]
    public EncryptionConfig Encryption { get; set; }

    public AppSettingsRnCore()
    {
      // TODO: [TESTS] (AppSettingsRnCore) Add tests
      Encryption = new EncryptionConfig();
    }
  }
}