using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TimeTracker.Core.Database.Entities;
using TimeTracker.Core.Database.Repos;
using TimeTracker.Core.Enums;
using TimeTracker.Core.Models.Dto;

namespace TimeTracker.Core.Services
{
  public interface IRawTimerService
  {
    Task<RawTimerDto> StartNew(int userId, RawTimerDto timerDto);
    Task<List<RawTimerDto>> GetActiveTimers(int userId);
    Task<bool> PauseTimer(int userId, long rawTimerId, EntryRunningState state, string notes);
    Task<bool> ResumeTimer(int userId, long rawTimerId);
    Task<bool> StopTimer(int userId, long rawTimerId);
    Task<List<RawTimerDto>> GetTimerSeries(int userId, long rootTimerId);
    Task<bool> UpdateNotes(int userId, long rawTimerId, string notes);
    Task<bool> UpdateTimerDuration(int userId, RawTimerDto timerDto);
    Task<bool> ResumeSingleTimer(int userId, long rawTimerId);
  }

  public class RawTimerService : IRawTimerService
  {
    private readonly IRawTimersRepo _rawTimersRepo;

    public RawTimerService(IRawTimersRepo rawTimersRepo)
    {
      _rawTimersRepo = rawTimersRepo;
    }

    public async Task<RawTimerDto> StartNew(int userId, RawTimerDto timerDto)
    {
      // TODO: [TESTS] (RawTimerService.StartNew) Add tests
      var entryEntity = timerDto.AsEntity();
      entryEntity.UserId = userId;

      if (await _rawTimersRepo.StartNew(entryEntity) <= 0)
      {
        // TODO: [HANDLE] (RawTimerService.StartNew) Handle this
        return null;
      }

      var dbEntry = await _rawTimersRepo.GetCurrentEntry(entryEntity);
      var rootId = dbEntry.RawTimerId;
      if (await _rawTimersRepo.SetRootTimerId(rootId, rootId) == 0)
      {
        // TODO: [HANDLE] (RawTimerService.StartNew) Handle this
        return null;
      }

      dbEntry.RootTimerId = rootId;
      return RawTimerDto.FromEntity(dbEntry);
    }

    public async Task<List<RawTimerDto>> GetActiveTimers(int userId)
    {
      // TODO: [TESTS] (RawTimerService.GetActiveTimers) Add tests
      var dbEntries = await _rawTimersRepo.GetActiveTimers(userId);
      return dbEntries.AsQueryable().Select(RawTimerDto.Projection).ToList();
    }

    public async Task<bool> PauseTimer(int userId, long rawTimerId, EntryRunningState state, string notes)
    {
      // TODO: [TESTS] (RawTimerService.PauseTimer) Add tests
      var dbEntry = await _rawTimersRepo.GetByRawTimerId(rawTimerId);
      if (dbEntry == null || dbEntry.UserId != userId)
      {
        // TODO: [HANDLE] (RawTimerService.PauseTimer) Handle this
        return false;
      }

      if (await _rawTimersRepo.PauseTimer(rawTimerId, state, notes) <= 0)
      {
        // TODO: [HANDLE] (RawTimerService.PauseTimer) Handle this
        return false;
      }

      return true;
    }

    public async Task<bool> ResumeTimer(int userId, long rawTimerId)
    {
      // TODO: [TESTS] (RawTimerService.ResumeTimer) Add tests

      var parentEntry = await _rawTimersRepo.GetByRawTimerId(rawTimerId);
      if (parentEntry == null || parentEntry.UserId != userId)
      {
        // TODO: [HANDLE] (RawTimerService.ResumeTimer) Handle this
        return false;
      }
      var resumedEntity = CreateResumedTimer(parentEntry);
      if (await _rawTimersRepo.SpawnResumedTimer(resumedEntity) == 0)
      {
        // TODO: [HANDLE] (RawTimerService.ResumeTimer) Handle this
        return false;
      }

      // ReSharper disable once ConvertIfStatementToReturnStatement
      if (await _rawTimersRepo.FlagAsResumed(rawTimerId) == 0)
      {
        // TODO: [HANDLE] (RawTimerService.ResumeTimer) Handle this
        return false;
      }

      return true;
    }

    public async Task<bool> StopTimer(int userId, long rawTimerId)
    {
      // TODO: [TESTS] (RawTimerService.StopTimer) Add tests
      var dbTimer = await _rawTimersRepo.GetByRawTimerId(rawTimerId);
      if (dbTimer == null || dbTimer.UserId != userId)
      {
        // TODO: [HANDLE] (RawTimerService.StopTimer) Handle this
        return false;
      }

      if (await _rawTimersRepo.StopTimer(rawTimerId) == 0)
      {
        // TODO: [HANDLE] (RawTimerService.StopTimer) Handle this
        return false;
      }

      // ReSharper disable once ConvertIfStatementToReturnStatement
      if (await _rawTimersRepo.CompleteTimerSet(dbTimer.RootTimerId) == 0)
      {
        // TODO: [HANDLE] (RawTimerService.StopTimer) Handle this
        return false;
      }

      return true;
    }

    public async Task<List<RawTimerDto>> GetTimerSeries(int userId, long rootTimerId)
    {
      // TODO: [TESTS] (RawTimerService.GetTimerSeries) Add tests
      var dbEntries = await _rawTimersRepo.GetTimerSeries(rootTimerId);
      if (dbEntries == null || dbEntries.Count == 0)
      {
        // TODO: [HANDLE] (RawTimerService.GetTimerSeries) Handle this
        return new List<RawTimerDto>();
      }

      if (dbEntries.First().UserId != userId)
      {
        // TODO: [HANDLE] (RawTimerService.GetTimerSeries) Handle this
        return new List<RawTimerDto>();
      }

      return dbEntries.AsQueryable().Select(RawTimerDto.Projection).ToList();
    }

    public async Task<bool> UpdateNotes(int userId, long rawTimerId, string notes)
    {
      // TODO: [TESTS] (RawTimerService.UpdateNotes) Add tests
      var dbEntry = await _rawTimersRepo.GetByRawTimerId(rawTimerId);

      if (dbEntry == null)
      {
        // TODO: [HANDLE] (RawTimerService.UpdateNotes) Handle this
        return false;
      }

      if (dbEntry.UserId != userId)
      {
        // TODO: [HANDLE] (RawTimerService.UpdateNotes) Handle this
        return false;
      }

      if (await _rawTimersRepo.UpdateNotes(rawTimerId, notes) == 0)
      {
        // TODO: [HANDLE] (RawTimerService.UpdateNotes) Handle this
        return false;
      }

      return true;
    }

    public async Task<bool> UpdateTimerDuration(int userId, RawTimerDto timerDto)
    {
      // TODO: [TESTS] (RawTimerService.UpdateTimerDuration) Add tests
      var dbTimer = await _rawTimersRepo.GetByRawTimerId(timerDto.RawTimerId);
      if (dbTimer == null)
      {
        // TODO: [HANDLE] (RawTimerService.UpdateTimerDuration) Handle this
        return false;
      }

      if (dbTimer.UserId != userId)
      {
        // TODO: [HANDLE] (RawTimerService.UpdateTimerDuration) Handle this
        return false;
      }

      var timerEntity = timerDto.AsEntity();
      // ReSharper disable once ConvertIfStatementToReturnStatement
      if (await _rawTimersRepo.UpdateTimerDuration(timerEntity) == 0)
      {
        // TODO: [HANDLE] (RawTimerService.UpdateTimerDuration) Handle this
        return false;
      }

      return true;
    }

    public async Task<bool> ResumeSingleTimer(int userId, long rawTimerId)
    {
      // TODO: [TESTS] (RawTimerService.ResumeSingleTimer) Add tests
      var parentTimer = await _rawTimersRepo.GetByRawTimerId(rawTimerId);
      if (parentTimer == null)
      {
        // TODO: [HANDLE] (RawTimerService.ResumeSingleTimer) Handle this
        return false;
      }

      if (parentTimer.UserId != userId)
      {
        // TODO: [HANDLE] (RawTimerService.ResumeSingleTimer) Handle this
        return false;
      }

      var runningTimers = await _rawTimersRepo.GetActiveTimers(userId);
      if (runningTimers.Count > 0)
      {
        foreach (var timer in runningTimers)
        {
          await _rawTimersRepo.PauseTimer(
            timer.RawTimerId,
            EntryRunningState.Paused,
            "user-paused (auto)"
          );
        }
      }

      var resumedTimer = CreateResumedTimer(parentTimer);
      // ReSharper disable once ConvertIfStatementToReturnStatement
      if (await _rawTimersRepo.SpawnResumedTimer(resumedTimer) == 0)
      {
        // TODO: [HANDLE] (RawTimerService.ResumeSingleTimer) Handle this
        return false;
      }

      return true;
    }

    // Internal methods
    private static RawTimerEntity CreateResumedTimer(RawTimerEntity parentTimer)
    {
      // TODO: [TESTS] (RawTimerService.CreateResumedTimer) Add tests
      return new RawTimerEntity
      {
        ParentTimerId = parentTimer.RawTimerId,
        RootTimerId = parentTimer.RootTimerId,
        ClientId = parentTimer.ClientId,
        ProductId = parentTimer.ProductId,
        ProjectId = parentTimer.ProjectId,
        UserId = parentTimer.UserId,
        Running = true,
        EntryState = EntryRunningState.Running,
        Completed = false,
        TimerNotes = "user-resumed"
      };
    }
  }
}
