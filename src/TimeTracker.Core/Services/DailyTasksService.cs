using System;
using System.Threading.Tasks;
using Rn.NetCore.Common.Logging;
using Rn.NetCore.Common.Metrics;
using Rn.NetCore.Common.Metrics.Builders;
using TimeTracker.Core.Database.Repos;
using TimeTracker.Core.Enums;
using TimeTracker.Core.Models.Dto;

namespace TimeTracker.Core.Services
{
  public interface IDailyTasksService
  {
    Task<bool> AddDailyTask(int userId, DailyTaskDto taskDto);
  }

  public class DailyTasksService : IDailyTasksService
  {
    private readonly ILoggerAdapter<DailyTasksService> _logger;
    private readonly IMetricService _metrics;
    private readonly IDailyTasksRepo _tasksRepo;

    public DailyTasksService(
      ILoggerAdapter<DailyTasksService> logger,
      IMetricService metrics,
      IDailyTasksRepo tasksRepo)
    {
      _logger = logger;
      _metrics = metrics;
      _tasksRepo = tasksRepo;
    }

    public async Task<bool> AddDailyTask(int userId, DailyTaskDto taskDto)
    {
      // TODO: [TESTS] (DailyTasksService.AddDailyTask) Add tests
      var builder = new ServiceMetricBuilder(nameof(DailyTasksService), nameof(AddDailyTask))
        .WithCategory(MetricCategory.DailyTasks, MetricSubCategory.Add)
        .WithUserId(userId);

      try
      {
        using (builder.WithTiming())
        {
          var taskEntity = taskDto.AsEntity(userId);

          // Check to see if this task already exists
          using (builder.WithCustomTiming1())
          {
            builder.IncrementQueryCount();
            if (await _tasksRepo.SearchByName(taskEntity) != null)
            {
              builder.IncrementResultsCount();
              return true;
            }
          }

          // Add a new task to the DB
          using (builder.WithCustomTiming2())
          {
            builder.IncrementQueryCount();

            if (await _tasksRepo.AddTask(taskEntity) == 0)
            {
              builder.MarkFailed();
              return false;
            }

            builder.IncrementResultsCount();
            return true;
          }
        }
      }
      catch (Exception ex)
      {
        _logger.LogUnexpectedException(ex);
        builder.WithException(ex);
        return false;
      }
      finally
      {
        await _metrics.SubmitPointAsync(builder.Build());
      }
    }
  }
}
