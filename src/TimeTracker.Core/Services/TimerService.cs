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
    Task<bool> StartNew(int userId, TimerDto entry);
    Task<List<TimerDto>> GetActive(int userId);
    Task<bool> Pause(int userId, long entryId, TimerState state, string notes);
    Task<bool> Resume(int userId, long entryId);
    Task<bool> Stop(int userId, long entryId);
    Task<List<TimerDto>> GetProjectEntries(int userId, int projectId);
    Task<bool> UpdateDuration(int userId, TimerDto entry);
    Task<bool> ResumeSingle(int userId, long entryId);
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

    public async Task<bool> StartNew(int userId, TimerDto entry)
    {
      // TODO: [TESTS] (TimerService.StartNew) Add tests
      var builder = new ServiceMetricBuilder(nameof(TimerService), nameof(StartNew))
        .WithCategory(MetricCategory.TrackedTime, MetricSubCategory.Add)
        .WithUserId(userId);

      try
      {
        using (builder.WithTiming())
        {
          AppendEntityInfo(builder, entry.AsEntity());
          var timerEntity = entry.AsEntity(userId);

          // Check for and handle a Running Existing timer
          using (builder.WithCustomTiming1())
          {
            var existingTimer = await _timeRepo.GetRunningExisting(timerEntity);
            builder.IncrementQueryCount().CountResult(existingTimer);
            if (existingTimer != null)
              return true;
          }

          // Try to create a new timer entry
          using (builder.WithCustomTiming2())
          {
            builder.IncrementQueryCount().WithResultsCount(1);
            if (await _timeRepo.StartNew(timerEntity) > 0)
              return true;

            builder.MarkFailed().WithResultsCount(0);
            return false;
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

    public async Task<List<TimerDto>> GetActive(int userId)
    {
      // TODO: [TESTS] (TimerService.GetActiveTimers) Add tests
      var builder = new ServiceMetricBuilder(nameof(TimerService), nameof(GetActive))
        .WithCategory(MetricCategory.TrackedTime, MetricSubCategory.GetFiltered)
        .WithUserId(userId);

      try
      {
        using (builder.WithTiming())
        {
          List<TimerEntity> entries;
          using (builder.WithCustomTiming1())
          {
            entries = await _timeRepo.GetActive(userId);
            builder.IncrementQueryCount().WithResultsCount(entries.Count);
          }

          return entries.AsQueryable()
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

    public async Task<bool> Pause(int userId, long entryId, TimerState state, string notes)
    {
      // TODO: [TESTS] (TimerService.PauseTimer) Add tests
      var builder = new ServiceMetricBuilder(nameof(TimerService), nameof(Pause))
        .WithCategory(MetricCategory.TrackedTime, MetricSubCategory.Update)
        .WithCustomTag1(state.ToString("G"), true)
        .WithUserId(userId);

      try
      {
        using (builder.WithTiming())
        {
          TimerEntity dbEntry;
          using (builder.WithCustomTiming1())
          {
            dbEntry = await _timeRepo.GetByEntryId(entryId);
            builder.IncrementQueryCount().CountResult(dbEntry);

            if (dbEntry == null)
            {
              builder.MarkFailed().WithResultsCount(0);
              return false;
            }

            AppendEntityInfo(builder, dbEntry);
          }

          if (dbEntry.UserId != userId)
          {
            // TODO: [HANDLE] (TimerService.PauseTimer) Handle this
            return false;
          }

          // Attempt to pause the timer
          using (builder.WithCustomTiming2())
          {
            if (await _timeRepo.Pause(entryId, state, notes) == 0)
            {
              builder.IncrementQueryCount().MarkFailed();
              return false;
            }

            builder.IncrementQueryCount().IncrementResultsCount();
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
            parentTimer = await _timeRepo.GetByEntryId(entryId);
            builder.IncrementQueryCount().CountResult(parentTimer);

            if (parentTimer == null)
            {
              builder.MarkFailed().WithResultsCount(0);
              return false;
            }

            AppendEntityInfo(builder, parentTimer);
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
            dbTimer = await _timeRepo.GetByEntryId(entryId);
            builder.IncrementQueryCount().CountResult(dbTimer);

            if (dbTimer == null)
            {
              builder.MarkFailed();
              return false;
            }

            AppendEntityInfo(builder, dbTimer);
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

            AppendEntityInfo(builder, dbEntries.First());
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
            dbTimer = await _timeRepo.GetByEntryId(entry.EntryId);
            builder.IncrementQueryCount().CountResult(dbTimer);

            if (dbTimer == null)
            {
              builder.MarkFailed();
              return false;
            }

            AppendEntityInfo(builder, dbTimer);
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
            parentTimer = await _timeRepo.GetByEntryId(entryId);
            builder.IncrementQueryCount().CountResult(parentTimer);

            if (parentTimer == null)
            {
              builder.MarkFailed();
              return false;
            }

            AppendEntityInfo(builder, parentTimer);
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
    private static void AppendEntityInfo(ServiceMetricBuilder builder, TimerEntity entity)
    {
      // TODO: [TESTS] (TimerService.AppendEntityInfo) Add tests
      builder
        .WithUserId(entity.UserId)
        .WithCustomInt1(entity.ClientId)
        .WithCustomInt2(entity.ProductId)
        .WithCustomInt3(entity.ProjectId);
    }
  }
}
