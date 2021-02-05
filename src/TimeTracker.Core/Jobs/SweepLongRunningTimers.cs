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


    // Constructor and Run() logic
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
          foreach (var keyValueEntity in await GetUsersWithRunningTimers(builder))
          {
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


    // Run() supporting methods
    private async Task<List<KeyValueEntity<int, string>>> GetUsersWithRunningTimers(CronMetricBuilder builder)
    {
      // TODO: [TESTS] (SweepLongRunningTimers.GetUsersWithRunningTimers) Add tests
      List<KeyValueEntity<int, string>> users;

      using (builder.WithCustomTiming1())
      {
        builder.IncrementQueryCount();
        users = await _timerRepo.GetUsersWithRunningTimers();
        builder.WithResultsCount(users.Count);
      }

      return users;
    }

    private async Task ProcessUserTimers(int userId)
    {
      // TODO: [TESTS] (SweepLongRunningTimers.ProcessUserTimers) Add tests
      // Load options and check if we have any timers that need to be stopped
      var options = await _optionService.GenerateOptions("RunningTimers", userId);
      var maxRunTimeSec = options.GetIntOption("MaxLength.Min", 60 * 5) * 60;
      var timers = await _timerRepo.GetLongRunningTimers(userId, maxRunTimeSec);
      if (timers.Count == 0)
        return;

      // Log and stop running timers
      _logger.Debug("Found {count} long running timer(s) for userId: {userId}",
        timers.Count,
        userId
      );

      foreach (var timer in timers)
      {
        await _timerService.PauseTimer(userId, timer.EntryId, TimerNote.AutoPaused);
      }
    }
  }
}
