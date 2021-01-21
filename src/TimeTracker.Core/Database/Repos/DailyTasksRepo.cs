using System.Collections.Generic;
using System.Threading.Tasks;
using Rn.NetCore.Common.Logging;
using Rn.NetCore.Common.Metrics;
using Rn.NetCore.DbCommon.Helpers;
using Rn.NetCore.DbCommon.Repos;
using TimeTracker.Core.Database.Entities;
using TimeTracker.Core.Database.Queries;
using TimeTracker.Core.Models.Dto;

namespace TimeTracker.Core.Database.Repos
{
  public interface IDailyTasksRepo
  {
    Task<List<DailyTaskEntity>> ListClientTasks(int clientId);
    Task<List<IntListItem>> GetClientTasksList(int clientId);

    Task<int> AddTask(DailyTaskEntity taskEntity);
    Task<DailyTaskEntity> SearchByName(DailyTaskEntity taskEntity);
    Task<DailyTaskEntity> GetTaskById(int taskId);
    Task<int> UpdateTask(DailyTaskEntity taskEntity);
  }

  public class DailyTasksRepo : BaseRepo<DailyTasksRepo>, IDailyTasksRepo
  {
    private readonly IDailyTasksQueries _queries;

    public DailyTasksRepo(
      ILoggerAdapter<DailyTasksRepo> logger,
      IDbHelper dbHelper,
      IMetricService metricService,
      IDailyTasksQueries queries) : base(logger, dbHelper, metricService, nameof(DailyTasksRepo), TargetDB.TimeTracker)
    {
      _queries = queries;
    }

    public async Task<List<DailyTaskEntity>> ListClientTasks(int clientId)
    {
      // TODO: [TESTS] (DailyTasksRepo.ListClientTasks) Add tests
      return await GetList<DailyTaskEntity>(
        nameof(ListClientTasks),
        _queries.ListClientTasks(),
        new
        {
          ClientId = clientId
        }
      );
    }

    public async Task<List<IntListItem>> GetClientTasksList(int clientId)
    {
      // TODO: [TESTS] (DailyTasksRepo.GetClientTasksList) Add tests
      return await GetList<IntListItem>(
        nameof(GetClientTasksList),
        _queries.GetClientTasksList(),
        new
        {
          ClientId = clientId
        }
      );
    }


    public async Task<int> AddTask(DailyTaskEntity taskEntity)
    {
      // TODO: [TESTS] (DailyTasksRepo.AddTask) Add tests
      return await ExecuteAsync(
        nameof(AddTask),
        _queries.AddTask(),
        taskEntity
      );
    }

    public async Task<DailyTaskEntity> SearchByName(DailyTaskEntity taskEntity)
    {
      // TODO: [TESTS] (DailyTasksRepo.SearchByName) Add tests
      return await GetSingle<DailyTaskEntity>(
        nameof(SearchByName),
        _queries.SearchByName(),
        taskEntity
      );
    }

    public async Task<DailyTaskEntity> GetTaskById(int taskId)
    {
      // TODO: [TESTS] (DailyTasksRepo.GetTaskById) Add tests
      return await GetSingle<DailyTaskEntity>(
        nameof(GetTaskById),
        _queries.GetTaskById(),
        new
        {
          TaskId = taskId
        }
      );
    }

    public async Task<int> UpdateTask(DailyTaskEntity taskEntity)
    {
      // TODO: [TESTS] (DailyTasksRepo.UpdateTask) Add tests
      return await ExecuteAsync(
        nameof(UpdateTask),
        _queries.UpdateTask(),
        taskEntity
      );
    }
  }
}
