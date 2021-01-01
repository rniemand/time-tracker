using Rn.NetCore.Common.Logging;
using Rn.NetCore.Common.Metrics;
using Rn.NetCore.DbCommon;
using TimeTracker.Core.Database.Queries;

namespace TimeTracker.Core.Database.Repos
{
  public interface ITrackedTimeRepo
  {
  }

  public class TrackedTimeRepo : BaseRepo<TrackedTimeRepo>, ITrackedTimeRepo
  {
    private readonly ITrackedTimeRepoQueries _queries;

    public TrackedTimeRepo(
      ILoggerAdapter<TrackedTimeRepo> logger,
      IDbHelper dbHelper,
      IMetricService metricService,
      ITrackedTimeRepoQueries queries)
      : base(logger, dbHelper, metricService, nameof(TrackedTimeRepo), TargetDB.TimeTracker)
    {
      _queries = queries;
    }
  }
}
