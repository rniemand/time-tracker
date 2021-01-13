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
    Task<int> Pause(long entryId, TimerState state, string notes);
    Task<int> Complete(long entryId, TimerState state, string notes);
    Task<TrackedTimeEntity> GetByEntryId(long entryId);
    Task<int> Stop(long entryId, TimerState state);
    Task<List<TrackedTimeEntity>> GetProjectEntries(int projectId);
    Task<int> UpdateDuration(TrackedTimeEntity entity);
    Task<List<TrackedTimeEntity>> GetRunning(int userId);
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

    public async Task<int> Pause(long entryId, TimerState state, string notes)
    {
      // TODO: [TESTS] (TrackedTimeRepo.Pause) Add tests
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
      // TODO: [TESTS] (TrackedTimeRepo.Complete) Add tests
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

    public async Task<TrackedTimeEntity> GetByEntryId(long entryId)
    {
      // TODO: [TESTS] (TrackedTimeRepo.GetByEntryId) Add tests
      return await GetSingle<TrackedTimeEntity>(
        nameof(GetByEntryId),
        _queries.GetByEntryId(),
        new { EntryId = entryId }
      );
    }

    public async Task<int> Stop(long entryId, TimerState state)
    {
      // TODO: [TESTS] (TrackedTimeRepo.Stop) Add tests
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
  }
}
