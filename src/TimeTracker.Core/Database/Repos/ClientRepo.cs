using System.Collections.Generic;
using System.Threading.Tasks;
using Rn.NetCore.Common.Logging;
using Rn.NetCore.Common.Metrics;
using Rn.NetCore.DbCommon;
using TimeTracker.Core.Database.Entities;
using TimeTracker.Core.Database.Queries;

namespace TimeTracker.Core.Database.Repos
{
  public interface IClientRepo
  {
    Task<List<ClientEntity>> GetAll(int userId);
  }

  public class ClientRepo : BaseRepo<ClientRepo>, IClientRepo
  {
    private readonly IClientRepoQueries _queries;

    public ClientRepo(
      ILoggerAdapter<ClientRepo> logger,
      IDbHelper dbHelper,
      IMetricService metricService,
      IClientRepoQueries queries)
        : base(logger, dbHelper, metricService, nameof(ClientRepo), TargetDB.TimeTracker)
    {
      _queries = queries;
    }

    public async Task<List<ClientEntity>> GetAll(int userId)
    {
      // TODO: [TESTS] (ClientRepo.GetAll) Add tests
      return await GetList<ClientEntity>(
        nameof(GetAll),
        _queries.GetAll(),
        new { UserId = userId }
      );
    }
  }
}
