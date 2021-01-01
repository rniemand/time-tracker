using Rn.NetCore.Common.Logging;
using TimeTracker.Core.Database.Repos;

namespace TimeTracker.Core.Services
{
  public interface ITrackedTimeService
  {
  }

  public class TrackedTimeService : ITrackedTimeService
  {
    private readonly ILoggerAdapter<TrackedTimeService> _logger;
    private readonly ITrackedTimeRepo _trackedTimeRepo;

    public TrackedTimeService(
      ILoggerAdapter<TrackedTimeService> logger,
      ITrackedTimeRepo trackedTimeRepo)
    {
      _logger = logger;
      _trackedTimeRepo = trackedTimeRepo;
    }
  }
}
