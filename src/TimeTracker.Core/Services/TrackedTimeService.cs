using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Rn.NetCore.Common.Logging;
using TimeTracker.Core.Database.Repos;
using TimeTracker.Core.Models.Dto;

namespace TimeTracker.Core.Services
{
  public interface ITrackedTimeService
  {
    Task<RawTrackedTimeDto> StartNew(int userId, RawTrackedTimeDto entryDto);
    Task<List<RawTrackedTimeDto>> GetRunningTimers(int userId);
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

      entryDto.AsEntity().UserId = userId;

      if (await _rawTrackedTimeRepo.StartNew(entryDto.AsEntity()) <= 0)
      {
        // TODO: [HANDLE] (TrackedTimeService.StartNew) Handle this
        return null;
      }

      return RawTrackedTimeDto.FromEntity(
        await _rawTrackedTimeRepo.GetCurrentEntry(entryDto.AsEntity())
      );
    }

    public async Task<List<RawTrackedTimeDto>> GetRunningTimers(int userId)
    {
      // TODO: [TESTS] (TrackedTimeService.GetRunningTimers) Add tests

      var dbEntries = await _rawTrackedTimeRepo.GetRunningTimers(userId);
      return dbEntries.AsQueryable().Select(RawTrackedTimeDto.Projection).ToList();
    }
  }
}
