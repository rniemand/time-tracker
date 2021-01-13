using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Rn.NetCore.Common.Logging;
using Rn.NetCore.Common.Metrics;
using Rn.NetCore.Common.Metrics.Builders;
using TimeTracker.Core.Database.Entities;
using TimeTracker.Core.Database.Repos;
using TimeTracker.Core.Enums;
using TimeTracker.Core.Models;
using TimeTracker.Core.Services;

namespace TimeTracker.Core.Jobs
{
  public class SweepLongRunningTimers
  {
    private readonly ILoggerAdapter<SweepLongRunningTimers> _logger;
    private readonly ITimerRepo _timerRepo;
    private readonly IOptionsService _optionService;
    private readonly ITimerService _timerService;
    private readonly IMetricService _metrics;

    public SweepLongRunningTimers(IServiceProvider services)
    {
      _logger = services.GetRequiredService<ILoggerAdapter<SweepLongRunningTimers>>();
      _timerRepo = services.GetRequiredService<ITimerRepo>();
      _optionService = services.GetRequiredService<IOptionsService>();
      _timerService = services.GetRequiredService<ITimerService>();
      _metrics = services.GetRequiredService<IMetricService>();
    }

    public async Task Run()
    {
      // TODO: [TESTS] (SweepLongRunningTimers.Run) Add tests
      var builder = new CronMetricBuilder(nameof(SweepLongRunningTimers), nameof(Run))
        .WithCategory(MetricCategory.TrackedTime, MetricSubCategory.Update);

      try
      {
        using (builder.WithTiming())
        {
          List<KeyValueEntity<int, string>> users;
          using (builder.WithCustomTiming1())
          {
            builder.IncrementQueryCount();
            users = await _timerRepo.GetUsersWithRunningTimers();
            builder.WithResultsCount(users.Count);
          }

          foreach (var keyValueEntity in users)
          {
            builder.IncrementQueryCount();
            await ProcessUserTimers(keyValueEntity.Key);
          }
        }
      }
      catch (Exception ex)
      {
        _logger.LogUnexpectedException(ex);
        builder.WithException(ex);
      }
      finally
      {
        await _metrics.SubmitPointAsync(builder.Build());
      }
    }

    private async Task ProcessUserTimers(int userId)
    {
      // TODO: [TESTS] (SweepLongRunningTimers.ProcessUserTimers) Add tests
      var builder = new CronMetricBuilder(nameof(SweepLongRunningTimers), nameof(ProcessUserTimers))
        .WithCategory(MetricCategory.TrackedTime, MetricSubCategory.Update)
        .WithUserId(userId);

      try
      {
        using (builder.WithTiming())
        {
          RawOptions options;
          using (builder.WithCustomTiming1())
          {
            builder.IncrementQueryCount();
            options = await _optionService.GenerateOptions("RunningTimers", userId);
          }

          var maxRunTimeSec = options.GetIntOption("MaxLength.Min", 60 * 5) * 60;
          List<TimerEntity> timers;
          using (builder.WithCustomTiming2())
          {
            builder.IncrementQueryCount();
            timers = await _timerRepo.GetLongRunningTimers(userId, maxRunTimeSec);
          }

          const TimerState endReason = TimerState.CronPaused;
          const string endString = "cron-paused";

          foreach (var timer in timers)
          {
            builder.IncrementQueryCount();

            if (await _timerService.PauseTimer(userId, timer.EntryId, endReason, endString))
              builder.IncrementResultsCount();
          }
        }
      }
      catch (Exception ex)
      {
        _logger.LogUnexpectedException(ex);
        builder.WithException(ex);
      }
      finally
      {
        await _metrics.SubmitPointAsync(builder.Build());
      }
    }
  }
}
