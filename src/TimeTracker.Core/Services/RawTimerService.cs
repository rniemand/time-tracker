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
    Task<bool> StartNew(int userId, TrackedTimeDto timerDto);
    Task<List<TrackedTimeDto>> GetActiveTimers(int userId);
    Task<bool> PauseTimer(int userId, long rawTimerId, EntryRunningState state, string notes);
    Task<bool> ResumeTimer(int userId, long rawTimerId);
    Task<bool> StopTimer(int userId, long rawTimerId);
    Task<List<TrackedTimeDto>> GetTimerSeries(int userId, long rootTimerId);
    Task<bool> UpdateNotes(int userId, long rawTimerId, string notes);
    Task<bool> UpdateTimerDuration(int userId, TrackedTimeDto timerDto);
    Task<bool> ResumeSingleTimer(int userId, long rawTimerId);
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

    public async Task<bool> StartNew(int userId, TrackedTimeDto timerDto)
    {
      // TODO: [TESTS] (TrackedTimeService.StartNew) Add tests
      var builder = new ServiceMetricBuilder(nameof(TrackedTimeService), nameof(StartNew))
        .WithCategory(MetricCategory.TrackedTime, MetricSubCategory.Add)
        .WithUserId(userId)
        .WithCustomInt1(timerDto.ClientId)
        .WithCustomInt2(timerDto.ProductId)
        .WithCustomInt3(timerDto.ProjectId);

      try
      {
        using (builder.WithTiming())
        {
          var timerEntity = timerDto.AsEntity(userId);

          // Handle the case of an existing timer
          using (builder.WithCustomTiming1())
          {
            var existingTimer = await _timeRepo.GetExistingTimer(timerEntity);
            builder.IncrementQueryCount().CountResult(existingTimer);
            if (existingTimer != null)
              return true;
          }

          // Attempt to create a new timer entry
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

    public async Task<List<TrackedTimeDto>> GetActiveTimers(int userId)
    {
      // TODO: [TESTS] (TrackedTimeService.GetActiveTimers) Add tests
      var builder = new ServiceMetricBuilder(nameof(TrackedTimeService), nameof(GetActiveTimers))
        .WithCategory(MetricCategory.TrackedTime, MetricSubCategory.GetList)
        .WithCustomInt1(userId);

      try
      {
        using (builder.WithTiming())
        {
          List<TrackedTimeEntity> dbEntries;
          using (builder.WithCustomTiming1())
          {
            builder.IncrementQueryCount();
            dbEntries = await _timeRepo.GetActiveTimers(userId);
            builder.WithResultsCount(dbEntries.Count);
          }

          return dbEntries.AsQueryable().Select(TrackedTimeDto.Projection).ToList();
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

    public async Task<bool> PauseTimer(int userId, long rawTimerId, EntryRunningState state, string notes)
    {
      // TODO: [TESTS] (TrackedTimeService.PauseTimer) Add tests
      var builder = new ServiceMetricBuilder(nameof(TrackedTimeService), nameof(PauseTimer))
        .WithCategory(MetricCategory.TrackedTime, MetricSubCategory.Update)
        .WithCustomInt1(userId)
        .WithCustomLong1(rawTimerId);

      try
      {
        using (builder.WithTiming())
        {
          TrackedTimeEntity dbEntry;
          using (builder.WithCustomTiming1())
          {
            builder.IncrementQueryCount();
            dbEntry = await _timeRepo.GetByRawTimerId(rawTimerId);
            builder.CountResult(dbEntry);
          }

          builder
            .WithCustomInt2(dbEntry?.ClientId ?? 0)
            .WithCustomInt3(dbEntry?.ProductId ?? 0)
            .WithCustomInt4(dbEntry?.ProjectId ?? 0);

          if (dbEntry == null || dbEntry.UserId != userId)
          {
            // TODO: [HANDLE] (TrackedTimeService.PauseTimer) Handle this
            return false;
          }

          using (builder.WithCustomTiming2())
          {
            builder.IncrementQueryCount();
            return await _timeRepo.PauseTimer(rawTimerId, state, notes) > 0;
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

    public async Task<bool> ResumeTimer(int userId, long rawTimerId)
    {
      // TODO: [TESTS] (TrackedTimeService.ResumeTimer) Add tests
      var builder = new ServiceMetricBuilder(nameof(TrackedTimeService), nameof(ResumeTimer))
        .WithCategory(MetricCategory.TrackedTime, MetricSubCategory.Update)
        .WithCustomInt1(userId);

      try
      {
        using (builder.WithTiming())
        {
          TrackedTimeEntity parentEntry;
          using (builder.WithCustomTiming1())
          {
            builder.IncrementQueryCount();
            parentEntry = await _timeRepo.GetByRawTimerId(rawTimerId);
            builder.CountResult(parentEntry);
          }

          builder
            .WithCustomInt2(parentEntry?.ClientId ?? 0)
            .WithCustomInt3(parentEntry?.ProductId ?? 0)
            .WithCustomInt4(parentEntry?.ProjectId ?? 0);

          if (parentEntry == null || parentEntry.UserId != userId)
          {
            // TODO: [HANDLE] (TrackedTimeService.ResumeTimer) Handle this
            return false;
          }

          var resumedEntity = CreateResumedTimer(parentEntry);
          using (builder.WithCustomTiming2())
          {
            builder.IncrementQueryCount();
            if (await _timeRepo.SpawnResumedTimer(resumedEntity) == 0)
              return false;
          }

          using (builder.WithCustomTiming3())
          {
            builder.IncrementQueryCount();
            return await _timeRepo.FlagAsResumed(rawTimerId) != 0;
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

    public async Task<bool> StopTimer(int userId, long rawTimerId)
    {
      // TODO: [TESTS] (TrackedTimeService.StopTimer) Add tests
      var builder = new ServiceMetricBuilder(nameof(TrackedTimeService), nameof(StopTimer))
        .WithCategory(MetricCategory.TrackedTime, MetricSubCategory.Update)
        .WithCustomInt1(userId);

      try
      {
        using (builder.WithTiming())
        {
          TrackedTimeEntity dbTimer;
          using (builder.WithCustomTiming1())
          {
            builder.IncrementQueryCount();
            dbTimer = await _timeRepo.GetByRawTimerId(rawTimerId);
            builder.CountResult(dbTimer);
          }

          builder
            .WithCustomInt2(dbTimer?.ClientId ?? 0)
            .WithCustomInt3(dbTimer?.ProductId ?? 0)
            .WithCustomInt4(dbTimer?.ProjectId ?? 0);

          if (dbTimer == null || dbTimer.UserId != userId)
          {
            // TODO: [HANDLE] (TrackedTimeService.StopTimer) Handle this
            return false;
          }

          using (builder.WithCustomTiming2())
          {
            builder.IncrementQueryCount();
            if (await _timeRepo.StopTimer(rawTimerId) == 0)
              return false;
          }

          using (builder.WithCustomTiming3())
          {
            builder.IncrementQueryCount();
            return await _timeRepo.CompleteTimerSet(dbTimer.RootEntryId) != 0;
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

    public async Task<List<TrackedTimeDto>> GetTimerSeries(int userId, long rootTimerId)
    {
      // TODO: [TESTS] (TrackedTimeService.GetTimerSeries) Add tests
      var builder = new ServiceMetricBuilder(nameof(TrackedTimeService), nameof(GetTimerSeries))
        .WithCategory(MetricCategory.TrackedTime, MetricSubCategory.GetList)
        .WithCustomInt1(userId);

      try
      {
        using (builder.WithTiming())
        {
          List<TrackedTimeEntity> dbEntries;
          using (builder.WithCustomTiming1())
          {
            builder.IncrementQueryCount();
            dbEntries = await _timeRepo.GetTimerSeries(rootTimerId);
            builder.WithResultsCount(dbEntries?.Count ?? 0);
          }

          if (dbEntries == null || dbEntries.Count == 0)
          {
            // TODO: [HANDLE] (TrackedTimeService.GetTimerSeries) Handle this
            return new List<TrackedTimeDto>();
          }

          builder
            .WithCustomInt2(dbEntries.FirstOrDefault()?.ClientId ?? 0)
            .WithCustomInt2(dbEntries.FirstOrDefault()?.ProductId ?? 0)
            .WithCustomInt2(dbEntries.FirstOrDefault()?.ProjectId ?? 0);

          // ReSharper disable once ConvertIfStatementToReturnStatement
          if (dbEntries.First().UserId != userId)
          {
            // TODO: [HANDLE] (TrackedTimeService.GetTimerSeries) Handle this
            return new List<TrackedTimeDto>();
          }

          return dbEntries.AsQueryable().Select(TrackedTimeDto.Projection).ToList();
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

    public async Task<bool> UpdateNotes(int userId, long rawTimerId, string notes)
    {
      // TODO: [TESTS] (TrackedTimeService.UpdateNotes) Add tests
      var builder = new ServiceMetricBuilder(nameof(TrackedTimeService), nameof(UpdateNotes))
        .WithCategory(MetricCategory.TrackedTime, MetricSubCategory.Update)
        .WithCustomInt1(userId);

      try
      {
        using (builder.WithTiming())
        {
          TrackedTimeEntity dbEntry;
          using (builder.WithCustomTiming1())
          {
            builder.IncrementQueryCount();
            dbEntry = await _timeRepo.GetByRawTimerId(rawTimerId);
            builder.CountResult(dbEntry);
          }

          if (dbEntry == null)
            return false;

          builder
            .WithCustomInt2(dbEntry.ClientId)
            .WithCustomInt3(dbEntry.ProductId)
            .WithCustomInt4(dbEntry.ProjectId);

          if (dbEntry.UserId != userId)
          {
            // TODO: [HANDLE] (TrackedTimeService.UpdateNotes) Handle this
            return false;
          }

          using (builder.WithCustomTiming2())
          {
            builder.IncrementQueryCount();
            return await _timeRepo.UpdateNotes(rawTimerId, notes) != 0;
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

    public async Task<bool> UpdateTimerDuration(int userId, TrackedTimeDto timerDto)
    {
      // TODO: [TESTS] (TrackedTimeService.UpdateTimerDuration) Add tests
      var builder = new ServiceMetricBuilder(nameof(TrackedTimeService), nameof(UpdateTimerDuration))
        .WithCategory(MetricCategory.TrackedTime, MetricSubCategory.Update)
        .WithCustomInt1(userId)
        .WithCustomInt2(timerDto.ClientId)
        .WithCustomInt3(timerDto.ProductId)
        .WithCustomInt4(timerDto.ProjectId);

      try
      {
        using (builder.WithTiming())
        {
          TrackedTimeEntity dbTimer;
          using (builder.WithCustomTiming1())
          {
            builder.IncrementQueryCount();
            dbTimer = await _timeRepo.GetByRawTimerId(timerDto.EntryId);
            builder.CountResult(dbTimer);
          }

          if (dbTimer == null)
            return false;

          if (dbTimer.UserId != userId)
          {
            // TODO: [HANDLE] (TrackedTimeService.UpdateTimerDuration) Handle this
            return false;
          }

          var timerEntity = timerDto.AsEntity();
          using (builder.WithCustomTiming2())
          {
            builder.IncrementQueryCount();
            return await _timeRepo.UpdateTimerDuration(timerEntity) != 0;
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

    public async Task<bool> ResumeSingleTimer(int userId, long rawTimerId)
    {
      // TODO: [TESTS] (TrackedTimeService.ResumeSingleTimer) Add tests
      var builder = new ServiceMetricBuilder(nameof(TrackedTimeService), nameof(ResumeSingleTimer))
        .WithCategory(MetricCategory.TrackedTime, MetricSubCategory.Update)
        .WithCustomInt1(userId);

      try
      {
        using (builder.WithTiming())
        {
          TrackedTimeEntity parentTimer;
          using (builder.WithCustomTiming1())
          {
            builder.IncrementQueryCount();
            parentTimer = await _timeRepo.GetByRawTimerId(rawTimerId);
            builder.CountResult(parentTimer);
          }

          if (parentTimer == null)
            return false;

          builder
            .WithCustomInt2(parentTimer.ClientId)
            .WithCustomInt3(parentTimer.ProductId)
            .WithCustomInt4(parentTimer.ProjectId);

          if (parentTimer.UserId != userId)
          {
            // TODO: [HANDLE] (TrackedTimeService.ResumeSingleTimer) Handle this
            return false;
          }

          using (builder.WithCustomTiming2())
          {
            builder.IncrementQueryCount();
            var runningTimers = await _timeRepo.GetRunningTimers(userId);

            if (runningTimers.Count > 0)
            {
              foreach (var timer in runningTimers)
              {
                builder.IncrementQueryCount();
                await _timeRepo.PauseTimer(timer.EntryId, EntryRunningState.Paused, "user-paused (auto)");
              }
            }
          }

          // Resume the given timer
          using (builder.WithCustomTiming3())
          {
            builder.IncrementQueryCount();
            return await ResumeTimer(userId, rawTimerId);
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
    private static TrackedTimeEntity CreateResumedTimer(TrackedTimeEntity parentTimer)
    {
      // TODO: [TESTS] (TrackedTimeService.CreateResumedTimer) Add tests
      return new TrackedTimeEntity
      {
        ParentEntryId = parentTimer.EntryId,
        RootEntryId = parentTimer.RootEntryId,
        ClientId = parentTimer.ClientId,
        ProductId = parentTimer.ProductId,
        ProjectId = parentTimer.ProjectId,
        UserId = parentTimer.UserId,
        Running = true,
        EntryState = EntryRunningState.Running,
        Completed = false,
        Notes = "user-resumed"
      };
    }
  }
}
