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
    Task<GetTimeSheetResponse> GetTimeSheet(GetTimeSheetRequest request, int userId);
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

      response.Projects = (await _entriesRepo.GetReferencedProjects(clientId, from, to))
        .AsQueryable()
        .Select(ProjectDto.Projection)
        .ToList();

      response.Products = (await _entriesRepo.GetReferencedProducts(clientId, from, to))
        .AsQueryable()
        .Select(ProductDto.Projection)
        .ToList();

      return response;
    }

    public async Task<GetTimeSheetResponse> UpdateEntry(AddTimeSheetEntryRequest request)
    {
      // TODO: [TESTS] (TimeSheetService.UpdateEntry) Add tests

      var dbProject = await _projectRepo.GetById(request.ProjectId);
      var entryDate = new DateTime(request.EntryDate.Year, request.EntryDate.Month, request.EntryDate.Day);

      var entry = new TimeSheetEntry
      {
        UserId = dbProject.UserId,
        ClientId = dbProject.ClientId,
        Deleted = false,
        ProductId = dbProject.ProductId,
        EntryDate = entryDate,
        EntryVersion = 1,
        ProjectId = dbProject.ProjectId,
        DateAddedUtc = _dateTime.UtcNow,
        DateDeletedUtc = null,
        DateUpdatedUtc = null,
        EntryTimeMin = request.LoggedTimeMin
      };


      return null;
    }
  }
}
