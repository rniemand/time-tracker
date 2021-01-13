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
  public interface ITimerRepo
  {
    Task<int> StartNew(TimerEntity entity);
    Task<TimerEntity> GetRunningExisting(TimerEntity entity);
    Task<List<TimerEntity>> GetActive(int userId);
    Task<int> Pause(long entryId, TimerState state, string notes);
    Task<int> Complete(long entryId, TimerState state, string notes);
    Task<TimerEntity> GetByEntryId(long entryId);
    Task<int> Stop(long entryId, TimerState state);
    Task<List<TimerEntity>> GetProjectEntries(int projectId);
    Task<int> UpdateDuration(TimerEntity entity);
    Task<List<TimerEntity>> GetRunning(int userId);
  }

  public class TimerRepo : BaseRepo<TimerRepo>, ITimerRepo
  {
    private readonly ITimerQueries _queries;

    public TimerRepo(
      ILoggerAdapter<TimerRepo> logger,
      IDbHelper dbHelper,
      IMetricService metricService,
      ITimerQueries queries)
      : base(logger, dbHelper, metricService, nameof(TimerRepo), TargetDB.TimeTracker)
    {
      _queries = queries;
    }

    public async Task<int> StartNew(TimerEntity entity)
    {
      // TODO: [TESTS] (TimerRepo.StartNew) Add tests
      return await ExecuteAsync(
        nameof(StartNew),
        _queries.StartNew(),
        entity
      );
    }

    public async Task<TimerEntity> GetRunningExisting(TimerEntity entity)
    {
      // TODO: [TESTS] (TimerRepo.GetRunningExisting) Add tests
      return await GetSingle<TimerEntity>(
        nameof(GetRunningExisting),
        _queries.GetRunningExisting(),
        entity
      );
    }

    public async Task<List<TimerEntity>> GetActive(int userId)
    {
      // TODO: [TESTS] (TimerRepo.GetActive) Add tests
      return await GetList<TimerEntity>(
        nameof(GetActive),
        _queries.GetActive(),
        new { UserId = userId }
      );
    }

    public async Task<int> Pause(long entryId, TimerState state, string notes)
    {
      // TODO: [TESTS] (TimerRepo.Pause) Add tests
      return await ExecuteAsync(
        nameof(Pause),
        _queries.Pause(),
        new
        {
          EntryId = entryId,
          EntryState = state,
          Notes = notes
        }
      );
    }

    public async Task<int> Complete(long entryId, TimerState state, string notes)
    {
      // TODO: [TESTS] (TimerRepo.Complete) Add tests
      return await ExecuteAsync(
        nameof(Pause),
        _queries.Complete(),
        new
        {
          EntryId = entryId,
          EntryState = state,
          Notes = notes
        }
      );
    }

    public async Task<TimerEntity> GetByEntryId(long entryId)
    {
      // TODO: [TESTS] (TimerRepo.GetByEntryId) Add tests
      return await GetSingle<TimerEntity>(
        nameof(GetByEntryId),
        _queries.GetByEntryId(),
        new { EntryId = entryId }
      );
    }

    public async Task<int> Stop(long entryId, TimerState state)
    {
      // TODO: [TESTS] (TimerRepo.Stop) Add tests
      return await ExecuteAsync(
        nameof(Stop),
        _queries.Stop(),
        new
        {
          EntryId = entryId,
          EntryState = state
        }
      );
    }

    public async Task<List<TimerEntity>> GetProjectEntries(int projectId)
    {
      // TODO: [TESTS] (TimerRepo.GetTrackedTime) Add tests
      return await GetList<TimerEntity>(
        nameof(GetProjectEntries),
        _queries.GetProjectEntries(),
        new { ProjectId = projectId }
      );
    }

    public async Task<int> UpdateDuration(TimerEntity entity)
    {
      // TODO: [TESTS] (TimerRepo.UpdateDuration) Add tests
      return await ExecuteAsync(
        nameof(UpdateDuration),
        _queries.UpdateDuration(),
        entity
      );
    }

    public async Task<List<TimerEntity>> GetRunning(int userId)
    {
      // TODO: [TESTS] (TimerRepo.GetRunning) Add tests
      return await GetList<TimerEntity>(
        nameof(GetActive),
        _queries.GetRunning(),
        new { UserId = userId }
      );
    }
  }
}
