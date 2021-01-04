using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Rn.NetCore.Common.Logging;
using TimeTracker.Core.Database.Repos;
using TimeTracker.Core.Services;

namespace TimeTracker.Core.Jobs
{
  public class SweepLongRunningTimers
  {
    private readonly ILoggerAdapter<SweepLongRunningTimers> _logger;
    private readonly IRawTimersRepo _rawTimersRepo;
    private readonly IOptionsService _optionService;
    private readonly IRawTimerService _timerService;

    public SweepLongRunningTimers(IServiceProvider services)
    {
      _logger = services.GetService<ILoggerAdapter<SweepLongRunningTimers>>();
      _rawTimersRepo = services.GetService<IRawTimersRepo>();
      _optionService = services.GetService<IOptionsService>();
      _timerService = services.GetService<IRawTimerService>();
    }

    public async Task Run()
    {
      var users = await _rawTimersRepo.GetUsersWithRunningTimers();

      foreach (var keyValueEntity in users)
      {
        var userId = keyValueEntity.Key;
        var options = await _optionService.GenerateOptions("RunningTimers", userId);
        var intOption = options.GetIntOption("MaxLength.Min", 60 * 5) * 60;
        var timers = await _rawTimersRepo.GetLongRunningTimers(userId, intOption);


        foreach (var timer in timers)
        {
          await _timerService.PauseTimer(userId, timer.RawTimerId, "auto-paused");

        }


      }

      await Task.CompletedTask;
    }
  }
}
