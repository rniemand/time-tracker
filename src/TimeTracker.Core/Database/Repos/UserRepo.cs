using System.Threading.Tasks;
using Rn.NetCore.Common.Logging;
using Rn.NetCore.Common.Metrics;
using Rn.NetCore.DbCommon;
using TimeTracker.Core.Database.Entities;

namespace TimeTracker.Core.Database.Repos
{
  public interface IUserRepo
  {
    Task<UserEntity> Bob();
  }

  public class UserRepo : BaseRepo<UserRepo>, IUserRepo
  {
    public UserRepo(
      ILoggerAdapter<UserRepo> logger,
      IDbHelper dbHelper,
      IMetricService metricService)
      : base(logger, dbHelper, metricService, nameof(UserRepo), TargetDB.TimeTracker)
    {
    }

    public async Task<UserEntity> Bob()
    {
      return await GetSingle<UserEntity>(
        nameof(Bob),
        "SELECT * FROM `Users` LIMIT 1"
      );
    }
  }
}
