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
    Task<GetTimeSheetResponse> AddTimeSheetRow(AddTimeSheetRowRequest request);
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

    public async Task<GetTimeSheetResponse> AddTimeSheetRow(AddTimeSheetRowRequest request)
    {
      // TODO: [TESTS] (TimeSheetService.AddTimeSheetRow) Add tests

      var datesExist = await EnsureDatesExist(request.ClientId,
        request.UserId,
        request.StartDate,
        request.NumberDays
      );

      // TODO: [EX] (TimeSheetService.AddTimeSheetRow) Throw better exception here
      if (!datesExist)
        throw new Exception("Unable to created required date range");

      var dates = await _timeSheetDateRepo.GetClientDatesForRange(
        request.ClientId,
        request.StartDate, 
        request.StartDate.AddDays(request.NumberDays)
      );


      return null;
    }


    // Internal methods
    private TimeSheetDate CreateTimeSheetDate(int clientId, int userId, DateTime date)
    {
      // TODO: [TESTS] (TimeSheetService.CreateTimeSheetDate) Add tests
      return new()
      {
        UserId = userId,
        ClientId = clientId,
        Deleted = false,
        DateAddedUtc = _dateTime.UtcNow,
        DateDeletedUtc = null,
        DateUpdatedUtc = null,
        EntryDate = date,
        DayOfWeek = date.DayOfWeek
      };
    }

    private TimeSheetDate CreateTimeSheetDate(ClientEntity client, DateTime date)
      => CreateTimeSheetDate(client.ClientId, client.UserId, date);

    private async Task<bool> EnsureDatesExist(int clientId, int userId, DateTime startDate, int length)
    {
      // TODO: [TESTS] (TimeSheetService.EnsureDatesExist) Add tests

      try
      {
        var endDate = startDate.AddDays(length);
        var dbEntries = await _timeSheetDateRepo.GetClientDatesForRange(clientId, startDate, endDate);

        for (var i = 0; i < length; i++)
        {
          var currentDate = startDate.AddDays(i);
          var dbCurrentDate = new DateTime(currentDate.Year, currentDate.Month, currentDate.Day);

          // Check to see if the requested date exists
          var dbEntry = dbEntries.FirstOrDefault(x => x.EntryDate == dbCurrentDate);
          if(dbEntry != null)
            continue;

          // Add a missing date entry
          var entryToAdd = CreateTimeSheetDate(clientId, userId, dbCurrentDate);
          if(await _timeSheetDateRepo.Add(entryToAdd) == 0)
            return false;
        }

        return true;
      }
      catch (Exception ex)
      {
        _logger.LogUnexpectedException(ex);
        return false;
      }
    }
  }
}
