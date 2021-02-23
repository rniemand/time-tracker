using System;
using System.Collections.Generic;
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
    private readonly ITimeSheetRowRepo _timeSheetRowRepo;
    private readonly IDateTimeAbstraction _dateTime;

    public TimeSheetService(
      ILoggerAdapter<TimeSheetService> logger,
      ITimeSheetDateRepo timeSheetDateRepo,
      IDateTimeAbstraction dateTime,
      ITimeSheetRowRepo timeSheetRowRepo)
    {
      _logger = logger;
      _timeSheetDateRepo = timeSheetDateRepo;
      _dateTime = dateTime;
      _timeSheetRowRepo = timeSheetRowRepo;
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

      response.Dates = (await _timeSheetDateRepo.GetClientDates(clientId, from, to))
        .AsQueryable()
        .Select(TimeSheetDateDto.Projection)
        .ToList();

      response.Rows = (await _timeSheetRowRepo.GetClientRows(clientId, from, to))
        .AsQueryable()
        .Select(TimeSheetRowDto.Projection)
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

      var dbRows = await _timeSheetRowRepo.GetProjectRows(
        request.ProductId,
        request.StartDate,
        request.StartDate.AddDays(request.NumberDays)
      );

      var dbDates = await _timeSheetDateRepo.GetClientDates(
        request.ClientId,
        request.StartDate,
        request.StartDate.AddDays(request.NumberDays)
      );

      for (var i = 0; i < request.NumberDays; i++)
      {
        var baseDate = request.StartDate.AddDays(i);
        var dbFilterDate = new DateTime(baseDate.Year, baseDate.Month, baseDate.Day);
        var dbDate = dbDates.First(x => x.EntryDate == dbFilterDate);
        var dbRow = dbRows.FirstOrDefault(x => x.DateId == dbDate.DateId);
        if (dbRow != null)
          continue;

        // TODO: [EX] (TimeSheetService.AddTimeSheetRow) Throw better exception
        var rowToAdd = CreateTimeSheetRow(request, dbDate.DateId);
        if (await _timeSheetRowRepo.AddRow(rowToAdd) == 0)
          throw new Exception("Unable to create row entry");
      }

      return await GetTimeSheet(new GetTimeSheetRequest
        {
          ClientId = request.ClientId,
          StartDate = request.StartDate,
          EndDate = request.StartDate.AddDays(request.NumberDays)
        },
        request.UserId
      );
    }


    // Internal methods
    private TimeSheetRow CreateTimeSheetRow(AddTimeSheetRowRequest request, int dateId)
    {
      // TODO: [TESTS] (TimeSheetService.CreateTimeSheetRow) Add tests
      return new()
      {
        ClientId = request.ClientId,
        DateId = dateId,
        ProductId = request.ProductId,
        DateAddedUtc = _dateTime.UtcNow,
        Deleted = false,
        ProjectId = request.ProjectId,
        UserId = request.UserId
      };
    }

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
        var dbEntries = await _timeSheetDateRepo.GetClientDates(clientId, startDate, endDate);

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
