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
  public interface IRawTimerService
  {
    Task<bool> StartNew(int userId, RawTimerDto timerDto);
    Task<List<RawTimerDto>> GetActiveTimers(int userId);
    Task<bool> PauseTimer(int userId, long rawTimerId, EntryRunningState state, string notes);
    Task<bool> ResumeTimer(int userId, long rawTimerId);
    Task<bool> StopTimer(int userId, long rawTimerId);
    Task<List<RawTimerDto>> GetTimerSeries(int userId, long rootTimerId);
    Task<bool> UpdateNotes(int userId, long rawTimerId, string notes);
    Task<bool> UpdateTimerDuration(int userId, RawTimerDto timerDto);
    Task<bool> ResumeSingleTimer(int userId, long rawTimerId);
  }

  public class RawTimerService : IRawTimerService
  {
    private readonly ILoggerAdapter<RawTimerService> _logger;
    private readonly IMetricService _metrics;
    private readonly IRawTimersRepo _rawTimersRepo;

    public RawTimerService(
      ILoggerAdapter<RawTimerService> logger,
      IMetricService metrics,
      IRawTimersRepo rawTimersRepo)
    {
      _logger = logger;
      _metrics = metrics;
      _rawTimersRepo = rawTimersRepo;
    }

    public async Task<bool> StartNew(int userId, RawTimerDto timerDto)
    {
      // TODO: [TESTS] (RawTimerService.StartNew) Add tests
      var builder = new ServiceMetricBuilder(nameof(RawTimerService), nameof(StartNew))
        .WithCategory(MetricCategory.RawTimer, MetricSubCategory.Add)
        .WithCustomInt1(userId)
        .WithCustomInt2(timerDto.ClientId)
        .WithCustomInt3(timerDto.ProductId)
        .WithCustomInt4(timerDto.ProjectId)
        .WithCustomTag1("started");

      try
      {
        using (builder.WithTiming())
        {
          var timerEntity = timerDto.AsEntity(userId);

          // Check if we can resume an existing timer
          RawTimerEntity existingTimer;
          using (builder.WithCustomTiming1())
          {
            builder.IncrementQueryCount();
            existingTimer = await _rawTimersRepo.SearchExistingTimer(timerEntity);
            builder.CountResult(existingTimer);
          }

          if (existingTimer != null)
          {
            using (builder.WithCustomTiming2())
            {
              builder.IncrementQueryCount().WithCustomTag1("resumed");
              return await ResumeTimer(userId, existingTimer.RawTimerId);
            }
          }

          // Create a new timer
          using (builder.WithCustomTiming2())
          {
            builder.IncrementQueryCount();
            if (await _rawTimersRepo.StartNew(timerEntity) <= 0)
              return false;
          }

          // Ensure that the "RootTimerId" is set to itself (top level timer)
          RawTimerEntity dbTimerEntity;
          using (builder.WithCustomTiming3())
          {
            builder.IncrementQueryCount();
            dbTimerEntity = await _rawTimersRepo.GetCurrentEntry(timerEntity);
          }

          var rootTimerId = dbTimerEntity.RawTimerId;
          using (builder.WithCustomTiming4())
          {
            builder.IncrementQueryCount();
            return await _rawTimersRepo.SetRootTimerId(rootTimerId, rootTimerId) != 0;
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
        await _metrics.SubmitPointAsync(builder);
      }
    }

    public async Task<List<RawTimerDto>> GetActiveTimers(int userId)
    {
      // TODO: [TESTS] (RawTimerService.GetActiveTimers) Add tests
      var builder = new ServiceMetricBuilder(nameof(RawTimerService), nameof(GetActiveTimers))
        .WithCategory(MetricCategory.RawTimer, MetricSubCategory.GetList)
        .WithCustomInt1(userId);

      try
      {
        using (builder.WithTiming())
        {
          List<RawTimerEntity> dbEntries;
          using (builder.WithCustomTiming1())
          {
            builder.IncrementQueryCount();
            dbEntries = await _rawTimersRepo.GetActiveTimers(userId);
            builder.WithResultCount(dbEntries.Count);
          }

          return dbEntries.AsQueryable().Select(RawTimerDto.Projection).ToList();
        }
      }
      catch (Exception ex)
      {
        _logger.LogUnexpectedException(ex);
        builder.WithException(ex);
        return new List<RawTimerDto>();
      }
      finally
      {
        await _metrics.SubmitPointAsync(builder);
      }
    }

    public async Task<bool> PauseTimer(int userId, long rawTimerId, EntryRunningState state, string notes)
    {
      // TODO: [TESTS] (RawTimerService.PauseTimer) Add tests
      var builder = new ServiceMetricBuilder(nameof(RawTimerService), nameof(PauseTimer))
        .WithCategory(MetricCategory.RawTimer, MetricSubCategory.Update)
        .WithCustomInt1(userId)
        .WithCustomDouble1(rawTimerId);

      try
      {
        using (builder.WithTiming())
        {
          RawTimerEntity dbEntry;
          using (builder.WithCustomTiming1())
          {
            builder.IncrementQueryCount();
            dbEntry = await _rawTimersRepo.GetByRawTimerId(rawTimerId);
            builder.CountResult(dbEntry);
          }

          builder
            .WithCustomInt2(dbEntry?.ClientId ?? 0)
            .WithCustomInt3(dbEntry?.ProductId ?? 0)
            .WithCustomInt4(dbEntry?.ProjectId ?? 0);

          if (dbEntry == null || dbEntry.UserId != userId)
          {
            // TODO: [HANDLE] (RawTimerService.PauseTimer) Handle this
            return false;
          }

          using (builder.WithCustomTiming2())
          {
            builder.IncrementQueryCount();
            return await _rawTimersRepo.PauseTimer(rawTimerId, state, notes) > 0;
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
        await _metrics.SubmitPointAsync(builder);
      }
    }

    public async Task<bool> ResumeTimer(int userId, long rawTimerId)
    {
      // TODO: [TESTS] (RawTimerService.ResumeTimer) Add tests
      var builder = new ServiceMetricBuilder(nameof(RawTimerService), nameof(ResumeTimer))
        .WithCategory(MetricCategory.RawTimer, MetricSubCategory.Update)
        .WithCustomInt1(userId);

      try
      {
        using (builder.WithTiming())
        {
          RawTimerEntity parentEntry;
          using (builder.WithCustomTiming1())
          {
            builder.IncrementQueryCount();
            parentEntry = await _rawTimersRepo.GetByRawTimerId(rawTimerId);
            builder.CountResult(parentEntry);
          }

          builder
            .WithCustomInt2(parentEntry?.ClientId ?? 0)
            .WithCustomInt3(parentEntry?.ProductId ?? 0)
            .WithCustomInt4(parentEntry?.ProjectId ?? 0);

          if (parentEntry == null || parentEntry.UserId != userId)
          {
            // TODO: [HANDLE] (RawTimerService.ResumeTimer) Handle this
            return false;
          }

          var resumedEntity = CreateResumedTimer(parentEntry);
          using (builder.WithCustomTiming2())
          {
            builder.IncrementQueryCount();
            if (await _rawTimersRepo.SpawnResumedTimer(resumedEntity) == 0)
              return false;
          }

          using (builder.WithCustomTiming3())
          {
            builder.IncrementQueryCount();
            return await _rawTimersRepo.FlagAsResumed(rawTimerId) != 0;
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
        await _metrics.SubmitPointAsync(builder);
      }
    }

    public async Task<bool> StopTimer(int userId, long rawTimerId)
    {
      // TODO: [TESTS] (RawTimerService.StopTimer) Add tests
      var builder = new ServiceMetricBuilder(nameof(RawTimerService), nameof(StopTimer))
        .WithCategory(MetricCategory.RawTimer, MetricSubCategory.Update)
        .WithCustomInt1(userId);

      try
      {
        using (builder.WithTiming())
        {
          RawTimerEntity dbTimer;
          using (builder.WithCustomTiming1())
          {
            builder.IncrementQueryCount();
            dbTimer = await _rawTimersRepo.GetByRawTimerId(rawTimerId);
            builder.CountResult(dbTimer);
          }

          builder
            .WithCustomInt2(dbTimer?.ClientId ?? 0)
            .WithCustomInt3(dbTimer?.ProductId ?? 0)
            .WithCustomInt4(dbTimer?.ProjectId ?? 0);

          if (dbTimer == null || dbTimer.UserId != userId)
          {
            // TODO: [HANDLE] (RawTimerService.StopTimer) Handle this
            return false;
          }

          using (builder.WithCustomTiming2())
          {
            builder.IncrementQueryCount();
            if (await _rawTimersRepo.StopTimer(rawTimerId) == 0)
              return false;
          }

          using (builder.WithCustomTiming3())
          {
            builder.IncrementQueryCount();
            return await _rawTimersRepo.CompleteTimerSet(dbTimer.RootTimerId) != 0;
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
        await _metrics.SubmitPointAsync(builder);
      }
    }

    public async Task<List<RawTimerDto>> GetTimerSeries(int userId, long rootTimerId)
    {
      // TODO: [TESTS] (RawTimerService.GetTimerSeries) Add tests
      var builder = new ServiceMetricBuilder(nameof(RawTimerService), nameof(GetTimerSeries))
        .WithCategory(MetricCategory.RawTimer, MetricSubCategory.GetList)
        .WithCustomInt1(userId);

      try
      {
        using (builder.WithTiming())
        {
          List<RawTimerEntity> dbEntries;
          using (builder.WithCustomTiming1())
          {
            builder.IncrementQueryCount();
            dbEntries = await _rawTimersRepo.GetTimerSeries(rootTimerId);
            builder.WithResultCount(dbEntries?.Count ?? 0);
          }

          if (dbEntries == null || dbEntries.Count == 0)
          {
            // TODO: [HANDLE] (RawTimerService.GetTimerSeries) Handle this
            return new List<RawTimerDto>();
          }

          builder
            .WithCustomInt2(dbEntries.FirstOrDefault()?.ClientId ?? 0)
            .WithCustomInt2(dbEntries.FirstOrDefault()?.ProductId ?? 0)
            .WithCustomInt2(dbEntries.FirstOrDefault()?.ProjectId ?? 0);

          // ReSharper disable once ConvertIfStatementToReturnStatement
          if (dbEntries.First().UserId != userId)
          {
            // TODO: [HANDLE] (RawTimerService.GetTimerSeries) Handle this
            return new List<RawTimerDto>();
          }

          return dbEntries.AsQueryable().Select(RawTimerDto.Projection).ToList();
        }
      }
      catch (Exception ex)
      {
        _logger.LogUnexpectedException(ex);
        builder.WithException(ex);
        return new List<RawTimerDto>();
      }
      finally
      {
        await _metrics.SubmitPointAsync(builder);
      }
    }

    public async Task<bool> UpdateNotes(int userId, long rawTimerId, string notes)
    {
      // TODO: [TESTS] (RawTimerService.UpdateNotes) Add tests
      var builder = new ServiceMetricBuilder(nameof(RawTimerService), nameof(UpdateNotes))
        .WithCategory(MetricCategory.RawTimer, MetricSubCategory.Update)
        .WithCustomInt1(userId);

      try
      {
        using (builder.WithTiming())
        {
          RawTimerEntity dbEntry;
          using (builder.WithCustomTiming1())
          {
            builder.IncrementQueryCount();
            dbEntry = await _rawTimersRepo.GetByRawTimerId(rawTimerId);
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
            // TODO: [HANDLE] (RawTimerService.UpdateNotes) Handle this
            return false;
          }

          using (builder.WithCustomTiming2())
          {
            builder.IncrementQueryCount();
            return await _rawTimersRepo.UpdateNotes(rawTimerId, notes) != 0;
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
        await _metrics.SubmitPointAsync(builder);
      }
    }

    public async Task<bool> UpdateTimerDuration(int userId, RawTimerDto timerDto)
    {
      // TODO: [TESTS] (RawTimerService.UpdateTimerDuration) Add tests
      var builder = new ServiceMetricBuilder(nameof(RawTimerService), nameof(UpdateTimerDuration))
        .WithCategory(MetricCategory.RawTimer, MetricSubCategory.Update)
        .WithCustomInt1(userId)
        .WithCustomInt2(timerDto.ClientId)
        .WithCustomInt3(timerDto.ProductId)
        .WithCustomInt4(timerDto.ProjectId);

      try
      {
        using (builder.WithTiming())
        {
          RawTimerEntity dbTimer;
          using (builder.WithCustomTiming1())
          {
            builder.IncrementQueryCount();
            dbTimer = await _rawTimersRepo.GetByRawTimerId(timerDto.RawTimerId);
            builder.CountResult(dbTimer);
          }

          if (dbTimer == null)
            return false;

          if (dbTimer.UserId != userId)
          {
            // TODO: [HANDLE] (RawTimerService.UpdateTimerDuration) Handle this
            return false;
          }

          var timerEntity = timerDto.AsEntity();
          using (builder.WithCustomTiming2())
          {
            builder.IncrementQueryCount();
            return await _rawTimersRepo.UpdateTimerDuration(timerEntity) != 0;
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
        await _metrics.SubmitPointAsync(builder);
      }
    }

    public async Task<bool> ResumeSingleTimer(int userId, long rawTimerId)
    {
      // TODO: [TESTS] (RawTimerService.ResumeSingleTimer) Add tests
      var builder = new ServiceMetricBuilder(nameof(RawTimerService), nameof(ResumeSingleTimer))
        .WithCategory(MetricCategory.RawTimer, MetricSubCategory.Update)
        .WithCustomInt1(userId);

      try
      {
        using (builder.WithTiming())
        {
          RawTimerEntity parentTimer;
          using (builder.WithCustomTiming1())
          {
            builder.IncrementQueryCount();
            parentTimer = await _rawTimersRepo.GetByRawTimerId(rawTimerId);
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
            // TODO: [HANDLE] (RawTimerService.ResumeSingleTimer) Handle this
            return false;
          }

          using (builder.WithCustomTiming2())
          {
            builder.IncrementQueryCount();
            var runningTimers = await _rawTimersRepo.GetRunningTimers(userId);

            if (runningTimers.Count > 0)
            {
              foreach (var timer in runningTimers)
              {
                builder.IncrementQueryCount();
                await _rawTimersRepo.PauseTimer(timer.RawTimerId, EntryRunningState.Paused, "user-paused (auto)");
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
        await _metrics.SubmitPointAsync(builder);
      }
    }

    // Internal methods
    private static RawTimerEntity CreateResumedTimer(RawTimerEntity parentTimer)
    {
      // TODO: [TESTS] (RawTimerService.CreateResumedTimer) Add tests
      return new RawTimerEntity
      {
        ParentTimerId = parentTimer.RawTimerId,
        RootTimerId = parentTimer.RootTimerId,
        ClientId = parentTimer.ClientId,
        ProductId = parentTimer.ProductId,
        ProjectId = parentTimer.ProjectId,
        UserId = parentTimer.UserId,
        Running = true,
        EntryState = EntryRunningState.Running,
        Completed = false,
        TimerNotes = "user-resumed"
      };
    }
  }
}
