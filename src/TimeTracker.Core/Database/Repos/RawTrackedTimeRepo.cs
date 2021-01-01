using System.Threading.Tasks;
using Rn.NetCore.Common.Logging;
using Rn.NetCore.Common.Metrics;
using Rn.NetCore.DbCommon;
using TimeTracker.Core.Database.Entities;
using TimeTracker.Core.Database.Queries;
using TimeTracker.Core.Models.Dto;

namespace TimeTracker.Core.Database.Repos
{
  public interface IRawTrackedTimeRepo
  {
    Task<int> StartNew(RawTrackedTimeEntity entity);
    Task<RawTrackedTimeEntity> GetCurrentEntry(RawTrackedTimeEntity entity);
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

    public async Task<int> StartNew(RawTrackedTimeEntity entity)
    {
      // TODO: [TESTS] (RawTrackedTimeRepo.StartNew) Add tests
      return await ExecuteAsync(nameof(StartNew), _queries.StartNew(), entity);
    }

    public async Task<RawTrackedTimeEntity> GetCurrentEntry(RawTrackedTimeEntity entity)
    {
      // TODO: [TESTS] (RawTrackedTimeRepo.GetCurrentEntry) Add tests
      return await GetSingle<RawTrackedTimeEntity>(nameof(GetCurrentEntry), _queries.GetCurrentEntry(), entity);
    }
  }
}
