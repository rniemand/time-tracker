namespace TimeTracker.Core.Enums
{
  public enum TimerState
  {
    Unknown = 0,
    Completed = 1,
    UserPaused = 2,
    UserStopped = 3,
    ServicePaused = 4,
    CronPaused = 5
  }
}
