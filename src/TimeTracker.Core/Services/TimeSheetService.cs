using System;
using System.Linq;
using System.Threading.Tasks;
using Rn.NetCore.Common.Abstractions;
using Rn.NetCore.Common.Logging;
using TimeTracker.Core.Database.Entities;
using TimeTracker.Core.Database.Repos;
using TimeTracker.Core.Models.Dto;
using TimeTracker.Core.Models.Requests;
using TimeTracker.Core.Models.Responses;

namespace TimeTracker.Core.Services
{
  public interface ITimeSheetService
  {
    Task<GetTimeSheetResponse> GetTimeSheet(GetTimeSheetRequest request);
    Task<GetTimeSheetResponse> UpdateEntry(AddTimeSheetEntryRequest request);
  }

  public class TimeSheetService : ITimeSheetService
  {
    private readonly ILoggerAdapter<TimeSheetService> _logger;
    private readonly IProjectRepo _projectRepo;
    private readonly ITimeSheetEntryRepo _entriesRepo;
    private readonly IDateTimeAbstraction _dateTime;

    public TimeSheetService(
      ILoggerAdapter<TimeSheetService> logger,
      IDateTimeAbstraction dateTime,
      IProjectRepo projectRepo,
      ITimeSheetEntryRepo entriesRepo)
    {
      _logger = logger;
      _dateTime = dateTime;
      _projectRepo = projectRepo;
      _entriesRepo = entriesRepo;
    }


    // Interface methods
    public async Task<GetTimeSheetResponse> GetTimeSheet(GetTimeSheetRequest request)
    {
      // TODO: [TESTS] (TimeSheetService.GetTimeSheet) Add tests
      // ReSharper disable once UseObjectOrCollectionInitializer
      var response = new GetTimeSheetResponse();

      var clientId = request.ClientId;
      var from = request.StartDate;
      var to = request.EndDate;

      response.StartDate = from;
      response.EndDate = to;
      response.DayCount = (to - from).Days;

      response.Projects = (await _entriesRepo.GetReferencedProjects(clientId, from, to))
        .AsQueryable()
        .Select(ProjectDto.Projection)
        .ToList();

      response.Products = (await _entriesRepo.GetReferencedProducts(clientId, from, to))
        .AsQueryable()
        .Select(ProductDto.Projection)
        .ToList();

      response.Entries = (await _entriesRepo.GetEntries(request.ClientId, from, to))
        .AsQueryable()
        .Select(TimeSheetEntryDto.Projection)
        .ToList();

      return response;
    }

    public async Task<GetTimeSheetResponse> UpdateEntry(AddTimeSheetEntryRequest request)
    {
      // TODO: [TESTS] (TimeSheetService.UpdateEntry) Add tests
      var loggedTime = request.LoggedTimeMin;
      var dbProject = await _projectRepo.GetById(request.ProjectId);
      var entryDate = new DateTime(request.EntryDate.Year, request.EntryDate.Month, request.EntryDate.Day);
      var dbEntry = await _entriesRepo.GetProjectTimeSheetEntry(request.ProjectId, request.EntryDate);

      if (dbEntry == null)
      {
        // TODO: [EX] (TimeSheetService.UpdateEntry) Throw better exception here
        if (await _entriesRepo.AddEntry(CreateTimeSheetEntry(dbProject, entryDate, loggedTime)) == 0)
          throw new Exception("Unable to create entry");
      }
      else
      {
        dbEntry.EntryVersion += 1;
        dbEntry.EntryTimeMin = loggedTime;
        // TODO: [EX] (TimeSheetService.UpdateEntry) Throw better exception
        if (await _entriesRepo.UpdateEntry(dbEntry) == 0)
          throw new Exception("Unable to update entry");
      }

      return await GetTimeSheet(new GetTimeSheetRequest
      {
        ClientId = dbProject.ClientId,
        StartDate = request.StartDate,
        EndDate = request.EndDate
      });
    }


    // Internal methods
    private TimeSheetEntry CreateTimeSheetEntry(ProjectEntity project, DateTime entryDate, int loggedMin = 0)
    {
      // TODO: [TESTS] (TimeSheetService.CreateTimeSheetEntry) Add tests
      return new TimeSheetEntry
      {
        Deleted = false,
        EntryVersion = 1,
        DateDeletedUtc = null,
        DateUpdatedUtc = null,
        UserId = project.UserId,
        ClientId = project.ClientId,
        ProductId = project.ProductId,
        ProjectId = project.ProjectId,
        EntryDate = entryDate,
        DateAddedUtc = _dateTime.UtcNow,
        EntryTimeMin = loggedMin
      };
    }
  }
}
