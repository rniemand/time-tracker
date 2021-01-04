using System;
using Microsoft.Extensions.DependencyInjection;
using Rn.NetCore.Common.Logging;

namespace TimeTracker.Core.Jobs
{
  public class TestHangfireJob
  {
    private readonly ILoggerAdapter<TestHangfireJob> _logger;

    public TestHangfireJob(IServiceProvider services)
    {
      _logger = services.GetService<ILoggerAdapter<TestHangfireJob>>();
    }

    public void Run()
    {
      _logger.Info("Running....");
    }
  }
}
