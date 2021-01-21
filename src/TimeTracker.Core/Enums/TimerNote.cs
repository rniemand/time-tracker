namespace TimeTracker.Core.Enums
{
  public class TimerNote
  {
    public const string UserPaused = "user-paused";
    public const string Completed = "completed";
    public const string UserStopped = "user-stopped";
    public const string AutoPaused = "auto-paused";
    public const string AutoCompleted = "auto-completed";
    public const string UserCompleted = "user-completed";

    public static string GenerateTimerNote(string currentNote, string updatedNote)
    {
      // TODO: [TESTS] (TimerNote.GenerateTimerNote) Add tests
      if (string.IsNullOrWhiteSpace(currentNote))
        return updatedNote;

      if (IsGenericTimerNote(updatedNote))
        return IsGenericTimerNote(currentNote) ? updatedNote : currentNote;

      return updatedNote;
    }

    private static bool IsGenericTimerNote(string note)
    {
      // TODO: [TESTS] (TimerNote.IsGenericTimerNote) Add tests
      switch (note)
      {
        case UserPaused:
        case Completed:
        case UserStopped:
        case AutoPaused:
        case AutoCompleted:
        case UserCompleted:
          return true;

        default:
          return false;
      }
    }
  }
}