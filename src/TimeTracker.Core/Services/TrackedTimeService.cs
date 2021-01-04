using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Rn.NetCore.Common.Logging;
using TimeTracker.Core.Database.Entities;
using TimeTracker.Core.Database.Repos;
using TimeTracker.Core.Enums;
using TimeTracker.Core.Models.Dto;

namespace TimeTracker.Core.Services
{
  public interface ITrackedTimeService
  {
    Task<RawTrackedTimeDto> StartNew(int userId, RawTrackedTimeDto entryDto);
    Task<List<RawTrackedTimeDto>> GetRunningTimers(int userId);
    Task<bool> PauseTimer(int userId, long entryId);
    Task<bool> ResumeTimer(int userId, long entryId);
  }

  public class TrackedTimeService : ITrackedTimeService
  {
    private readonly ILoggerAdapter<TrackedTimeService> _logger;
    private readonly IRawTimersRepo _rawTimersRepo;

    public TrackedTimeService(
      ILoggerAdapter<TrackedTimeService> logger,
      IRawTimersRepo rawTimersRepo)
    {
      _logger = logger;
      _rawTimersRepo = rawTimersRepo;
    }

    public async Task<RawTrackedTimeDto> StartNew(int userId, RawTrackedTimeDto entryDto)
    {
      // TODO: [TESTS] (TrackedTimeService.StartNew) Add tests
      var entryEntity = entryDto.AsEntity();
      entryEntity.UserId = userId;

      if (await _rawTimersRepo.StartNew(entryEntity) <= 0)
      {
        // TODO: [HANDLE] (TrackedTimeService.StartNew) Handle this
        return null;
      }

      var dbEntry = await _rawTimersRepo.GetCurrentEntry(entryEntity);
      var rootId = dbEntry.EntryId;
      if (await _rawTimersRepo.SetRootParentEntryId(rootId, rootId) == 0)
      {
        // TODO: [HANDLE] (TrackedTimeService.StartNew) Handle this
        return null;
      }

      dbEntry.RootParentEntryId = rootId;
      return RawTrackedTimeDto.FromEntity(dbEntry);
    }

    public async Task<List<RawTrackedTimeDto>> GetRunningTimers(int userId)
    {
      // TODO: [TESTS] (TrackedTimeService.GetRunningTimers) Add tests

      var dbEntries = await _rawTimersRepo.GetRunningTimers(userId);
      return dbEntries.AsQueryable().Select(RawTrackedTimeDto.Projection).ToList();
    }

    public async Task<bool> PauseTimer(int userId, long entryId)
    {
      // TODO: [TESTS] (TrackedTimeService.PauseTimer) Add tests

      var dbEntry = await _rawTimersRepo.GetByEntryId(entryId);
      if (dbEntry == null || dbEntry.UserId != userId)
      {
        // TODO: [HANDLE] (TrackedTimeService.PauseTimer) Handle this
        return false;
      }

      if (await _rawTimersRepo.PauseTimer(entryId) <= 0)
      {
        // TODO: [HANDLE] (TrackedTimeService.PauseTimer) Handle this
        return false;
      }

      return true;
    }

    public async Task<bool> ResumeTimer(int userId, long entryId)
    {
      // TODO: [TESTS] (TrackedTimeService.ResumeTimer) Add tests

      var parentEntry = await _rawTimersRepo.GetByEntryId(entryId);
      if (parentEntry == null || parentEntry.UserId != userId)
      {
        // TODO: [HANDLE] (TrackedTimeService.ResumeTimer) Handle this
        return false;
      }

      var resumedEntity = new RawTrackedTimeEntity
      {
        ParentEntryId = parentEntry.EntryId,
        RootParentEntryId = parentEntry.RootParentEntryId,
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
        // TODO: [HANDLE] (TrackedTimeService.ResumeTimer) Handle this
        return false;
      }

      if (await _rawTimersRepo.FlagAsResumed(entryId) == 0)
      {
        // TODO: [HANDLE] (TrackedTimeService.ResumeTimer) Handle this
        return false;
      }

      return true;
    }
  }
}
