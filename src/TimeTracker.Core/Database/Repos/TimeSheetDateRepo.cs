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
  public interface ITimeSheetDateRepo
  {
    Task<List<TimeSheetDate>> GetUserDates(int userId, DateTime from, DateTime to);
    Task<List<TimeSheetDate>> GetClientDates(int clientId, DateTime from, DateTime to);
    Task<TimeSheetDate> GetEntry(int userId, int clientId, DateTime date);
    Task<int> Add(TimeSheetDate timeSheetDate);
    Task<TimeSheetDate> GetById(int dateId);
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
    public async Task<List<TimeSheetDate>> GetUserDates(int userId, DateTime @from, DateTime to)
    {
      // TODO: [TESTS] (TimeSheetDateRepo.GetUserDates) Add tests
      return await GetList<TimeSheetDate>(
        nameof(GetUserDates),
        _queries.GetUserDates(),
        new
        {
          UserId = userId,
          StartDate = from.ToShortDbDate(),
          EndDate = to.ToShortDbDate()
        }
      );
    }

    public async Task<List<TimeSheetDate>> GetClientDates(int clientId, DateTime @from, DateTime to)
    {
      // TODO: [TESTS] (TimeSheetDateRepo.GetClientDates) Add tests
      return await GetList<TimeSheetDate>(
        nameof(GetClientDates),
        _queries.GetClientDates(),
        new
        {
          StartDate = from.ToShortDbDate(),
          EndDate = to.ToShortDbDate(),
          ClientId = clientId
        }
      );
    }

    public async Task<TimeSheetDate> GetEntry(int userId, int clientId, DateTime date)
    {
      // TODO: [TESTS] (TimeSheetDateRepo.GetEntry) Add tests
      return await GetSingle<TimeSheetDate>(
        nameof(GetEntry),
        _queries.GetEntry(),
        new
        {
          UserId = userId,
          ClientId = clientId,
          EntryDate = date.ToShortDbDate()
        }
      );
    }

    public async Task<int> Add(TimeSheetDate timeSheetDate)
    {
      // TODO: [TESTS] (TimeSheetDateRepo.Add) Add tests
      return await ExecuteAsync(nameof(Add), _queries.Add(), timeSheetDate);
    }

    public async Task<TimeSheetDate> GetById(int dateId)
    {
      // TODO: [TESTS] (TimeSheetDateRepo.GetById) Add tests
      return await GetSingle<TimeSheetDate>(
        nameof(GetById),
        _queries.GetById(),
        new
        {
          DateId = dateId
        }
      );
    }
  }
}
