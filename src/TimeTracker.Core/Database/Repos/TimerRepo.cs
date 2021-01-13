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
    Task<List<TimerEntity>> GetActiveTimers(int userId);
    Task<List<TimerEntity>> GetProjectTimers(int projectId);
    Task<List<TimerEntity>> GetRunningTimers(int userId);
    Task<List<TimerEntity>> GetLongRunningTimers(int userId, int thresholdSec);

    Task<int> AddTimer(TimerEntity timerEntity);
    Task<int> PauseTimer(long entryId, string notes);
    Task<int> CompleteTimer(long entryId, string notes);
    Task<TimerEntity> GetTimerById(long entryId);
    Task<TimerEntity> GetActiveTimer(TimerEntity timerEntity);
    Task<int> UpdateTimerDuration(TimerEntity entity);

    Task<List<KeyValueEntity<int, string>>> GetUsersWithRunningTimers();
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


    public async Task<List<TimerEntity>> GetActiveTimers(int userId)
    {
      // TODO: [TESTS] (TimerRepo.GetActiveTimers) Add tests
      return await GetList<TimerEntity>(
        nameof(GetActiveTimers),
        _queries.GetActiveTimers(),
        new { UserId = userId }
      );
    }

    public async Task<List<TimerEntity>> GetProjectTimers(int projectId)
    {
      // TODO: [TESTS] (TimerRepo.GetProjectTimers) Add tests
      return await GetList<TimerEntity>(
        nameof(GetProjectTimers),
        _queries.GetProjectTimers(),
        new
        {
          ProjectId = projectId
        }
      );
    }

    public async Task<List<TimerEntity>> GetRunningTimers(int userId)
    {
      // TODO: [TESTS] (TimerRepo.GetRunningTimers) Add tests
      return await GetList<TimerEntity>(
        nameof(GetActiveTimers),
        _queries.GetRunningTimers(),
        new
        {
          UserId = userId
        }
      );
    }

    public async Task<List<TimerEntity>> GetLongRunningTimers(int userId, int thresholdSec)
    {
      // TODO: [TESTS] (TimerRepo.GetLongRunningTimers) Add tests
      return await GetList<TimerEntity>(
        nameof(GetLongRunningTimers),
        _queries.GetLongRunningTimers(),
        new
        {
          UserId = userId,
          ThresholdSec = thresholdSec
        }
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

    public async Task<int> PauseTimer(long entryId, string notes)
    {
      // TODO: [TESTS] (TimerRepo.PauseTimer) Add tests
      return await ExecuteAsync(
        nameof(PauseTimer),
        _queries.PauseTimer(),
        new
        {
          EntryId = entryId,
          EntryState = TimerState.Paused,
          Notes = notes
        }
      );
    }

    public async Task<int> CompleteTimer(long entryId, string notes)
    {
      // TODO: [TESTS] (TimerRepo.CompleteTimer) Add tests
      return await ExecuteAsync(
        nameof(CompleteTimer),
        _queries.CompleteTimer(),
        new
        {
          EntryState = TimerState.Completed,
          EntryId = entryId,
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

    public async Task<TimerEntity> GetActiveTimer(TimerEntity timerEntity)
    {
      // TODO: [TESTS] (TimerRepo.GetActiveTimer) Add tests
      return await GetSingle<TimerEntity>(
        nameof(GetActiveTimer),
        _queries.GetActiveTimer(),
        timerEntity
      );
    }

    public async Task<int> UpdateTimerDuration(TimerEntity entity)
    {
      // TODO: [TESTS] (TimerRepo.UpdateTimerDuration) Add tests
      return await ExecuteAsync(
        nameof(UpdateTimerDuration),
        _queries.UpdateTimerDuration(),
        entity
      );
    }


    public async Task<List<KeyValueEntity<int, string>>> GetUsersWithRunningTimers()
    {
      // TODO: [TESTS] (TimerRepo.GetUsersWithRunningTimers) Add tests
      return await GetList<KeyValueEntity<int, string>>(
        nameof(GetUsersWithRunningTimers),
        _queries.GetUsersWithRunningTimers()
      );
    }
  }
}
