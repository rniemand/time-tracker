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
    private readonly IRawTrackedTimeRepo _rawTrackedTimeRepo;

    public TrackedTimeService(
      ILoggerAdapter<TrackedTimeService> logger,
      IRawTrackedTimeRepo rawTrackedTimeRepo)
    {
      _logger = logger;
      _rawTrackedTimeRepo = rawTrackedTimeRepo;
    }
  }
}
