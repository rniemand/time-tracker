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
  public interface ITimerService
  {
    Task<bool> StartTimer(int userId, TimerDto timerDto);
    Task<List<TimerDto>> GetActiveTimers(int userId);
    Task<bool> PauseTimer(int userId, long entryId, string notes = null);
    Task<bool> Resume(int userId, long entryId); // review
    Task<bool> Stop(int userId, long entryId); // review
    Task<List<TimerDto>> GetProjectEntries(int userId, int projectId); // review
    Task<bool> UpdateDuration(int userId, TimerDto entry); // review
    Task<bool> ResumeSingle(int userId, long entryId); // review
  }

  public class TimerService : ITimerService
  {
    private readonly ILoggerAdapter<TimerService> _logger;
    private readonly IMetricService _metrics;
    private readonly ITimerRepo _timeRepo;

    public TimerService(
      ILoggerAdapter<TimerService> logger,
      IMetricService metrics,
      ITimerRepo timeRepo)
    {
      _logger = logger;
      _metrics = metrics;
      _timeRepo = timeRepo;
    }

    public async Task<bool> StartTimer(int userId, TimerDto timerDto)
    {
      // TODO: [TESTS] (TimerService.StartTimer) Add tests
      var builder = new ServiceMetricBuilder(nameof(TimerService), nameof(StartTimer))
        .WithCategory(MetricCategory.TrackedTime, MetricSubCategory.Add)
        .WithUserId(userId);

      try
      {
        using (builder.WithTiming())
        {
          var timerEntity = timerDto.AsEntity(userId);
          AppendTimerInfo(builder, timerEntity);

          // If there is a running timer matching our criteria there is no need to start a new one
          using (builder.WithCustomTiming1())
          {
            builder.IncrementQueryCount();
            if (await _timeRepo.GetRunningTimer(timerEntity) != null)
            {
              builder.IncrementResultsCount();
              return true;
            }
          }

          // Create a new timer
          using (builder.WithCustomTiming2())
          {
            builder.IncrementQueryCount();
            if (await _timeRepo.AddTimer(timerEntity) == 0)
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

    public async Task<List<TimerDto>> GetActiveTimers(int userId)
    {
      // TODO: [TESTS] (TimerService.GetActiveTimers) Add tests
      var builder = new ServiceMetricBuilder(nameof(TimerService), nameof(GetActiveTimers))
        .WithCategory(MetricCategory.TrackedTime, MetricSubCategory.GetFiltered)
        .WithUserId(userId);

      try
      {
        using (builder.WithTiming())
        {
          List<TimerEntity> activeTimers;
          using (builder.WithCustomTiming1())
          {
            builder.IncrementQueryCount();
            activeTimers = await _timeRepo.GetActiveTimers(userId);
            builder.WithResultsCount(activeTimers.Count);
          }

          return activeTimers.AsQueryable()
            .Select(TimerDto.Projection)
            .ToList();
        }
      }
      catch (Exception ex)
      {
        _logger.LogUnexpectedException(ex);
        builder.WithException(ex);
        return new List<TimerDto>();
      }
      finally
      {
        await _metrics.SubmitPointAsync(builder.Build());
      }
    }

    public async Task<bool> PauseTimer(int userId, long entryId, string notes = null)
    {
      // TODO: [TESTS] (TimerService.PauseTimer) Add tests
      var builder = new ServiceMetricBuilder(nameof(TimerService), nameof(PauseTimer))
        .WithCategory(MetricCategory.TrackedTime, MetricSubCategory.Update)
        .WithCustomTag1(TimerState.Paused.ToString("G"), true)
        .WithUserId(userId);

      try
      {
        using (builder.WithTiming())
        {
          TimerEntity dbEntry;
          notes = string.IsNullOrWhiteSpace(notes) ? "user-paused" : notes;

          // Check to see if this is a valid timer
          using (builder.WithCustomTiming1())
          {
            builder.IncrementQueryCount();
            dbEntry = await _timeRepo.GetTimerById(entryId);

            if (dbEntry == null)
            {
              builder.MarkFailed();
              return false;
            }

            builder.IncrementResultsCount();
            AppendTimerInfo(builder, dbEntry);
          }

          // Check to see that the provided user owns this timer
          if (dbEntry.UserId != userId)
          {
            // TODO: [HANDLE] (TimerService.PauseTimer) Handle this
            return false;
          }

          // Pause the timer
          using (builder.WithCustomTiming2())
          {
            builder.IncrementQueryCount();

            if (await _timeRepo.PauseTimer(entryId, TimerState.Paused, notes) == 0)
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

    public async Task<bool> Resume(int userId, long entryId)
    {
      // TODO: [TESTS] (TimerService.Resume) Add tests
      var builder = new ServiceMetricBuilder(nameof(TimerService), nameof(Resume))
        .WithCategory(MetricCategory.TrackedTime, MetricSubCategory.Update)
        .WithUserId(userId);

      try
      {
        using (builder.WithTiming())
        {
          TimerEntity parentTimer;

          // Ensure that we have a valid timer
          using (builder.WithCustomTiming1())
          {
            parentTimer = await _timeRepo.GetTimerById(entryId);
            builder.IncrementQueryCount().CountResult(parentTimer);

            if (parentTimer == null)
            {
              builder.MarkFailed().WithResultsCount(0);
              return false;
            }

            AppendTimerInfo(builder, parentTimer);
          }

          // Ensure the provided user owns the timer
          if (parentTimer.UserId != userId)
          {
            // TODO: [HANDLE] (TimerService.ResumeTimer) Handle this
            return false;
          }

          // If the timer is already running, we are done
          if (parentTimer.Running)
            return true;

          // End the current timer so we can start a new one
          using (builder.WithCustomTiming2())
          {
            builder.IncrementQueryCount();

            if (await _timeRepo.Stop(parentTimer.EntryId, TimerState.Completed) == 0)
            {
              builder.MarkFailed();
              return false;
            }

            builder.IncrementResultsCount();
          }

          // Create a new timer
          using (builder.WithCustomTiming3())
          {
            builder.IncrementQueryCount();

            if (await _timeRepo.StartNew(parentTimer) == 0)
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

    public async Task<bool> Stop(int userId, long entryId)
    {
      // TODO: [TESTS] (TimerService.Stop) Add tests
      var builder = new ServiceMetricBuilder(nameof(TimerService), nameof(Stop))
        .WithCategory(MetricCategory.TrackedTime, MetricSubCategory.Update)
        .WithUserId(userId);

      try
      {
        using (builder.WithTiming())
        {
          TimerEntity dbTimer;

          // Ensure that we are working with a valid timer
          using (builder.WithCustomTiming1())
          {
            dbTimer = await _timeRepo.GetTimerById(entryId);
            builder.IncrementQueryCount().CountResult(dbTimer);

            if (dbTimer == null)
            {
              builder.MarkFailed();
              return false;
            }

            AppendTimerInfo(builder, dbTimer);
          }

          // Ensure the provided user owns this timer
          if (dbTimer.UserId != userId)
          {
            // TODO: [HANDLE] (TimerService.StopTimer) Handle this
            return false;
          }

          // Stop the timer with "UserStopped"
          using (builder.WithCustomTiming2())
          {
            builder.IncrementQueryCount();

            if (await _timeRepo.Stop(entryId, TimerState.UserStopped) == 0)
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

    public async Task<List<TimerDto>> GetProjectEntries(int userId, int projectId)
    {
      // TODO: [TESTS] (TimerService.GetProjectEntries) Add tests
      var builder = new ServiceMetricBuilder(nameof(TimerService), nameof(GetProjectEntries))
        .WithCategory(MetricCategory.TrackedTime, MetricSubCategory.GetList)
        .WithUserId(userId);

      try
      {
        using (builder.WithTiming())
        {
          List<TimerEntity> dbEntries;

          // Ensure that we have some entries to work with
          using (builder.WithCustomTiming1())
          {
            dbEntries = await _timeRepo.GetProjectEntries(projectId);
            builder.IncrementQueryCount();

            if ((dbEntries?.Count ?? 0) == 0)
              return new List<TimerDto>();

            AppendTimerInfo(builder, dbEntries.First());
          }

          // Ensure that the current user owns these entries
          if (dbEntries.First().UserId != userId)
          {
            // TODO: [HANDLE] (TimerService.GetTimerSeries) Handle this
            return new List<TimerDto>();
          }

          // Cast and return the results
          builder.WithResultsCount(dbEntries.Count);
          return dbEntries.AsQueryable()
            .Select(TimerDto.Projection)
            .ToList();
        }
      }
      catch (Exception ex)
      {
        _logger.LogUnexpectedException(ex);
        builder.WithException(ex);
        return new List<TimerDto>();
      }
      finally
      {
        await _metrics.SubmitPointAsync(builder.Build());
      }
    }

    public async Task<bool> UpdateDuration(int userId, TimerDto entry)
    {
      // TODO: [TESTS] (TimerService.UpdateDuration) Add tests
      var builder = new ServiceMetricBuilder(nameof(TimerService), nameof(UpdateDuration))
        .WithCategory(MetricCategory.TrackedTime, MetricSubCategory.Update)
        .WithUserId(userId);

      try
      {
        using (builder.WithTiming())
        {
          TimerEntity dbTimer;

          // Ensure that we have a valid entry to work with
          using (builder.WithCustomTiming1())
          {
            dbTimer = await _timeRepo.GetTimerById(entry.EntryId);
            builder.IncrementQueryCount().CountResult(dbTimer);

            if (dbTimer == null)
            {
              builder.MarkFailed();
              return false;
            }

            AppendTimerInfo(builder, dbTimer);
          }

          // Ensure that the current user owns this entry
          if (dbTimer.UserId != userId)
          {
            // TODO: [HANDLE] (TimerService.UpdateTimerDuration) Handle this
            return false;
          }

          // Update the timer entry
          using (builder.WithCustomTiming2())
          {
            builder.IncrementQueryCount();

            if (await _timeRepo.UpdateDuration(entry.AsEntity()) == 0)
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

    public async Task<bool> ResumeSingle(int userId, long entryId)
    {
      // TODO: [TESTS] (TimerService.ResumeSingle) Add tests
      var builder = new ServiceMetricBuilder(nameof(TimerService), nameof(ResumeSingle))
        .WithCategory(MetricCategory.TrackedTime, MetricSubCategory.Update)
        .WithUserId(userId);

      try
      {
        using (builder.WithTiming())
        {
          TimerEntity parentTimer;

          // Ensure that we have a valid timer to work with
          using (builder.WithCustomTiming1())
          {
            parentTimer = await _timeRepo.GetTimerById(entryId);
            builder.IncrementQueryCount().CountResult(parentTimer);

            if (parentTimer == null)
            {
              builder.MarkFailed();
              return false;
            }

            AppendTimerInfo(builder, parentTimer);
          }

          // Ensure that the provided user owns this timer
          if (parentTimer.UserId != userId)
          {
            // TODO: [HANDLE] (TimerService.ResumeSingleTimer) Handle this
            return false;
          }

          // Search for, and pause all currently running user timers
          using (builder.WithCustomTiming2())
          {
            builder.IncrementQueryCount();
            var runningTimers = await _timeRepo.GetRunning(userId);

            if (runningTimers.Count > 0)
            {
              const TimerState endReason = TimerState.Completed;
              const string endReasonString = "auto-completed";

              foreach (var timer in runningTimers)
              {
                builder.IncrementQueryCount();
                if (await _timeRepo.Complete(timer.EntryId, endReason, endReasonString) > 0)
                  builder.IncrementResultsCount();
              }
            }
          }

          // Resume the given timer
          using (builder.WithCustomTiming3())
          {
            builder.IncrementQueryCount();

            if (await _timeRepo.StartNew(parentTimer) == 0)
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
    private static void AppendTimerInfo(ServiceMetricBuilder builder, TimerEntity entity)
    {
      // TODO: [TESTS] (TimerService.AppendTimerInfo) Add tests
      builder
        .WithUserId(entity.UserId)
        .WithCustomInt1(entity.ClientId)
        .WithCustomInt2(entity.ProductId)
        .WithCustomInt3(entity.ProjectId);
    }
  }
}
