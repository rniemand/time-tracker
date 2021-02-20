using System;
using System.Linq;
using System.Threading.Tasks;
using Rn.NetCore.Common.Abstractions;
using Rn.NetCore.Common.Logging;
using TimeTracker.Core.Caches;
using TimeTracker.Core.Database.Entities;
using TimeTracker.Core.Database.Repos;
using TimeTracker.Core.Models.Dto;
using TimeTracker.Core.Models.Requests;
using TimeTracker.Core.Models.Responses;

namespace TimeTracker.Core.Services
{
  public interface ITimeSheetService
  {
    Task<bool> EnsureTimeSheetDateExists(TimeSheetDateCache cache, ClientEntity client, DateTime date);
    Task<GetTimeSheetResponse> GetTimeSheet(GetTimeSheetRequest request, int userId);
  }

  public class TimeSheetService : ITimeSheetService
  {
    private readonly ILoggerAdapter<TimeSheetService> _logger;
    private readonly ITimeSheetDateRepo _timeSheetDateRepo;
    private readonly IDateTimeAbstraction _dateTime;

    public TimeSheetService(
      ILoggerAdapter<TimeSheetService> logger,
      ITimeSheetDateRepo timeSheetDateRepo,
      IDateTimeAbstraction dateTime)
    {
      _logger = logger;
      _timeSheetDateRepo = timeSheetDateRepo;
      _dateTime = dateTime;
    }


    // Interface methods
    public async Task<bool> EnsureTimeSheetDateExists(TimeSheetDateCache cache, ClientEntity client, DateTime date)
    {
      // TODO: [TESTS] (TimeSheetService.EnsureTimeSheetDateExists) Add tests
      var userId = client.UserId;
      var clientId = client.ClientId;

      var dbDate = cache.GetCachedEntry(client, date)
                   ?? await _timeSheetDateRepo.GetEntry(userId, clientId, date);

      // If the entry exists, ensure it's cached and return it
      cache.CacheEntry(dbDate);
      if (dbDate != null)
        return true;

      // We need to create a new entry
      if (await _timeSheetDateRepo.Add(CreateTimeSheetDate(client, date)) == 0)
        return false;

      dbDate = await _timeSheetDateRepo.GetEntry(userId, clientId, date);
      cache.CacheEntry(dbDate);
      return true;
    }

    public async Task<GetTimeSheetResponse> GetTimeSheet(GetTimeSheetRequest request, int userId)
    {
      // TODO: [TESTS] (TimeSheetService.GetTimeSheet) Add tests
      // ReSharper disable once UseObjectOrCollectionInitializer
      var response = new GetTimeSheetResponse();

      var clientId = request.ClientId;
      var from = request.StartDate;
      var to = request.EndDate;

      request.StartDate = from;
      request.EndDate = to;

      response.Dates = (await _timeSheetDateRepo.GetClientDatesForRange(clientId, from, to))
        .AsQueryable()
        .Select(TimeSheetDateDto.Projection)
        .ToList();

      return response;
    }


    // Internal methods
    private TimeSheetDate CreateTimeSheetDate(ClientEntity client, DateTime date)
    {
      // TODO: [TESTS] (TimeSheetService.CreateTimeSheetDate) Add tests
      return new TimeSheetDate
      {
        UserId = client.UserId,
        ClientId = client.ClientId,
        Deleted = false,
        DateAddedUtc = _dateTime.UtcNow,
        DateDeletedUtc = null,
        DateUpdatedUtc = null,
        EntryDate = date,
        DayOfWeek = date.DayOfWeek
      };
    }
  }
}
