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
  public interface ITimeSheetRowRepo
  {
    Task<TimeSheetRow> GetRow(TimeSheetRow row);
    Task<int> AddRow(TimeSheetRow row);
    Task<List<TimeSheetRow>> GetProjectRows(int projectId, DateTime from, DateTime to);
    Task<List<TimeSheetRow>> GetClientRows(int clientId, DateTime from, DateTime to);
    Task<List<ProjectEntity>> GetTimeSheetProjects(int clientId, DateTime from, DateTime to);
    Task<List<ProductEntity>> GetTimeSheetProducts(int clientId, DateTime from, DateTime to);
  }

  public class TimeSheetRowRepo : BaseRepo<TimeSheetRowRepo>, ITimeSheetRowRepo
  {
    private readonly ITimeSheetRowRepoQueries _queries;

    public TimeSheetRowRepo(
      ILoggerAdapter<TimeSheetRowRepo> logger,
      IDbHelper dbHelper,
      IMetricService metrics,
      ITimeSheetRowRepoQueries queries)
      : base(logger, dbHelper, metrics, nameof(TimeSheetRowRepo), TargetDB.TimeTracker)
    {
      _queries = queries;
    }


    // Interface methods
    public async Task<TimeSheetRow> GetRow(TimeSheetRow row)
    {
      // TODO: [TESTS] (TimeSheetRowRepo.GetRow) Add tests
      return await GetSingle<TimeSheetRow>(nameof(GetRow), _queries.GetRow(), row);
    }

    public async Task<int> AddRow(TimeSheetRow row)
    {
      // TODO: [TESTS] (TimeSheetRowRepo.AddRow) Add tests
      return await ExecuteAsync(
        nameof(AddRow),
        _queries.AddRow(),
        row
      );
    }

    public async Task<List<TimeSheetRow>> GetProjectRows(int projectId, DateTime @from, DateTime to)
    {
      // TODO: [TESTS] (TimeSheetRowRepo.GetProjectRows) Add tests
      return await GetList<TimeSheetRow>(
        nameof(GetProjectRows),
        _queries.GetProjectRows(),
        new
        {
          ProjectId = projectId,
          FromDate = from.ToShortDbDate(),
          ToDate = to.ToShortDbDate()
        }
      );
    }

    public async Task<List<TimeSheetRow>> GetClientRows(int clientId, DateTime @from, DateTime to)
    {
      // TODO: [TESTS] (TimeSheetRowRepo.GetClientRows) Add tests
      return await GetList<TimeSheetRow>(
        nameof(GetClientRows),
        _queries.GetClientRows(),
        new
        {
          ClientId = clientId,
          FromDate = from.ToShortDbDate(),
          ToDate = to.ToShortDbDate()
        }
      );
    }

    public async Task<List<ProjectEntity>> GetTimeSheetProjects(int clientId, DateTime @from, DateTime to)
    {
      // TODO: [TESTS] (TimeSheetRowRepo.GetTimeSheetProjects) Add tests
      return await GetList<ProjectEntity>(
        nameof(GetTimeSheetProjects),
        _queries.GetTimeSheetProjects(),
        new
        {
          ClientId = clientId,
          FromDate = from.ToShortDbDate(),
          ToDate = to.ToShortDbDate()
        }
      );
    }

    public async Task<List<ProductEntity>> GetTimeSheetProducts(int clientId, DateTime @from, DateTime to)
    {
      // TODO: [TESTS] (TimeSheetRowRepo.GetTimeSheetProducts) Add tests
      return await GetList<ProductEntity>(
        nameof(GetTimeSheetProducts),
        _queries.GetTimeSheetProducts(),
        new
        {
          ClientId = clientId,
          FromDate = from.ToShortDbDate(),
          ToDate = to.ToShortDbDate()
        }
      );
    }
  }
}
