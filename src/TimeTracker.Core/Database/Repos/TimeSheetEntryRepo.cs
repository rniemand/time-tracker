using Rn.NetCore.Common.Logging;
using Rn.NetCore.Common.Metrics;
using Rn.NetCore.DbCommon.Helpers;
using Rn.NetCore.DbCommon.Repos;
using TimeTracker.Core.Database.Queries;

namespace TimeTracker.Core.Database.Repos
{
  public interface ITimeSheetEntryRepo
  {
  }

  public class TimeSheetEntryRepo : BaseRepo<TimeSheetEntryRepo>, ITimeSheetEntryRepo
  {
    private readonly ITimeSheetEntryRepoQueries _queries;

    public TimeSheetEntryRepo(
      ILoggerAdapter<TimeSheetEntryRepo> logger,
      IDbHelper dbHelper,
      IMetricService metrics,
      ITimeSheetEntryRepoQueries queries)
      : base(logger, dbHelper, metrics, nameof(TimeSheetEntryRepo), TargetDB.TimeTracker)
    {
      _queries = queries;
    }
  }
}
