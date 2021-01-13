namespace TimeTracker.Core.Enums
{
  public enum TimerState
  {
    Unknown = 0,
    Completed = 1, // maybe rename to ENDED
    Paused = 2,
    Stopped = 3,

    UserPaused = 9,
    UserStopped = 10,
    ServicePaused = 11,
    CronPaused = 12
  }
}
