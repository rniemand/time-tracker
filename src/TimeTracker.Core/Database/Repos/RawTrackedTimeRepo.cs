using System.Collections.Generic;
using System.Threading.Tasks;
using Rn.NetCore.Common.Logging;
using Rn.NetCore.Common.Metrics;
using Rn.NetCore.DbCommon;
using TimeTracker.Core.Database.Entities;
using TimeTracker.Core.Database.Queries;

namespace TimeTracker.Core.Database.Repos
{
  public interface IRawTrackedTimeRepo
  {
    Task<int> StartNew(RawTrackedTimeEntity entity);
    Task<RawTrackedTimeEntity> GetCurrentEntry(RawTrackedTimeEntity entity);
    Task<List<RawTrackedTimeEntity>> GetRunningTimers(int userId);
    Task<int> PauseTimer(long entryId);
    Task<RawTrackedTimeEntity> GetByEntryId(long entryId);
    Task<int> FlagAsResumed(RawTrackedTimeEntity entity);
    Task<int> SpawnResumedTimer(RawTrackedTimeEntity entity);
    Task<int> SetRootParentEntryId(long entryId, long rootParentEntryId);
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

    public async Task<List<RawTrackedTimeEntity>> GetRunningTimers(int userId)
    {
      // TODO: [TESTS] (RawTrackedTimeRepo.GetRunningTimers) Add tests
      return await GetList<RawTrackedTimeEntity>(
        nameof(GetRunningTimers),
        _queries.GetRunningTimers(),
        new { UserId = userId }
      );
    }

    public async Task<int> PauseTimer(long entryId)
    {
      // TODO: [TESTS] (RawTrackedTimeRepo.PauseTimer) Add tests
      return await ExecuteAsync(
        nameof(PauseTimer),
        _queries.PauseTimer(),
        new { EntryId = entryId }
      );
    }

    public async Task<RawTrackedTimeEntity> GetByEntryId(long entryId)
    {
      // TODO: [TESTS] (RawTrackedTimeRepo.GetByEntryId) Add tests
      return await GetSingle<RawTrackedTimeEntity>(
        nameof(GetByEntryId),
        _queries.GetByEntryId(),
        new { EntryId = entryId }
      );
    }

    public async Task<int> FlagAsResumed(RawTrackedTimeEntity entity)
    {
      // TODO: [TESTS] (RawTrackedTimeRepo.FlagAsResumed) Add tests
      return await ExecuteAsync(
        nameof(FlagAsResumed),
        _queries.FlagAsResumed(),
        entity
      );
    }

    public async Task<int> SpawnResumedTimer(RawTrackedTimeEntity entity)
    {
      // TODO: [TESTS] (RawTrackedTimeRepo.SpawnResumedTimer) Add tests
      return await ExecuteAsync(
        nameof(SpawnResumedTimer),
        _queries.SpawnResumedTimer(),
        entity
      );
    }

    public async Task<int> SetRootParentEntryId(long entryId, long rootParentEntryId)
    {
      // TODO: [TESTS] (RawTrackedTimeRepo.SetRootParentEntryId) Add tests
      return await ExecuteAsync(
        nameof(SetRootParentEntryId),
        _queries.SetRootParentEntryId(),
        new
        {
          EntryId = entryId,
          RootParentEntryId = rootParentEntryId
        }
      );
    }
  }
}
