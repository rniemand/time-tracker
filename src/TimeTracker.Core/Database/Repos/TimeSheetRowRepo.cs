using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Rn.NetCore.Common.Logging;
using Rn.NetCore.Common.Metrics;
using Rn.NetCore.DbCommon.Helpers;
using Rn.NetCore.DbCommon.Repos;
using TimeTracker.Core.Database.Entities;
using TimeTracker.Core.Database.Queries;

namespace TimeTracker.Core.Database.Repos
{
  public interface ITimeSheetRowRepo
  {
    Task<TimeSheetRow> GetRow(TimeSheetRow row);
    Task<int> AddRow(TimeSheetRow row);
    Task<List<TimeSheetRow>> GetRowsForRange(int projectId, DateTime from, DateTime to);
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
      return await ExecuteAsync(nameof(AddRow), _queries.AddRow(), row);
    }

    public async Task<List<TimeSheetRow>> GetRowsForRange(int projectId, DateTime @from, DateTime to)
    {
      // TODO: [TESTS] (TimeSheetRowRepo.GetRowsForRange) Add tests
      return await GetList<TimeSheetRow>(
        nameof(GetRowsForRange),
        _queries.GetRowsForRange(),
        new
        {

        }
      );
    }
  }
}
