using Rn.NetCore.Common.Logging;
using Rn.NetCore.Common.Metrics;
using Rn.NetCore.DbCommon.Helpers;
using Rn.NetCore.DbCommon.Repos;
using TimeTracker.Core.Database.Queries;

namespace TimeTracker.Core.Database.Repos
{
  public interface ITimeSheetRowRepo
  {
  }

  public class TimeSheetRowRepo : BaseRepo<TimeSheetRowRepo>, ITimeSheetRowRepo
  {
    private readonly ITimeSheetRowRepoQueries _queries;

    public TimeSheetRowRepo(
      ILoggerAdapter<TimeSheetRowRepo> logger,
      IDbHelper dbHelper,
      IMetricService metrics,
      ITimeSheetRowRepoQueries queries)
      : base(logger, dbHelper, metrics, nameof(TimeSheetRowRepo), TargetDB.TimeTracker)
    {
      _queries = queries;
    }
  }
}
