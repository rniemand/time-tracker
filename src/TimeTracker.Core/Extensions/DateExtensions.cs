using System;

namespace TimeTracker.Core.Extensions
{
  public static class DateExtensions
  {
    public static string ToShortDbDate(this DateTime date)
      => date.ToString("yyyy-MM-dd");
  }
}
