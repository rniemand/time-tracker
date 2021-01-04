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
    Task<int> StartNew(RawTimerEntity timerEntity);
    Task<RawTimerEntity> GetCurrentEntry(RawTimerEntity timerEntity);
    Task<List<RawTimerEntity>> GetRunningTimers(int userId);
    Task<int> PauseTimer(long rawTimerId);
    Task<RawTimerEntity> GetByRawTimerId(long rawTimerId);
    Task<int> FlagAsResumed(long rawTimerId);
    Task<int> SpawnResumedTimer(RawTimerEntity timerEntity);
    Task<int> SetRootTimerId(long rawTimerId, long rootTimerId);
    Task<int> StopTimer(long rawTimerId);
    Task<int> CompleteTimer(long rootTimerId);
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

    public async Task<int> StartNew(RawTimerEntity timerEntity)
    {
      // TODO: [TESTS] (RawTimersRepo.StartNew) Add tests
      return await ExecuteAsync(
        nameof(StartNew),
        _queries.StartNew(),
        timerEntity
      );
    }

    public async Task<RawTimerEntity> GetCurrentEntry(RawTimerEntity timerEntity)
    {
      // TODO: [TESTS] (RawTimersRepo.GetCurrentEntry) Add tests
      return await GetSingle<RawTimerEntity>(
        nameof(GetCurrentEntry),
        _queries.GetCurrentEntry(),
        timerEntity
      );
    }

    public async Task<List<RawTimerEntity>> GetRunningTimers(int userId)
    {
      // TODO: [TESTS] (RawTimersRepo.GetRunningTimers) Add tests
      return await GetList<RawTimerEntity>(
        nameof(GetRunningTimers),
        _queries.GetRunningTimers(),
        new
        {
          UserId = userId
        }
      );
    }

    public async Task<int> PauseTimer(long rawTimerId)
    {
      // TODO: [TESTS] (RawTimersRepo.PauseTimer) Add tests
      return await ExecuteAsync(
        nameof(PauseTimer),
        _queries.PauseTimer(),
        new
        {
          RawTimerId = rawTimerId
        }
      );
    }

    public async Task<RawTimerEntity> GetByRawTimerId(long rawTimerId)
    {
      // TODO: [TESTS] (RawTimersRepo.GetByRawTimerId) Add tests
      return await GetSingle<RawTimerEntity>(
        nameof(GetByRawTimerId),
        _queries.GetByRawTimerId(),
        new
        {
          RawTimerId = rawTimerId
        }
      );
    }

    public async Task<int> FlagAsResumed(long rawTimerId)
    {
      // TODO: [TESTS] (RawTimersRepo.FlagAsResumed) Add tests
      return await ExecuteAsync(
        nameof(FlagAsResumed),
        _queries.FlagAsResumed(),
        new
        {
          RawTimerId = rawTimerId
        }
      );
    }

    public async Task<int> SpawnResumedTimer(RawTimerEntity timerEntity)
    {
      // TODO: [TESTS] (RawTimersRepo.SpawnResumedTimer) Add tests
      return await ExecuteAsync(
        nameof(SpawnResumedTimer),
        _queries.SpawnResumedTimer(),
        timerEntity
      );
    }

    public async Task<int> SetRootTimerId(long rawTimerId, long rootTimerId)
    {
      // TODO: [TESTS] (RawTimersRepo.SetRootTimerId) Add tests
      return await ExecuteAsync(
        nameof(SetRootTimerId),
        _queries.SetRootTimerId(),
        new
        {
          RawTimerId = rawTimerId,
          RootTimerId = rootTimerId
        }
      );
    }

    public async Task<int> StopTimer(long rawTimerId)
    {
      // TODO: [TESTS] (RawTimersRepo.StopTimer) Add tests
      return await ExecuteAsync(
        nameof(StopTimer),
        _queries.StopTimer(),
        new { RawTimerId = rawTimerId }
      );
    }

    public async Task<int> CompleteTimer(long rootTimerId)
    {
      // TODO: [TESTS] (RawTimersRepo.CompleteTimer) Add tests
      return await ExecuteAsync(
        nameof(CompleteTimer),
        _queries.CompleteTimer(),
        new
        {
          RootTimerId = rootTimerId
        }
      );
    }
  }
}
