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
    Task<List<RawTimerDto>> GetRunningTimers(int userId);
    Task<bool> PauseTimer(int userId, long rawTimerId);
    Task<bool> ResumeTimer(int userId, long rawTimerId);
    Task<bool> StopTimer(int userId, long rawTimerId);
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

    public async Task<List<RawTimerDto>> GetRunningTimers(int userId)
    {
      // TODO: [TESTS] (RawTimerService.GetRunningTimers) Add tests

      var dbEntries = await _rawTimersRepo.GetRunningTimers(userId);
      return dbEntries.AsQueryable().Select(RawTimerDto.Projection).ToList();
    }

    public async Task<bool> PauseTimer(int userId, long rawTimerId)
    {
      // TODO: [TESTS] (RawTimerService.PauseTimer) Add tests

      var dbEntry = await _rawTimersRepo.GetByRawTimerId(rawTimerId);
      if (dbEntry == null || dbEntry.UserId != userId)
      {
        // TODO: [HANDLE] (RawTimerService.PauseTimer) Handle this
        return false;
      }

      if (await _rawTimersRepo.PauseTimer(rawTimerId) <= 0)
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

      var resumedEntity = new RawTimerEntity
      {
        ParentTimerId = parentEntry.RawTimerId,
        RootTimerId = parentEntry.RootTimerId,
        ClientId = parentEntry.ClientId,
        ProductId = parentEntry.ProductId,
        ProjectId = parentEntry.ProjectId,
        UserId = parentEntry.UserId,
        Running = true,
        EntryState = EntryRunningState.Running,
        Completed = false
      };

      if (await _rawTimersRepo.SpawnResumedTimer(resumedEntity) == 0)
      {
        // TODO: [HANDLE] (RawTimerService.ResumeTimer) Handle this
        return false;
      }

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
  }
}
