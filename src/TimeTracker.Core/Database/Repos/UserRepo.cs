using System.Collections.Generic;
using System.Threading.Tasks;
using Rn.NetCore.Common.Logging;
using Rn.NetCore.Common.Metrics;
using Rn.NetCore.DbCommon.Helpers;
using Rn.NetCore.DbCommon.Repos;
using TimeTracker.Core.Database.Entities;
using TimeTracker.Core.Database.Queries;

namespace TimeTracker.Core.Database.Repos
{
  public interface IUserRepo
  {
    Task<UserEntity> GetUsingCredentials(string username, string password);
    Task<int> UpdateLastLoginDate(int userId);
    Task<UserEntity> GetUserById(int userId);
    Task<List<UserEntity>> GetEnabledUsers();
  }

  public class UserRepo : BaseRepo<UserRepo>, IUserRepo
  {
    private readonly IUserQueries _queries;

    public UserRepo(
      ILoggerAdapter<UserRepo> logger,
      IDbHelper dbHelper,
      IMetricService metricService,
      IUserQueries queries)
      : base(logger, dbHelper, metricService, nameof(UserRepo), TargetDB.TimeTracker)
    {
      _queries = queries;
    }


    // Interface methods
    public async Task<UserEntity> GetUsingCredentials(string username, string password)
    {
      // TODO: [TESTS] (UserRepo.GetUsingCredentials) Add tests

      return await GetSingle<UserEntity>(
        nameof(GetUsingCredentials),
        _queries.GetUsingCredentials(),
        new
        {
          Username = username,
          Password = password
        }
      );
    }

    public async Task<int> UpdateLastLoginDate(int userId)
    {
      // TODO: [TESTS] (UserRepo.UpdateLastLoginDate) Add tests
      return await ExecuteAsync(
        nameof(UpdateLastLoginDate),
        _queries.UpdateLastLoginDate(),
        new { UserId = userId }
      );
    }

    public async Task<UserEntity> GetUserById(int userId)
    {
      // TODO: [TESTS] (UserRepo.GetUserById) Add tests
      return await GetSingle<UserEntity>(
        nameof(GetUserById),
        _queries.GetUserById(),
        new { UserId = userId }
      );
    }

    public async Task<List<UserEntity>> GetEnabledUsers()
    {
      // TODO: [TESTS] (UserRepo.GetEnabledUsers) Add tests
      return await GetList<UserEntity>(
        nameof(GetEnabledUsers),
        _queries.GetEnabledUsers()
      );
    }
  }
}
