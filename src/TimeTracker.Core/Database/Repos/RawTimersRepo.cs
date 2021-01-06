using System.Collections.Generic;
using System.Threading.Tasks;
using Rn.NetCore.Common.Logging;
using Rn.NetCore.Common.Metrics;
using Rn.NetCore.DbCommon;
using TimeTracker.Core.Database.Entities;
using TimeTracker.Core.Database.Queries;
using TimeTracker.Core.Enums;

namespace TimeTracker.Core.Database.Repos
{
  public interface IRawTimersRepo
  {
    Task<int> StartNew(RawTimerEntity timerEntity);
    Task<RawTimerEntity> GetCurrentEntry(RawTimerEntity timerEntity);
    Task<List<RawTimerEntity>> GetRunningTimers(int userId);
    Task<int> PauseTimer(long rawTimerId, EntryRunningState state, string notes);
    Task<RawTimerEntity> GetByRawTimerId(long rawTimerId);
    Task<int> FlagAsResumed(long rawTimerId);
    Task<int> SpawnResumedTimer(RawTimerEntity timerEntity);
    Task<int> SetRootTimerId(long rawTimerId, long rootTimerId);
    Task<int> StopTimer(long rawTimerId);
    Task<int> CompleteTimerSet(long rootTimerId);
    Task<List<RawTimerEntity>> GetTimerSeries(long rootTimerId);
    Task<List<KeyValueEntity<int, string>>> GetUsersWithRunningTimers();
    Task<List<RawTimerEntity>> GetLongRunningTimers(int userId, int thresholdSec);
    Task<int> UpdateNotes(long rawTimerId, string notes);
    Task<int> UpdateTimerDuration(RawTimerEntity timerEntity);
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

    public async Task<int> PauseTimer(long rawTimerId, EntryRunningState state, string notes)
    {
      // TODO: [TESTS] (RawTimersRepo.PauseTimer) Add tests
      return await ExecuteAsync(
        nameof(PauseTimer),
        _queries.PauseTimer(),
        new
        {
          RawTimerId = rawTimerId,
          TimerNotes = notes,
          EntryState = state
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

    public async Task<int> CompleteTimerSet(long rootTimerId)
    {
      // TODO: [TESTS] (RawTimersRepo.CompleteTimerSet) Add tests
      return await ExecuteAsync(
        nameof(CompleteTimerSet),
        _queries.CompleteTimerSet(),
        new
        {
          RootTimerId = rootTimerId
        }
      );
    }

    public async Task<List<RawTimerEntity>> GetTimerSeries(long rootTimerId)
    {
      // TODO: [TESTS] (RawTimersRepo.GetTimerSeries) Add tests
      return await GetList<RawTimerEntity>(
        nameof(GetTimerSeries),
        _queries.GetTimerSeries(),
        new
        {
          RootTimerId = rootTimerId
        }
      );
    }

    public async Task<List<KeyValueEntity<int, string>>> GetUsersWithRunningTimers()
    {
      // TODO: [TESTS] (RawTimersRepo.GetUsersWithRunningTimers) Add tests
      return await GetList<KeyValueEntity<int, string>>(
        nameof(GetUsersWithRunningTimers),
        _queries.GetUsersWithRunningTimers()
      );
    }

    public async Task<List<RawTimerEntity>> GetLongRunningTimers(int userId, int thresholdSec)
    {
      // TODO: [TESTS] (RawTimersRepo.GetLongRunningTimers) Add tests
      return await GetList<RawTimerEntity>(
        nameof(GetLongRunningTimers),
        _queries.GetLongRunningTimers(),
        new
        {
          UserId = userId,
          ThresholdSec = thresholdSec
        }
      );
    }

    public async Task<int> UpdateNotes(long rawTimerId, string notes)
    {
      // TODO: [TESTS] (RawTimersRepo.UpdateNotes) Add tests
      return await ExecuteAsync(
        nameof(UpdateNotes),
        _queries.UpdateNotes(),
        new
        {
          RawTimerId = rawTimerId,
          TimerNotes = notes
        }
      );
    }

    public async Task<int> UpdateTimerDuration(RawTimerEntity timerEntity)
    {
      // TODO: [TESTS] (RawTimersRepo.UpdateTimerDuration) Add tests
      return await ExecuteAsync(
        nameof(UpdateTimerDuration),
        _queries.UpdateTimerDuration(),
        timerEntity
      );
    }
  }
}
