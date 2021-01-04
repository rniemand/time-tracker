using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Rn.NetCore.Common.Logging;
using TimeTracker.Core.Services;

namespace TimeTracker.Core.Jobs
{
  public class SweepLongRunningTimers
  {
    private readonly ILoggerAdapter<SweepLongRunningTimers> _logger;
    private readonly IOptionsService _optionService;

    public SweepLongRunningTimers(IServiceProvider services)
    {
      _logger = services.GetService<ILoggerAdapter<SweepLongRunningTimers>>();
      _optionService = services.GetService<IOptionsService>();
    }

    public async Task Run()
    {
      _logger.Info("WOOOO");



      await Task.CompletedTask;
    }
  }
}
