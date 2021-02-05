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
    Task<List<TimerDto>> GetDailyTaskTimers(int userId, int taskId);
    Task<List<TimerDto>> ListUserTimers(int userId, DateTime fromDate);
    Task<List<TimerDto>> ListUserTimers(int userId, DateTime startDate, DateTime endDate);

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
          builder.IncrementQueryCount();
          var activeTimers = await _timerRepo.GetActiveTimers(userId);
          builder.WithResultsCount(activeTimers.Count);

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
          // Ensure that we have some entries to work with
          var dbEntries = await _timerRepo.GetProjectTimers(projectId);
          builder.IncrementQueryCount();

          if ((dbEntries?.Count ?? 0) == 0)
            return new List<TimerDto>();

          AppendTimerInfo(builder, dbEntries.First());
          builder.WithResultsCount(dbEntries.Count);

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

    public async Task<List<TimerDto>> GetDailyTaskTimers(int userId, int taskId)
    {
      // TODO: [TESTS] (TimerService.GetDailyTaskTimers) Add tests
      var builder = new ServiceMetricBuilder(nameof(TimerService), nameof(GetDailyTaskTimers))
        .WithCategory(MetricCategory.TrackedTime, MetricSubCategory.GetList)
        .WithUserId(userId)
        .WithCustomInt1(taskId);

      try
      {
        using (builder.WithTiming())
        {
          // Fetch related timer entries for the given taskId
          builder.IncrementQueryCount();
          var timers = await _timerRepo.GetDailyTaskTimers(taskId);

          if (timers.Count == 0)
            return new List<TimerDto>();

          builder.WithResultsCount(timers.Count);

          // Ensure that the provided user owns these entries
          // ReSharper disable once InvertIf
          if (timers.First().UserId != userId)
          {
            // TODO: [HANDLE] (TimerService.GetDailyTaskTimers) Handle this
            builder.MarkFailed();
            return new List<TimerDto>();
          }

          // Cast and return the entries
          return timers.AsQueryable()
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

    public async Task<List<TimerDto>> ListUserTimers(int userId, DateTime fromDate)
    {
      // TODO: [TESTS] (TimerService.ListUserTimers) Add tests
      var builder = new ServiceMetricBuilder(nameof(TimerService), nameof(ListUserTimers))
        .WithCategory(MetricCategory.TrackedTime, MetricSubCategory.GetList)
        .WithUserId(userId);

      try
      {
        using (builder.WithTiming())
        {
          builder.IncrementQueryCount();
          var dbTimers = await _timerRepo.ListUserTimers(userId, fromDate);
          builder.WithResultsCount(dbTimers.Count);

          if (dbTimers.Count == 0)
            return new List<TimerDto>();

          return dbTimers.AsQueryable()
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

    public async Task<List<TimerDto>> ListUserTimers(int userId, DateTime startDate, DateTime endDate)
    {
      // TODO: [TESTS] (TimerService.ListUserTimers) Add tests
      var builder = new ServiceMetricBuilder(nameof(TimerService), nameof(ListUserTimers))
        .WithCategory(MetricCategory.TrackedTime, MetricSubCategory.GetList)
        .WithUserId(userId);

      try
      {
        using (builder.WithTiming())
        {
          builder.IncrementQueryCount();
          var dbTimers = await _timerRepo.ListUserTimers(userId, startDate, endDate);
          builder.WithResultsCount(dbTimers.Count);

          if (dbTimers.Count == 0)
            return new List<TimerDto>();

          return dbTimers.AsQueryable()
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
        .WithUserId(userId)
        .MarkFailed();

      try
      {
        using (builder.WithTiming())
        {
          var timerEntity = timerDto.AsEntity(userId);
          AppendTimerInfo(builder, timerEntity);

          // Look for any non-completed timer matching the provided criteria
          builder.IncrementQueryCount();
          var currentTimer = await _timerRepo.GetActiveTimer(timerEntity);
          builder.CountResult(currentTimer);

          if (currentTimer != null)
          {
            // The current timer is already running, there is nothing else to do
            if (currentTimer.Running)
            {
              builder.MarkSuccess();
              return true;
            }

            // We will need to "Complete" the active timer in order to start a new one
            var timerNote = TimerNote.GenerateTimerNote(currentTimer.Notes, TimerNote.UserCompleted);
            builder.IncrementQueryCount();
            if (await _timerRepo.CompleteTimer(currentTimer.EntryId, timerNote) == 0)
            {
              return false;
            }

            builder.MarkSuccess().IncrementResultsCount();
          }

          // Create a new timer
          using (builder.WithCustomTiming2())
          {
            return await AddTimer(builder, timerEntity);
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
          // Check to see if this is a valid timer
          var timerEntity = await GetTimerById(builder, entryId);
          if (!ValidateTimer(userId, timerEntity))
            return false;

          // Work out the correct notes to apply to the timer
          notes = string.IsNullOrWhiteSpace(notes) ? TimerNote.UserPaused : notes;
          var timerNote = TimerNote.GenerateTimerNote(timerEntity.Notes, notes);

          // Pause the timer
          builder.IncrementQueryCount();
          if (await _timerRepo.PauseTimer(entryId, timerNote) == 0)
          {
            builder.MarkFailed();
            return false;
          }

          builder.IncrementResultsCount();
          return true;
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
          // Ensure that we have a valid timer
          var timerEntity = await GetTimerById(builder, entryId);
          if (!ValidateTimer(userId, timerEntity))
            return false;

          // If the timer is already running, we are done
          if (timerEntity.Running)
            return true;

          var timerNote = TimerNote.GenerateTimerNote(timerEntity.Notes, TimerNote.Completed);
          
          // End the current timer so we can start a new one
          builder.IncrementQueryCount();
          if (await _timerRepo.CompleteTimer(timerEntity.EntryId, timerNote) == 0)
          {
            builder.MarkFailed();
            return false;
          }

          builder.IncrementResultsCount();

          // Create a new timer
          using (builder.WithCustomTiming3())
          {
            return await AddTimer(builder, timerEntity);
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
          // Ensure that we are working with a valid timer
          var dbTimer = await GetTimerById(builder, entryId);
          if (!ValidateTimer(userId, dbTimer))
            return false;
          
          // Generate notes for the completed timer
          notes = string.IsNullOrWhiteSpace(notes) ? TimerNote.UserStopped : notes;
          var timerNote = TimerNote.GenerateTimerNote(dbTimer.Notes, notes);
          
          // Complete the timer
          builder.IncrementQueryCount();
          if (await _timerRepo.CompleteTimer(entryId, timerNote) == 0)
          {
            builder.MarkFailed();
            return false;
          }

          builder.IncrementResultsCount();
          return true;
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
          // Ensure that we have a valid entry to work with
          var dbTimer = await GetTimerById(builder, entry.EntryId);
          if (!ValidateTimer(userId, dbTimer))
            return false;

          // Update the timer entry
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
          // Ensure that we have a valid timer to work with
          var parentTimer = await GetTimerById(builder, entryId);
          if (!ValidateTimer(userId, parentTimer))
            return false;

          string timerNote;

          // Search for, and pause all currently running user timers
          builder.IncrementQueryCount();
          var runningTimers = await _timerRepo.GetRunningTimers(userId);
          if (runningTimers.Count > 0)
          {
            foreach (var timer in runningTimers)
            {
              timerNote = TimerNote.GenerateTimerNote(timer.Notes, TimerNote.AutoPaused);
              builder.IncrementQueryCount();
              if (await _timerRepo.PauseTimer(timer.EntryId, timerNote) > 0)
              {
                builder.IncrementResultsCount();
              }
            }
          }

          // Ensure that the parent timer is completed (so we can spawn a new one)
          timerNote = TimerNote.GenerateTimerNote(parentTimer.Notes, TimerNote.AutoCompleted);
          builder.IncrementQueryCount();
          if (await _timerRepo.CompleteTimer(parentTimer.EntryId, timerNote) == 0)
          {
            builder.MarkFailed();
            return false;
          }

          builder.IncrementResultsCount();

          // Resume the given timer
          using (builder.WithCustomTiming4())
          {
            return await AddTimer(builder, parentTimer);
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

      builder.MarkSuccess().IncrementResultsCount();
      return true;
    }

    private static bool ValidateTimer(int userId, TimerEntity dbTimer = null)
    {
      // TODO: [TESTS] (TimerService.ValidateTimer) Add tests
      if (dbTimer == null)
      {
        return false;
      }

      // Ensure that the provided user owns this timer
      // ReSharper disable once ConvertIfStatementToReturnStatement
      if (dbTimer.UserId == userId)
      {
        return true;
      }

      // TODO: [HANDLE] (TimerService.ValidateTimer) Handle this
      return false;
    }
  }
}
