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
  }
}
