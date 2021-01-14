using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Rn.NetCore.Common.Logging;
using Rn.NetCore.Common.Metrics;
using Rn.NetCore.Common.Metrics.Builders;
using TimeTracker.Core.Database.Entities;
using TimeTracker.Core.Database.Repos;
using TimeTracker.Core.Enums;
using TimeTracker.Core.Models.Dto;

namespace TimeTracker.Core.Services
{
  public interface IDailyTasksService
  {
    Task<List<DailyTaskDto>> ListClientTasks(int userId, int clientId);

    Task<bool> AddDailyTask(int userId, DailyTaskDto taskDto);
    Task<DailyTaskDto> GetTaskById(int userId, int taskId);
    Task<bool> UpdateTask(int userId, DailyTaskDto taskDto);
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

    public async Task<List<DailyTaskDto>> ListClientTasks(int userId, int clientId)
    {
      // TODO: [TESTS] (DailyTasksService.ListClientTasks) Add tests
      var builder = new ServiceMetricBuilder(nameof(DailyTasksService), nameof(ListClientTasks))
        .WithCategory(MetricCategory.DailyTasks, MetricSubCategory.GetList)
        .WithUserId(userId)
        .WithCustomInt1(clientId);

      try
      {
        using (builder.WithTiming())
        {
          List<DailyTaskEntity> tasks;
          using (builder.WithCustomTiming1())
          {
            builder.IncrementQueryCount();
            tasks = await _tasksRepo.ListClientTasks(clientId);
            builder.WithResultsCount(tasks.Count);
          }

          return tasks.AsQueryable()
            .Select(DailyTaskDto.Projection)
            .ToList();
        }
      }
      catch (Exception ex)
      {
        _logger.LogUnexpectedException(ex);
        builder.WithException(ex);
        return new List<DailyTaskDto>();
      }
      finally
      {
        await _metrics.SubmitPointAsync(builder.Build());
      }
    }


    public async Task<bool> AddDailyTask(int userId, DailyTaskDto taskDto)
    {
      // TODO: [TESTS] (DailyTasksService.AddDailyTask) Add tests
      var builder = new ServiceMetricBuilder(nameof(DailyTasksService), nameof(AddDailyTask))
        .WithCategory(MetricCategory.DailyTasks, MetricSubCategory.Add)
        .WithUserId(userId)
        .WithCustomInt1(taskDto.ClientId);

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

    public async Task<DailyTaskDto> GetTaskById(int userId, int taskId)
    {
      // TODO: [TESTS] (DailyTasksService.GetTaskById) Add tests
      var builder = new ServiceMetricBuilder(nameof(DailyTasksService), nameof(GetTaskById))
        .WithCategory(MetricCategory.DailyTasks, MetricSubCategory.GetById)
        .WithUserId(userId)
        .WithCustomInt1(0)
        .WithCustomInt2(taskId);

      try
      {
        using (builder.WithTiming())
        {
          DailyTaskEntity taskEntity;

          // Ensure that we are dealing with a valid task entry
          using (builder.WithCustomTiming1())
          {
            taskEntity = await GetById(builder, taskId);
            if (taskEntity == null)
              return null;
          }

          // Ensure that the provided user owns this task
          // ReSharper disable once InvertIf
          if (taskEntity.UserId != userId)
          {
            // TODO: [HANDLE] (DailyTasksService.GetTaskById) Handle this
            builder.MarkFailed();
            return null;
          }

          return DailyTaskDto.FromEntity(taskEntity);
        }
      }
      catch (Exception ex)
      {
        _logger.LogUnexpectedException(ex);
        builder.WithException(ex);
        return null;
      }
      finally
      {
        await _metrics.SubmitPointAsync(builder.Build());
      }
    }

    public async Task<bool> UpdateTask(int userId, DailyTaskDto taskDto)
    {
      // TODO: [TESTS] (DailyTasksService.UpdateTask) Add tests
      var builder = new ServiceMetricBuilder(nameof(DailyTasksService), nameof(GetTaskById))
        .WithCategory(MetricCategory.DailyTasks, MetricSubCategory.GetById)
        .WithUserId(userId)
        .WithCustomInt1(taskDto.ClientId)
        .WithCustomInt2(taskDto.TaskId);

      try
      {
        using (builder.WithTiming())
        {
          DailyTaskEntity taskEntity;

          // Ensure we have a valid task
          using (builder.WithCustomTiming1())
          {
            taskEntity = await GetById(builder, taskDto.TaskId);
            if (taskEntity == null)
              return false;
          }

          // Ensure that the provided user owns this task
          if (taskEntity.UserId != userId)
          {
            // TODO: [HANDLE] (DailyTasksService.UpdateTask) Handle this
            builder.MarkFailed();
            return false;
          }

          // Update the task
          using (builder.WithCustomTiming2())
          {
            builder.IncrementQueryCount();

            if (await _tasksRepo.UpdateTask(taskDto.AsEntity()) == 0)
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


    // Internal methods
    private async Task<DailyTaskEntity> GetById(ServiceMetricBuilder builder, int taskId)
    {
      // TODO: [TESTS] (DailyTasksService.GetById) Add tests
      builder.IncrementQueryCount();
      var taskEntity = await _tasksRepo.GetTaskById(taskId);

      if (taskEntity == null)
      {
        builder.MarkFailed();
        return null;
      }

      builder.IncrementResultsCount()
        .WithCustomInt1(taskEntity.ClientId)
        .WithCustomInt2(taskEntity.TaskId)
        .WithUserId(taskEntity.UserId);

      return taskEntity;
    }
  }
}
