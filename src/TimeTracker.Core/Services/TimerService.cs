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
    Task<List<TimerDto>> GetActiveTimers(int userId);
    Task<List<TimerDto>> GetProjectTimers(int userId, int projectId);

    Task<bool> StartTimer(int userId, TimerDto timerDto);
    Task<bool> PauseTimer(int userId, long entryId, string notes = null);
    Task<bool> ResumeTimer(int userId, long entryId);
    Task<bool> CompleteTimer(int userId, long entryId, string notes = null);
    Task<bool> UpdateTimerDuration(int userId, TimerDto entry);
    Task<bool> ResumeSingleTimer(int userId, long entryId);
  }

  public class TimerService : ITimerService
  {
    private readonly ILoggerAdapter<TimerService> _logger;
    private readonly IMetricService _metrics;
    private readonly ITimerRepo _timerRepo;

    public TimerService(
      ILoggerAdapter<TimerService> logger,
      IMetricService metrics,
      ITimerRepo timerRepo)
    {
      _logger = logger;
      _metrics = metrics;
      _timerRepo = timerRepo;
    }


    // Multiple timer methods
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
            activeTimers = await _timerRepo.GetActiveTimers(userId);
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

    public async Task<List<TimerDto>> GetProjectTimers(int userId, int projectId)
    {
      // TODO: [TESTS] (TimerService.GetProjectTimers) Add tests
      var builder = new ServiceMetricBuilder(nameof(TimerService), nameof(GetProjectTimers))
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
            dbEntries = await _timerRepo.GetProjectTimers(projectId);
            builder.IncrementQueryCount();

            if ((dbEntries?.Count ?? 0) == 0)
              return new List<TimerDto>();

            AppendTimerInfo(builder, dbEntries.First());
            builder.WithResultsCount(dbEntries.Count);
          }

          // Ensure that the current user owns these entries
          if (dbEntries.First().UserId != userId)
          {
            // TODO: [HANDLE] (TimerService.GetTimerSeries) Handle this
            return new List<TimerDto>();
          }

          // Cast and return the results
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


    // Single timer methods
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

          // Look for any non-completed timer matching the provided criteria
          using (builder.WithCustomTiming1())
          {
            builder.IncrementQueryCount();
            var activeTimer = await _timerRepo.GetActiveTimer(timerEntity);
            builder.CountResult(activeTimer);

            if (activeTimer != null)
            {
              // Already running, we are done
              if (activeTimer.Running)
                return true;

              // We will need to complete this timer before starting a new one
              builder.IncrementQueryCount();
              if (await _timerRepo.CompleteTimer(activeTimer.EntryId, "user-completed") == 0)
              {
                // TODO: [HANDLE] (TimerService.StartTimer) Handle this
                builder.MarkFailed();
                return false;
              }

              builder.IncrementResultsCount();
            }
          }

          // Create a new timer
          using (builder.WithCustomTiming2())
            return await AddTimer(builder, timerEntity);
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
          TimerEntity timerEntity;
          notes = string.IsNullOrWhiteSpace(notes) ? "user-paused" : notes;

          // Check to see if this is a valid timer
          using (builder.WithCustomTiming1())
          {
            timerEntity = await GetTimerById(builder, entryId);
            if (timerEntity == null)
              return false;
          }

          // Check to see that the provided user owns this timer
          if (timerEntity.UserId != userId)
          {
            // TODO: [HANDLE] (TimerService.PauseTimer) Handle this
            return false;
          }

          // Pause the timer
          using (builder.WithCustomTiming2())
          {
            builder.IncrementQueryCount();

            if (await _timerRepo.PauseTimer(entryId, notes) == 0)
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

    public async Task<bool> ResumeTimer(int userId, long entryId)
    {
      // TODO: [TESTS] (TimerService.ResumeTimer) Add tests
      var builder = new ServiceMetricBuilder(nameof(TimerService), nameof(ResumeTimer))
        .WithCategory(MetricCategory.TrackedTime, MetricSubCategory.Update)
        .WithUserId(userId);

      try
      {
        using (builder.WithTiming())
        {
          TimerEntity timerEntity;

          // Ensure that we have a valid timer
          using (builder.WithCustomTiming1())
          {
            timerEntity = await GetTimerById(builder, entryId);
            if (timerEntity == null)
              return false;
          }

          // Ensure the provided user owns the timer
          if (timerEntity.UserId != userId)
          {
            // TODO: [HANDLE] (TimerService.ResumeTimer) Handle this
            return false;
          }

          // If the timer is already running, we are done
          if (timerEntity.Running)
            return true;

          // End the current timer so we can start a new one
          using (builder.WithCustomTiming2())
          {
            builder.IncrementQueryCount();

            if (await _timerRepo.CompleteTimer(timerEntity.EntryId, "completed") == 0)
            {
              builder.MarkFailed();
              return false;
            }

            builder.IncrementResultsCount();
          }

          // Create a new timer
          using (builder.WithCustomTiming3())
            return await AddTimer(builder, timerEntity);
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

    public async Task<bool> CompleteTimer(int userId, long entryId, string notes = null)
    {
      // TODO: [TESTS] (TimerService.CompleteTimer) Add tests
      var builder = new ServiceMetricBuilder(nameof(TimerService), nameof(CompleteTimer))
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
            dbTimer = await GetTimerById(builder, entryId);
            if (dbTimer == null)
              return false;
          }

          // Ensure the provided user owns this timer
          if (dbTimer.UserId != userId)
          {
            // TODO: [HANDLE] (TimerService.StopTimer) Handle this
            return false;
          }

          // Stop the timer with "user stopped"
          using (builder.WithCustomTiming2())
          {
            notes = string.IsNullOrWhiteSpace(notes) ? "user-stopped" : notes;
            builder.IncrementQueryCount();

            if (await _timerRepo.CompleteTimer(entryId, notes) == 0)
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

    public async Task<bool> UpdateTimerDuration(int userId, TimerDto entry)
    {
      // TODO: [TESTS] (TimerService.UpdateTimerDuration) Add tests
      var builder = new ServiceMetricBuilder(nameof(TimerService), nameof(UpdateTimerDuration))
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
            dbTimer = await GetTimerById(builder, entry.EntryId);
            if (dbTimer == null)
              return false;
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

            if (await _timerRepo.UpdateTimerDuration(entry.AsEntity()) == 0)
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

    public async Task<bool> ResumeSingleTimer(int userId, long entryId)
    {
      // TODO: [TESTS] (TimerService.ResumeSingleTimer) Add tests
      var builder = new ServiceMetricBuilder(nameof(TimerService), nameof(ResumeSingleTimer))
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
            parentTimer = await GetTimerById(builder, entryId);
            if (parentTimer == null)
              return false;
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
            var runningTimers = await _timerRepo.GetRunningTimers(userId);
            if (runningTimers.Count > 0)
            {
              foreach (var timer in runningTimers)
              {
                builder.IncrementQueryCount();
                if (await _timerRepo.PauseTimer(timer.EntryId, "auto-paused") > 0)
                  builder.IncrementResultsCount();
              }
            }
          }

          // Ensure that the parent timer is completed (so we can spawn a new one)
          using (builder.WithCustomTiming3())
          {
            builder.IncrementQueryCount();

            if (await _timerRepo.CompleteTimer(parentTimer.EntryId, "auto-completed") == 0)
            {
              builder.MarkFailed();
              return false;
            }

            builder.IncrementResultsCount();
          }

          // Resume the given timer
          using (builder.WithCustomTiming4())
            return await AddTimer(builder, parentTimer);
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

    private async Task<TimerEntity> GetTimerById(ServiceMetricBuilder builder, long entryId)
    {
      // TODO: [TESTS] (TimerService.GetTimerById) Add tests
      builder.IncrementQueryCount();
      var dbTimer = await _timerRepo.GetTimerById(entryId);

      if (dbTimer == null)
      {
        builder.MarkFailed();
        return null;
      }

      builder.IncrementResultsCount();
      AppendTimerInfo(builder, dbTimer);
      return dbTimer;
    }

    private async Task<bool> AddTimer(ServiceMetricBuilder builder, TimerEntity timerEntity)
    {
      // TODO: [TESTS] (TimerService.AddTimer) Add tests
      builder.IncrementQueryCount();

      if (await _timerRepo.AddTimer(timerEntity) == 0)
      {
        builder.MarkFailed();
        return false;
      }

      builder.IncrementResultsCount();
      return true;
    }
  }
}
