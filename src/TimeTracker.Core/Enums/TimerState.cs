namespace TimeTracker.Core.Enums
{
  public enum TimerState
  {
    Unknown = 0,
    Completed = 1, // maybe rename to ENDED
    Paused = 2,
    UserPaused = 9,
    UserStopped = 3,
    ServicePaused = 4,
    CronPaused = 5
  }
}
