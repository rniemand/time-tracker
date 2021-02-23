using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Rn.NetCore.Common.Logging;
using Rn.NetCore.Common.Metrics;
using Rn.NetCore.DbCommon.Helpers;
using Rn.NetCore.DbCommon.Repos;
using TimeTracker.Core.Database.Entities;
using TimeTracker.Core.Database.Queries;
using TimeTracker.Core.Extensions;

namespace TimeTracker.Core.Database.Repos
{
  public interface ITimeSheetEntryRepo
  {
    Task<List<ProjectEntity>> GetReferencedProjects(int clientId, DateTime fromDate, DateTime toDate);
    Task<List<ProductEntity>> GetReferencedProducts(int clientId, DateTime fromDate, DateTime toDate);
    Task<TimeSheetEntry> GetProjectTimeSheetEntry(int projectId, DateTime entryDate);
    Task<int> AddEntry(TimeSheetEntry entry);
    Task<int> UpdateEntry(TimeSheetEntry entry);
    Task<List<TimeSheetEntry>> GetEntries(int clientId, DateTime fromDate, DateTime toDate);
  }

  public class TimeSheetEntryRepo : BaseRepo<TimeSheetEntryRepo>, ITimeSheetEntryRepo
  {
    private readonly ITimeSheetEntryRepoQueries _queries;

    public TimeSheetEntryRepo(
      ILoggerAdapter<TimeSheetEntryRepo> logger,
      IDbHelper dbHelper,
      IMetricService metrics,
      ITimeSheetEntryRepoQueries queries)
      : base(logger, dbHelper, metrics, nameof(TimeSheetEntryRepo), TargetDB.TimeTracker)
    {
      _queries = queries;
    }


    // Interface methods
    public async Task<List<ProjectEntity>> GetReferencedProjects(int clientId, DateTime fromDate, DateTime toDate)
    {
      // TODO: [TESTS] (TimeSheetEntryRepo.GetReferencedProjects) Add tests
      return await GetList<ProjectEntity>(
        nameof(GetReferencedProjects),
        _queries.GetReferencedProjects(),
        new
        {
          ClientId = clientId,
          FromDate = fromDate.ToShortDbDate(),
          ToDate = toDate.ToShortDbDate()
        }
      );
    }

    public async Task<List<ProductEntity>> GetReferencedProducts(int clientId, DateTime fromDate, DateTime toDate)
    {
      // TODO: [TESTS] (TimeSheetEntryRepo.GetReferencedProducts) Add tests
      return await GetList<ProductEntity>(
        nameof(GetReferencedProducts),
        _queries.GetReferencedProducts(),
        new
        {
          ClientId = clientId,
          FromDate = fromDate.ToShortDbDate(),
          ToDate = toDate.ToShortDbDate()
        }
      );
    }

    public async Task<TimeSheetEntry> GetProjectTimeSheetEntry(int projectId, DateTime entryDate)
    {
      // TODO: [TESTS] (TimeSheetEntryRepo.GetProjectTimeSheetEntry) Add tests
      return await GetSingle<TimeSheetEntry>(
        nameof(GetProjectTimeSheetEntry),
        _queries.GetProjectTimeSheetEntry(),
        new
        {
          ProjectId = projectId,
          EntryDate = entryDate.ToShortDbDate()
        }
      );
    }

    public async Task<int> AddEntry(TimeSheetEntry entry)
    {
      // TODO: [TESTS] (TimeSheetEntryRepo.AddEntry) Add tests
      return await ExecuteAsync(
        nameof(AddEntry),
        _queries.AddEntry(),
        entry
      );
    }

    public async Task<int> UpdateEntry(TimeSheetEntry entry)
    {
      // TODO: [TESTS] (TimeSheetEntryRepo.UpdateEntry) Add tests
      return await ExecuteAsync(
        nameof(UpdateEntry),
        _queries.UpdateEntry(),
        entry
      );
    }

    public async Task<List<TimeSheetEntry>> GetEntries(int clientId, DateTime fromDate, DateTime toDate)
    {
      // TODO: [TESTS] (TimeSheetEntryRepo.GetEntries) Add tests
      return await GetList<TimeSheetEntry>(
        nameof(GetEntries),
        _queries.GetEntries(),
        new
        {
          ClientId = clientId,
          FromDate = fromDate.ToShortDbDate(),
          ToDate = toDate.ToShortDbDate()
        }
      );
    }
  }
}
