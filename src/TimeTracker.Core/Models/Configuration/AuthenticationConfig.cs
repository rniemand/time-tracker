namespace TimeTracker.Core.Models.Configuration
{
  public class AuthenticationConfig
  {
    public string Secret { get; set; }
    public int SessionLengthMin { get; set; }

    public AuthenticationConfig()
    {
      // TODO: [TESTS] (AuthenticationConfig) Add tests
      Secret = string.Empty;
      SessionLengthMin = 1440;
    }
  }
}