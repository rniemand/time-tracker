using System;
using System.Data;
using Dapper;

namespace TimeTracker.Core.Database
{
  public class DateTimeHandler : SqlMapper.TypeHandler<DateTime>
  {
    public override void SetValue(IDbDataParameter parameter, DateTime value)
    {
      parameter.Value = value;
    }

    public override DateTime Parse(object value)
    {
      // Support DateTimeOffset from SQL
      if (value is DateTimeOffset offset)
      {
        return offset.UtcDateTime;
      }

      // Force the value into a UTC DateTime
      return DateTime.SpecifyKind((DateTime)value, DateTimeKind.Utc);
    }
  }
}
