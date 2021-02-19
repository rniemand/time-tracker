using Rn.NetCore.Common.Logging;
using Rn.NetCore.Common.Metrics;
using Rn.NetCore.DbCommon.Helpers;
using Rn.NetCore.DbCommon.Repos;
using TimeTracker.Core.Database.Queries;

namespace TimeTracker.Core.Database.Repos
{
  public interface ITimeSheetDateRepo
  {
  }

  public class TimeSheetDateRepo : BaseRepo<TimeSheetDateRepo>, ITimeSheetDateRepo
  {
    private readonly ITimeSheetDateRepoQueries _queries;

    public TimeSheetDateRepo(
      ILoggerAdapter<TimeSheetDateRepo> logger,
      IDbHelper dbHelper,
      IMetricService metrics,
      ITimeSheetDateRepoQueries queries)
        : base(logger, dbHelper, metrics, nameof(TimeSheetDateRepo), TargetDB.TimeTracker)
    {
      _queries = queries;
    }
  }
}
