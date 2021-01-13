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
  public interface ITrackedTimeService
  {
    Task<bool> StartNew(int userId, TrackedTimeDto entry);
    Task<List<TrackedTimeDto>> GetActive(int userId);
    Task<bool> Pause(int userId, long entryId, TimerEndReason endReason, string notes);
    Task<bool> Resume(int userId, long entryId);
    Task<bool> Stop(int userId, long entryId);
    Task<List<TrackedTimeDto>> GetProjectEntries(int userId, int projectId);
    Task<bool> UpdateDuration(int userId, TrackedTimeDto entry);
    Task<bool> ResumeSingle(int userId, long entryId);
  }

  public class TrackedTimeService : ITrackedTimeService
  {
    private readonly ILoggerAdapter<TrackedTimeService> _logger;
    private readonly IMetricService _metrics;
    private readonly ITrackedTimeRepo _timeRepo;

    public TrackedTimeService(
      ILoggerAdapter<TrackedTimeService> logger,
      IMetricService metrics,
      ITrackedTimeRepo timeRepo)
    {
      _logger = logger;
      _metrics = metrics;
      _timeRepo = timeRepo;
    }

    public async Task<bool> StartNew(int userId, TrackedTimeDto entry)
    {
      // TODO: [TESTS] (TrackedTimeService.StartNew) Add tests
      var builder = new ServiceMetricBuilder(nameof(TrackedTimeService), nameof(StartNew))
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

    public async Task<List<TrackedTimeDto>> GetActive(int userId)
    {
      // TODO: [TESTS] (TrackedTimeService.GetActiveTimers) Add tests
      var builder = new ServiceMetricBuilder(nameof(TrackedTimeService), nameof(GetActive))
        .WithCategory(MetricCategory.TrackedTime, MetricSubCategory.GetFiltered)
        .WithUserId(userId);

      try
      {
        using (builder.WithTiming())
        {
          List<TrackedTimeEntity> entries;
          using (builder.WithCustomTiming1())
          {
            entries = await _timeRepo.GetActive(userId);
            builder.IncrementQueryCount().WithResultsCount(entries.Count);
          }

          return entries.AsQueryable()
            .Select(TrackedTimeDto.Projection)
            .ToList();
        }
      }
      catch (Exception ex)
      {
        _logger.LogUnexpectedException(ex);
        builder.WithException(ex);
        return new List<TrackedTimeDto>();
      }
      finally
      {
        await _metrics.SubmitPointAsync(builder.Build());
      }
    }

    public async Task<bool> Pause(int userId, long entryId, TimerEndReason endReason, string notes)
    {
      // TODO: [TESTS] (TrackedTimeService.PauseTimer) Add tests
      var builder = new ServiceMetricBuilder(nameof(TrackedTimeService), nameof(Pause))
        .WithCategory(MetricCategory.TrackedTime, MetricSubCategory.Update)
        .WithCustomTag1(endReason.ToString("G"), true)
        .WithUserId(userId);

      try
      {
        using (builder.WithTiming())
        {
          TrackedTimeEntity dbEntry;
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
            // TODO: [HANDLE] (TrackedTimeService.PauseTimer) Handle this
            return false;
          }

          // Attempt to pause the timer
          using (builder.WithCustomTiming2())
          {
            if (await _timeRepo.Pause(entryId, endReason, notes) == 0)
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
      // TODO: [TESTS] (TrackedTimeService.Resume) Add tests
      var builder = new ServiceMetricBuilder(nameof(TrackedTimeService), nameof(Resume))
        .WithCategory(MetricCategory.TrackedTime, MetricSubCategory.Update)
        .WithUserId(userId);

      try
      {
        using (builder.WithTiming())
        {
          TrackedTimeEntity parentTimer;

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
            // TODO: [HANDLE] (TrackedTimeService.ResumeTimer) Handle this
            return false;
          }

          // If the timer is already running, we are done
          if (parentTimer.Running)
            return true;

          // End the current timer so we can start a new one
          using (builder.WithCustomTiming2())
          {
            builder.IncrementQueryCount();

            if (await _timeRepo.Stop(parentTimer.EntryId, TimerEndReason.Completed) == 0)
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
      // TODO: [TESTS] (TrackedTimeService.Stop) Add tests
      var builder = new ServiceMetricBuilder(nameof(TrackedTimeService), nameof(Stop))
        .WithCategory(MetricCategory.TrackedTime, MetricSubCategory.Update)
        .WithUserId(userId);

      try
      {
        using (builder.WithTiming())
        {
          TrackedTimeEntity dbTimer;

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
            // TODO: [HANDLE] (TrackedTimeService.StopTimer) Handle this
            return false;
          }

          // Stop the timer with "UserStopped"
          using (builder.WithCustomTiming2())
          {
            builder.IncrementQueryCount();

            if (await _timeRepo.Stop(entryId, TimerEndReason.UserStopped) == 0)
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

    public async Task<List<TrackedTimeDto>> GetProjectEntries(int userId, int projectId)
    {
      // TODO: [TESTS] (TrackedTimeService.GetProjectEntries) Add tests
      var builder = new ServiceMetricBuilder(nameof(TrackedTimeService), nameof(GetProjectEntries))
        .WithCategory(MetricCategory.TrackedTime, MetricSubCategory.GetList)
        .WithUserId(userId);

      try
      {
        using (builder.WithTiming())
        {
          List<TrackedTimeEntity> dbEntries;

          // Ensure that we have some entries to work with
          using (builder.WithCustomTiming1())
          {
            dbEntries = await _timeRepo.GetProjectEntries(projectId);
            builder.IncrementQueryCount();

            if ((dbEntries?.Count ?? 0) == 0)
              return new List<TrackedTimeDto>();

            AppendEntityInfo(builder, dbEntries.First());
          }

          // Ensure that the current user owns these entries
          if (dbEntries.First().UserId != userId)
          {
            // TODO: [HANDLE] (TrackedTimeService.GetTimerSeries) Handle this
            return new List<TrackedTimeDto>();
          }

          // Cast and return the results
          builder.WithResultsCount(dbEntries.Count);
          return dbEntries.AsQueryable()
            .Select(TrackedTimeDto.Projection)
            .ToList();
        }
      }
      catch (Exception ex)
      {
        _logger.LogUnexpectedException(ex);
        builder.WithException(ex);
        return new List<TrackedTimeDto>();
      }
      finally
      {
        await _metrics.SubmitPointAsync(builder.Build());
      }
    }

    public async Task<bool> UpdateDuration(int userId, TrackedTimeDto entry)
    {
      // TODO: [TESTS] (TrackedTimeService.UpdateDuration) Add tests
      var builder = new ServiceMetricBuilder(nameof(TrackedTimeService), nameof(UpdateDuration))
        .WithCategory(MetricCategory.TrackedTime, MetricSubCategory.Update)
        .WithUserId(userId);

      try
      {
        using (builder.WithTiming())
        {
          TrackedTimeEntity dbTimer;

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
            // TODO: [HANDLE] (TrackedTimeService.UpdateTimerDuration) Handle this
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
      // TODO: [TESTS] (TrackedTimeService.ResumeSingle) Add tests
      var builder = new ServiceMetricBuilder(nameof(TrackedTimeService), nameof(ResumeSingle))
        .WithCategory(MetricCategory.TrackedTime, MetricSubCategory.Update)
        .WithUserId(userId);

      try
      {
        using (builder.WithTiming())
        {
          TrackedTimeEntity parentTimer;

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
            // TODO: [HANDLE] (TrackedTimeService.ResumeSingleTimer) Handle this
            return false;
          }

          // Search for, and pause all currently running user timers
          using (builder.WithCustomTiming2())
          {
            builder.IncrementQueryCount();
            var runningTimers = await _timeRepo.GetRunning(userId);

            if (runningTimers.Count > 0)
            {
              const TimerEndReason endReason = TimerEndReason.Completed;
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
    private static void AppendEntityInfo(ServiceMetricBuilder builder, TrackedTimeEntity entity)
    {
      // TODO: [TESTS] (TrackedTimeService.AppendEntityInfo) Add tests
      builder
        .WithUserId(entity.UserId)
        .WithCustomInt1(entity.ClientId)
        .WithCustomInt2(entity.ProductId)
        .WithCustomInt3(entity.ProjectId);
    }
  }
}
