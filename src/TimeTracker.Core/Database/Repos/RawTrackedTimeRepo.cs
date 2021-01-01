using Rn.NetCore.Common.Logging;
using Rn.NetCore.Common.Metrics;
using Rn.NetCore.DbCommon;
using TimeTracker.Core.Database.Queries;

namespace TimeTracker.Core.Database.Repos
{
  public interface IRawTrackedTimeRepo
  {
  }

  public class RawTrackedTimeRepo : BaseRepo<RawTrackedTimeRepo>, IRawTrackedTimeRepo
  {
    private readonly IRawTrackedTimeRepoQueries _queries;

    public RawTrackedTimeRepo(
      ILoggerAdapter<RawTrackedTimeRepo> logger,
      IDbHelper dbHelper,
      IMetricService metricService,
      IRawTrackedTimeRepoQueries queries)
      : base(logger, dbHelper, metricService, nameof(RawTrackedTimeRepo), TargetDB.TimeTracker)
    {
      _queries = queries;
    }
  }
}
