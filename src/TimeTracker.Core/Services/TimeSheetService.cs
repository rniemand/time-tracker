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
    private readonly ITimeSheetDateRepo _dateRepo;
    private readonly ITimeSheetRowRepo _rowRepo;
    private readonly IDateTimeAbstraction _dateTime;

    public TimeSheetService(
      ILoggerAdapter<TimeSheetService> logger,
      ITimeSheetDateRepo dateRepo,
      IDateTimeAbstraction dateTime,
      ITimeSheetRowRepo rowRepo)
    {
      _logger = logger;
      _dateRepo = dateRepo;
      _dateTime = dateTime;
      _rowRepo = rowRepo;
    }


    // Interface methods
    public async Task<bool> EnsureTimeSheetDateExists(TimeSheetDateCache cache, ClientEntity client, DateTime date)
    {
      // TODO: [TESTS] (TimeSheetService.EnsureTimeSheetDateExists) Add tests
      var userId = client.UserId;
      var clientId = client.ClientId;

      var dbDate = cache.GetCachedEntry(client, date)
                   ?? await _dateRepo.GetEntry(userId, clientId, date);

      // If the entry exists, ensure it's cached and return it
      cache.CacheEntry(dbDate);
      if (dbDate != null)
        return true;

      // We need to create a new entry
      if (await _dateRepo.Add(CreateTimeSheetDate(client, date)) == 0)
        return false;

      dbDate = await _dateRepo.GetEntry(userId, clientId, date);
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

      response.Dates = (await _dateRepo.GetClientDates(clientId, from, to))
        .AsQueryable()
        .Select(TimeSheetDateDto.Projection)
        .ToList();

      response.Projects = (await _rowRepo.GetTimeSheetProjects(clientId, from, to))
        .AsQueryable()
        .Select(ProjectDto.Projection)
        .ToList();

      return response;
    }

    public async Task<GetTimeSheetResponse> AddTimeSheetRow(AddTimeSheetRowRequest request)
    {
      // TODO: [TESTS] (TimeSheetService.AddTimeSheetRow) Add tests
      var clientId = request.ClientId;
      var userId = request.UserId;
      var projectId = request.ProjectId;
      var fromDate = request.StartDate;
      var numDays = request.NumberDays;
      var toDate = fromDate.AddDays(numDays);

      // Ensure that all dates exist for the requested range
      // TODO: [EX] (TimeSheetService.AddTimeSheetRow) Throw better exception here
      var datesExist = await EnsureDatesExist(clientId, userId, fromDate, numDays);
      if (!datesExist)
        throw new Exception("Unable to created required date range");

      var dbRows = await _rowRepo.GetProjectRows(projectId, fromDate, toDate);
      var dbDates = await _dateRepo.GetClientDates(clientId, fromDate, toDate);

      for (var i = 0; i < numDays; i++)
      {
        var baseDate = fromDate.AddDays(i);
        var filterDate = new DateTime(baseDate.Year, baseDate.Month, baseDate.Day);
        var dbDate = dbDates.First(x => x.EntryDate == filterDate);
        var dbRow = dbRows.FirstOrDefault(x => x.DateId == dbDate.DateId);
        if (dbRow != null)
          continue;

        // TODO: [EX] (TimeSheetService.AddTimeSheetRow) Throw better exception
        var rowToAdd = CreateTimeSheetRow(request, dbDate.DateId);
        if (await _rowRepo.AddRow(rowToAdd) == 0)
          throw new Exception("Unable to create row entry");
      }

      return await GetTimeSheet(new GetTimeSheetRequest
      {
        ClientId = clientId,
        StartDate = fromDate,
        EndDate = toDate
      },
        userId
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
        var dbEntries = await _dateRepo.GetClientDates(clientId, startDate, endDate);

        for (var i = 0; i < length; i++)
        {
          var currentDate = startDate.AddDays(i);
          var dbCurrentDate = new DateTime(currentDate.Year, currentDate.Month, currentDate.Day);

          // Check to see if the requested date exists
          var dbEntry = dbEntries.FirstOrDefault(x => x.EntryDate == dbCurrentDate);
          if (dbEntry != null)
            continue;

          // Add a missing date entry
          var entryToAdd = CreateTimeSheetDate(clientId, userId, dbCurrentDate);
          if (await _dateRepo.Add(entryToAdd) == 0)
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
