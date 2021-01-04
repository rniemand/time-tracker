using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Razor.Language;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Rn.NetCore.Common.Logging;
using TimeTracker.Core.Database.Repos;
using TimeTracker.Core.Models.Dto;

namespace TimeTracker.Core.Services
{
  public interface ITrackedTimeService
  {
    Task<RawTrackedTimeDto> StartNew(int userId, RawTrackedTimeDto entryDto);
    Task<List<RawTrackedTimeDto>> GetRunningTimers(int userId);
    Task<RawTrackedTimeDto> PauseTimer(int userId, long entryId);
    Task<RawTrackedTimeDto> ResumeTimer(int userId, long entryId);
  }

  public class TrackedTimeService : ITrackedTimeService
  {
    private readonly ILoggerAdapter<TrackedTimeService> _logger;
    private readonly IRawTrackedTimeRepo _rawTrackedTimeRepo;

    public TrackedTimeService(
      ILoggerAdapter<TrackedTimeService> logger,
      IRawTrackedTimeRepo rawTrackedTimeRepo)
    {
      _logger = logger;
      _rawTrackedTimeRepo = rawTrackedTimeRepo;
    }

    public async Task<RawTrackedTimeDto> StartNew(int userId, RawTrackedTimeDto entryDto)
    {
      // TODO: [TESTS] (TrackedTimeService.StartNew) Add tests
      var entryEntity = entryDto.AsEntity();
      entryEntity.UserId = userId;

      if (await _rawTrackedTimeRepo.StartNew(entryEntity) <= 0)
      {
        // TODO: [HANDLE] (TrackedTimeService.StartNew) Handle this
        return null;
      }

      var dbEntry = await _rawTrackedTimeRepo.GetCurrentEntry(entryEntity);
      var rootId = dbEntry.EntryId;
      if (await _rawTrackedTimeRepo.SetRootParentEntryId(rootId, rootId) == 0)
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

      var dbEntries = await _rawTrackedTimeRepo.GetRunningTimers(userId);
      return dbEntries.AsQueryable().Select(RawTrackedTimeDto.Projection).ToList();
    }

    public async Task<RawTrackedTimeDto> PauseTimer(int userId, long entryId)
    {
      // TODO: [TESTS] (TrackedTimeService.PauseTimer) Add tests

      var dbEntry = await _rawTrackedTimeRepo.GetByEntryId(entryId);
      if (dbEntry == null || dbEntry.UserId != userId)
      {
        // TODO: [HANDLE] (TrackedTimeService.PauseTimer) Handle this
        return null;
      }

      if (await _rawTrackedTimeRepo.PauseTimer(entryId) <= 0)
      {
        // TODO: [HANDLE] (TrackedTimeService.PauseTimer) Handle this
        return null;
      }

      dbEntry = await _rawTrackedTimeRepo.GetByEntryId(entryId);
      return RawTrackedTimeDto.FromEntity(dbEntry);
    }

    public async Task<RawTrackedTimeDto> ResumeTimer(int userId, long entryId)
    {
      // TODO: [TESTS] (TrackedTimeService.ResumeTimer) Add tests

      var parentEntry = await _rawTrackedTimeRepo.GetByEntryId(entryId);
      if (parentEntry == null || parentEntry.UserId != userId)
      {
        // TODO: [HANDLE] (TrackedTimeService.ResumeTimer) Handle this
        return null;
      }




      /*
       *  return @"UPDATE `RawTrackedTime`
      SET
	      `RootParentEntryId` = @RootParentEntryId,
	      `Running` = @Running
      WHERE
	      `EntryId` = @EntryId";
       */


      return null;
    }
  }
}
