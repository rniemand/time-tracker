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
    Task<TimerEntity> GetRunningTimer(TimerEntity timerEntity);
    Task<int> AddTimer(TimerEntity timerEntity);
    Task<int> StartNew(TimerEntity entity); // review
    Task<TimerEntity> GetRunningExisting(TimerEntity entity); // review
    Task<List<TimerEntity>> GetActiveTimers(int userId);
    Task<int> PauseTimer(long entryId, TimerState state, string notes);
    Task<int> Complete(long entryId, TimerState state, string notes); // review
    Task<TimerEntity> GetTimerById(long entryId);
    Task<int> Stop(long entryId, TimerState state); // review
    Task<List<TimerEntity>> GetProjectEntries(int projectId); // review
    Task<int> UpdateDuration(TimerEntity entity); // review
    Task<List<TimerEntity>> GetRunning(int userId); // review
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

    public async Task<TimerEntity> GetRunningTimer(TimerEntity timerEntity)
    {
      // TODO: [TESTS] (TimerRepo.GetRunningTimer) Add tests
      return await GetSingle<TimerEntity>(
        nameof(GetRunningTimer),
        _queries.GetRunningTimer(),
        timerEntity
      );
    }

    public async Task<int> AddTimer(TimerEntity timerEntity)
    {
      // TODO: [TESTS] (TimerRepo.AddTimer) Add tests
      return await ExecuteAsync(
        nameof(AddTimer),
        _queries.AddTimer(),
        timerEntity
      );
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

    public async Task<List<TimerEntity>> GetActiveTimers(int userId)
    {
      // TODO: [TESTS] (TimerRepo.GetActiveTimers) Add tests
      return await GetList<TimerEntity>(
        nameof(GetActiveTimers),
        _queries.GetActiveTimers(),
        new { UserId = userId }
      );
    }

    public async Task<int> PauseTimer(long entryId, TimerState state, string notes)
    {
      // TODO: [TESTS] (TimerRepo.PauseTimer) Add tests
      return await ExecuteAsync(
        nameof(PauseTimer),
        _queries.PauseTimer(),
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
        nameof(PauseTimer),
        _queries.Complete(),
        new
        {
          EntryId = entryId,
          EntryState = state,
          Notes = notes
        }
      );
    }

    public async Task<TimerEntity> GetTimerById(long entryId)
    {
      // TODO: [TESTS] (TimerRepo.GetTimerById) Add tests
      return await GetSingle<TimerEntity>(
        nameof(GetTimerById),
        _queries.GetTimerById(),
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
        nameof(GetActiveTimers),
        _queries.GetRunning(),
        new { UserId = userId }
      );
    }
  }
}
