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
  public interface ITrackedTimeRepo
  {
    Task<int> StartNew(TrackedTimeEntity timerEntity);
    Task<TrackedTimeEntity> GetExisting(TrackedTimeEntity timerEntity);
    Task<List<TrackedTimeEntity>> GetActive(int userId);



    Task<TrackedTimeEntity> GetCurrentEntry(TrackedTimeEntity timerEntity); // revise
    Task<int> PauseTimer(long rawTimerId, EntryRunningState state, string notes); // revise
    Task<TrackedTimeEntity> GetByRawTimerId(long rawTimerId); // revise
    Task<int> FlagAsResumed(long rawTimerId); // revise
    Task<int> SpawnResumedTimer(TrackedTimeEntity timerEntity); // revise
    Task<int> SetRootTimerId(long rawTimerId, long rootTimerId); // revise
    Task<int> StopTimer(long rawTimerId); // revise
    Task<int> CompleteTimerSet(long rootTimerId); // revise
    Task<List<TrackedTimeEntity>> GetTimerSeries(long rootTimerId); // revise
    Task<List<KeyValueEntity<int, string>>> GetUsersWithRunningTimers(); // revise
    Task<List<TrackedTimeEntity>> GetLongRunningTimers(int userId, int thresholdSec); // revise
    Task<int> UpdateNotes(long rawTimerId, string notes); // revise
    Task<int> UpdateTimerDuration(TrackedTimeEntity timerEntity); // revise
    Task<List<TrackedTimeEntity>> GetRunningTimers(int userId); // revise
  }

  public class TrackedTimeRepo : BaseRepo<TrackedTimeRepo>, ITrackedTimeRepo
  {
    private readonly ITrackedTimeQueries _queries;

    public TrackedTimeRepo(
      ILoggerAdapter<TrackedTimeRepo> logger,
      IDbHelper dbHelper,
      IMetricService metricService,
      ITrackedTimeQueries queries)
      : base(logger, dbHelper, metricService, nameof(TrackedTimeRepo), TargetDB.TimeTracker)
    {
      _queries = queries;
    }

    public async Task<int> StartNew(TrackedTimeEntity timerEntity)
    {
      // TODO: [TESTS] (TrackedTimeRepo.StartNew) Add tests
      return await ExecuteAsync(
        nameof(StartNew),
        _queries.StartNew(),
        timerEntity
      );
    }

    public async Task<TrackedTimeEntity> GetExisting(TrackedTimeEntity timerEntity)
    {
      // TODO: [TESTS] (TrackedTimeRepo.GetExisting) Add tests
      return await GetSingle<TrackedTimeEntity>(
        nameof(GetExisting),
        _queries.GetExisting(),
        timerEntity
      );
    }

    public async Task<List<TrackedTimeEntity>> GetActive(int userId)
    {
      // TODO: [TESTS] (TrackedTimeRepo.GetRunningTimers) Add tests
      return await GetList<TrackedTimeEntity>(
        nameof(GetActive),
        _queries.GetActive(),
        new { UserId = userId }
      );
    }





    public async Task<TrackedTimeEntity> GetCurrentEntry(TrackedTimeEntity timerEntity)
    {
      // TODO: [TESTS] (TrackedTimeRepo.GetCurrentEntry) Add tests
      return await GetSingle<TrackedTimeEntity>(
        nameof(GetCurrentEntry),
        _queries.GetCurrentEntry(),
        timerEntity
      );
    }

    public async Task<int> PauseTimer(long rawTimerId, EntryRunningState state, string notes)
    {
      // TODO: [TESTS] (TrackedTimeRepo.PauseTimer) Add tests
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

    public async Task<TrackedTimeEntity> GetByRawTimerId(long rawTimerId)
    {
      // TODO: [TESTS] (TrackedTimeRepo.GetByRawTimerId) Add tests
      return await GetSingle<TrackedTimeEntity>(
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
      // TODO: [TESTS] (TrackedTimeRepo.FlagAsResumed) Add tests
      return await ExecuteAsync(
        nameof(FlagAsResumed),
        _queries.FlagAsResumed(),
        new
        {
          RawTimerId = rawTimerId
        }
      );
    }

    public async Task<int> SpawnResumedTimer(TrackedTimeEntity timerEntity)
    {
      // TODO: [TESTS] (TrackedTimeRepo.SpawnResumedTimer) Add tests
      return await ExecuteAsync(
        nameof(SpawnResumedTimer),
        _queries.SpawnResumedTimer(),
        timerEntity
      );
    }

    public async Task<int> SetRootTimerId(long rawTimerId, long rootTimerId)
    {
      // TODO: [TESTS] (TrackedTimeRepo.SetRootTimerId) Add tests
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
      // TODO: [TESTS] (TrackedTimeRepo.StopTimer) Add tests
      return await ExecuteAsync(
        nameof(StopTimer),
        _queries.StopTimer(),
        new { RawTimerId = rawTimerId }
      );
    }

    public async Task<int> CompleteTimerSet(long rootTimerId)
    {
      // TODO: [TESTS] (TrackedTimeRepo.CompleteTimerSet) Add tests
      return await ExecuteAsync(
        nameof(CompleteTimerSet),
        _queries.CompleteTimerSet(),
        new
        {
          RootTimerId = rootTimerId
        }
      );
    }

    public async Task<List<TrackedTimeEntity>> GetTimerSeries(long rootTimerId)
    {
      // TODO: [TESTS] (TrackedTimeRepo.GetTimerSeries) Add tests
      return await GetList<TrackedTimeEntity>(
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
      // TODO: [TESTS] (TrackedTimeRepo.GetUsersWithRunningTimers) Add tests
      return await GetList<KeyValueEntity<int, string>>(
        nameof(GetUsersWithRunningTimers),
        _queries.GetUsersWithRunningTimers()
      );
    }

    public async Task<List<TrackedTimeEntity>> GetLongRunningTimers(int userId, int thresholdSec)
    {
      // TODO: [TESTS] (TrackedTimeRepo.GetLongRunningTimers) Add tests
      return await GetList<TrackedTimeEntity>(
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
      // TODO: [TESTS] (TrackedTimeRepo.UpdateNotes) Add tests
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

    public async Task<int> UpdateTimerDuration(TrackedTimeEntity timerEntity)
    {
      // TODO: [TESTS] (TrackedTimeRepo.UpdateTimerDuration) Add tests
      return await ExecuteAsync(
        nameof(UpdateTimerDuration),
        _queries.UpdateTimerDuration(),
        timerEntity
      );
    }

    public async Task<List<TrackedTimeEntity>> GetRunningTimers(int userId)
    {
      // TODO: [TESTS] (TrackedTimeRepo.GetRunningTimers) Add tests
      return await GetList<TrackedTimeEntity>(
        nameof(GetActive),
        _queries.GetActive(),
        new
        {
          UserId = userId
        }
      );
    }
  }
}
