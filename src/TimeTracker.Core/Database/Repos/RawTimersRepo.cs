using System.Collections.Generic;
using System.Threading.Tasks;
using Rn.NetCore.Common.Logging;
using Rn.NetCore.Common.Metrics;
using Rn.NetCore.DbCommon;
using TimeTracker.Core.Database.Entities;
using TimeTracker.Core.Database.Queries;

namespace TimeTracker.Core.Database.Repos
{
  public interface IRawTimersRepo
  {
    Task<int> StartNew(RawTrackedTimeEntity entity);
    Task<RawTrackedTimeEntity> GetCurrentEntry(RawTrackedTimeEntity entity);
    Task<List<RawTrackedTimeEntity>> GetRunningTimers(int userId);
    Task<int> PauseTimer(long entryId);
    Task<RawTrackedTimeEntity> GetByEntryId(long entryId);
    Task<int> FlagAsResumed(long entryId);
    Task<int> SpawnResumedTimer(RawTrackedTimeEntity entity);
    Task<int> SetRootParentEntryId(long entryId, long rootParentEntryId);
    Task<int> StopTimer(long entryId);
    Task<int> CompleteTimer(long rootParentEntryId);
  }

  public class RawTimersRepo : BaseRepo<RawTimersRepo>, IRawTimersRepo
  {
    private readonly IRawTimersRepoQueries _queries;

    public RawTimersRepo(
      ILoggerAdapter<RawTimersRepo> logger,
      IDbHelper dbHelper,
      IMetricService metricService,
      IRawTimersRepoQueries queries)
      : base(logger, dbHelper, metricService, nameof(RawTimersRepo), TargetDB.TimeTracker)
    {
      _queries = queries;
    }

    public async Task<int> StartNew(RawTrackedTimeEntity entity)
    {
      // TODO: [TESTS] (RawTimersRepo.StartNew) Add tests
      return await ExecuteAsync(nameof(StartNew), _queries.StartNew(), entity);
    }

    public async Task<RawTrackedTimeEntity> GetCurrentEntry(RawTrackedTimeEntity entity)
    {
      // TODO: [TESTS] (RawTimersRepo.GetCurrentEntry) Add tests
      return await GetSingle<RawTrackedTimeEntity>(nameof(GetCurrentEntry), _queries.GetCurrentEntry(), entity);
    }

    public async Task<List<RawTrackedTimeEntity>> GetRunningTimers(int userId)
    {
      // TODO: [TESTS] (RawTimersRepo.GetRunningTimers) Add tests
      return await GetList<RawTrackedTimeEntity>(
        nameof(GetRunningTimers),
        _queries.GetRunningTimers(),
        new { UserId = userId }
      );
    }

    public async Task<int> PauseTimer(long entryId)
    {
      // TODO: [TESTS] (RawTimersRepo.PauseTimer) Add tests
      return await ExecuteAsync(
        nameof(PauseTimer),
        _queries.PauseTimer(),
        new { EntryId = entryId }
      );
    }

    public async Task<RawTrackedTimeEntity> GetByEntryId(long entryId)
    {
      // TODO: [TESTS] (RawTimersRepo.GetByEntryId) Add tests
      return await GetSingle<RawTrackedTimeEntity>(
        nameof(GetByEntryId),
        _queries.GetByEntryId(),
        new { EntryId = entryId }
      );
    }

    public async Task<int> FlagAsResumed(long entryId)
    {
      // TODO: [TESTS] (RawTimersRepo.FlagAsResumed) Add tests
      return await ExecuteAsync(
        nameof(FlagAsResumed),
        _queries.FlagAsResumed(),
        new { EntryId = entryId }
      );
    }

    public async Task<int> SpawnResumedTimer(RawTrackedTimeEntity entity)
    {
      // TODO: [TESTS] (RawTimersRepo.SpawnResumedTimer) Add tests
      return await ExecuteAsync(
        nameof(SpawnResumedTimer),
        _queries.SpawnResumedTimer(),
        entity
      );
    }

    public async Task<int> SetRootParentEntryId(long entryId, long rootParentEntryId)
    {
      // TODO: [TESTS] (RawTimersRepo.SetRootParentEntryId) Add tests
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

    public async Task<int> StopTimer(long entryId)
    {
      // TODO: [TESTS] (RawTimersRepo.StopTimer) Add tests
      return await ExecuteAsync(
        nameof(StopTimer),
        _queries.StopTimer(),
        new { EntryId = entryId }
      );
    }

    public async Task<int> CompleteTimer(long rootParentEntryId)
    {
      // TODO: [TESTS] (RawTimersRepo.CompleteTimer) Add tests
      return await ExecuteAsync(
        nameof(CompleteTimer),
        _queries.CompleteTimer(),
        new { RootParentEntryId = rootParentEntryId }
      );
    }
  }
}
