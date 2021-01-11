using Newtonsoft.Json;

namespace TimeTracker.Core.Models.Configuration
{
  public class AuthenticationConfig
  {
    [JsonProperty("Secret")]
    public string Secret { get; set; }

    [JsonProperty("SessionLengthMin")]
    public int SessionLengthMin { get; set; }

    public AuthenticationConfig()
    {
      // TODO: [TESTS] (AuthenticationConfig) Add tests
      Secret = string.Empty;
      SessionLengthMin = 1440;
    }
  }
}