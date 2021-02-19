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
  public interface ITimeSheetDateRepo
  {
    Task<List<TimeSheetDate>> GetDatesForRange(int userId, DateTime from, DateTime to);
  }

  public class TimeSheetDateRepo : BaseRepo<TimeSheetDateRepo>, ITimeSheetDateRepo
  {
    private readonly ITimeSheetDateRepoQueries _queries;

    public TimeSheetDateRepo(
      ILoggerAdapter<TimeSheetDateRepo> logger,
      IDbHelper dbHelper,
      IMetricService metrics,
      ITimeSheetDateRepoQueries queries)
        : base(logger, dbHelper, metrics, nameof(TimeSheetDateRepo), TargetDB.TimeTracker)
    {
      _queries = queries;
    }


    // Interface methods
    public async Task<List<TimeSheetDate>> GetDatesForRange(int userId, DateTime @from, DateTime to)
    {
      // TODO: [TESTS] (TimeSheetDateRepo.GetDatesForRange) Add tests
      return await GetList<TimeSheetDate>(
        nameof(GetDatesForRange),
        _queries.GetDatesForRange(),
        new
        {
          UserId = userId,
          StartDate = from,
          EndDate = to
        }
      );
    }
  }
}
