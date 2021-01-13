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
    Task<int> StartNew(TrackedTimeEntity entity);
    Task<TrackedTimeEntity> GetRunningExisting(TrackedTimeEntity entity);
    Task<List<TrackedTimeEntity>> GetActive(int userId);
    Task<int> Pause(long entryId, TimerEndReason endReason, string notes);
    Task<int> Complete(long entryId, TimerEndReason endReason, string notes);
    Task<TrackedTimeEntity> GetByEntryId(long entryId);
    Task<int> Stop(long entryId, TimerEndReason endReason);
    Task<List<TrackedTimeEntity>> GetProjectEntries(int projectId);
    Task<int> UpdateDuration(TrackedTimeEntity entity);
    Task<List<TrackedTimeEntity>> GetRunning(int userId);




    Task<TrackedTimeEntity> GetCurrentEntry(TrackedTimeEntity timerEntity); // revise
    Task<int> FlagAsResumed(long rawTimerId); // revise
    Task<int> SpawnResumedTimer(TrackedTimeEntity timerEntity); // revise
    Task<int> SetRootTimerId(long rawTimerId, long rootTimerId); // revise
    Task<int> CompleteTimerSet(long rootTimerId); // revise
    Task<List<KeyValueEntity<int, string>>> GetUsersWithRunningTimers(); // revise
    Task<List<TrackedTimeEntity>> GetLongRunningTimers(int userId, int thresholdSec); // revise
    Task<int> UpdateNotes(long rawTimerId, string notes); // revise
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

    public async Task<int> StartNew(TrackedTimeEntity entity)
    {
      // TODO: [TESTS] (TrackedTimeRepo.StartNew) Add tests
      return await ExecuteAsync(
        nameof(StartNew),
        _queries.StartNew(),
        entity
      );
    }

    public async Task<TrackedTimeEntity> GetRunningExisting(TrackedTimeEntity entity)
    {
      // TODO: [TESTS] (TrackedTimeRepo.GetRunningExisting) Add tests
      return await GetSingle<TrackedTimeEntity>(
        nameof(GetRunningExisting),
        _queries.GetRunningExisting(),
        entity
      );
    }

    public async Task<List<TrackedTimeEntity>> GetActive(int userId)
    {
      // TODO: [TESTS] (TrackedTimeRepo.GetActive) Add tests
      return await GetList<TrackedTimeEntity>(
        nameof(GetActive),
        _queries.GetActive(),
        new { UserId = userId }
      );
    }

    public async Task<int> Pause(long entryId, TimerEndReason endReason, string notes)
    {
      // TODO: [TESTS] (TrackedTimeRepo.Pause) Add tests
      return await ExecuteAsync(
        nameof(Pause),
        _queries.Pause(),
        new
        {
          EntryId = entryId,
          EndReason = endReason,
          Notes = notes
        }
      );
    }

    public async Task<int> Complete(long entryId, TimerEndReason endReason, string notes)
    {
      // TODO: [TESTS] (TrackedTimeRepo.Complete) Add tests
      return await ExecuteAsync(
        nameof(Pause),
        _queries.Complete(),
        new
        {
          EntryId = entryId,
          EndReason = endReason,
          Notes = notes
        }
      );
    }

    public async Task<TrackedTimeEntity> GetByEntryId(long entryId)
    {
      // TODO: [TESTS] (TrackedTimeRepo.GetByEntryId) Add tests
      return await GetSingle<TrackedTimeEntity>(
        nameof(GetByEntryId),
        _queries.GetByEntryId(),
        new { EntryId = entryId }
      );
    }

    public async Task<int> Stop(long entryId, TimerEndReason endReason)
    {
      // TODO: [TESTS] (TrackedTimeRepo.Stop) Add tests
      return await ExecuteAsync(
        nameof(Stop),
        _queries.Stop(),
        new
        {
          EntryId = entryId,
          EndReason = endReason
        }
      );
    }

    public async Task<List<TrackedTimeEntity>> GetProjectEntries(int projectId)
    {
      // TODO: [TESTS] (TrackedTimeRepo.GetTrackedTime) Add tests
      return await GetList<TrackedTimeEntity>(
        nameof(GetProjectEntries),
        _queries.GetProjectEntries(),
        new { ProjectId = projectId }
      );
    }

    public async Task<int> UpdateDuration(TrackedTimeEntity entity)
    {
      // TODO: [TESTS] (TrackedTimeRepo.UpdateDuration) Add tests
      return await ExecuteAsync(
        nameof(UpdateDuration),
        _queries.UpdateDuration(),
        entity
      );
    }

    public async Task<List<TrackedTimeEntity>> GetRunning(int userId)
    {
      // TODO: [TESTS] (TrackedTimeRepo.GetRunning) Add tests
      return await GetList<TrackedTimeEntity>(
        nameof(GetActive),
        _queries.GetRunning(),
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
  }
}
