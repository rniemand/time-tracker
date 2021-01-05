namespace TimeTracker.Core.Models.Configuration
{
  public class TimeTrackerConfig
  {
    public AuthenticationConfig Authentication { get; set; }

    public TimeTrackerConfig()
    {
      // TODO: [TESTS] (TimeTrackerConfig) Add tests
      Authentication = new AuthenticationConfig();
    }
  }
}
