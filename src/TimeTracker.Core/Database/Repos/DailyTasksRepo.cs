using Rn.NetCore.Common.Logging;
using Rn.NetCore.Common.Metrics;
using Rn.NetCore.DbCommon;
using TimeTracker.Core.Database.Queries;

namespace TimeTracker.Core.Database.Repos
{
  public interface IDailyTasksRepo
  {
  }

  public class DailyTasksRepo : BaseRepo<DailyTasksRepo>, IDailyTasksRepo
  {
    private readonly IDailyTasksQueries _queries;

    public DailyTasksRepo(
      ILoggerAdapter<DailyTasksRepo> logger,
      IDbHelper dbHelper,
      IMetricService metricService,
      IDailyTasksQueries queries) : base(logger, dbHelper, metricService, nameof(DailyTasksRepo), TargetDB.TimeTracker)
    {
      _queries = queries;
    }
  }
}
