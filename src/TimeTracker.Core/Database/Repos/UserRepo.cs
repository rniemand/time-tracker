﻿using System.Threading.Tasks;
using Rn.NetCore.Common.Logging;
using Rn.NetCore.Common.Metrics;
using Rn.NetCore.DbCommon;
using TimeTracker.Core.Database.Entities;
using TimeTracker.Core.Database.Queries;

namespace TimeTracker.Core.Database.Repos
{
  public interface IUserRepo
  {
    Task<UserEntity> GetUsingCredentials(string username, string password);
  }

  public class UserRepo : BaseRepo<UserRepo>, IUserRepo
  {
    private readonly IUserRepoQueries _queries;

    public UserRepo(
      ILoggerAdapter<UserRepo> logger,
      IDbHelper dbHelper,
      IMetricService metricService,
      IUserRepoQueries queries)
      : base(logger, dbHelper, metricService, nameof(UserRepo), TargetDB.TimeTracker)
    {
      _queries = queries;
    }

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
  }
}
