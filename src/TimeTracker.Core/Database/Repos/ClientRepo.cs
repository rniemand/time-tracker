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
  public interface IClientRepo
  {
    Task<List<ClientEntity>> GetAll(int userId);
    Task<ClientEntity> GetByName(int userId, string clientName);
    Task<int> Add(ClientEntity entity);
    Task<int> Update(ClientEntity entity);
    Task<ClientEntity> GetById(int clientId);
  }

  public class ClientRepo : BaseRepo<ClientRepo>, IClientRepo
  {
    private readonly IClientQueries _queries;

    public ClientRepo(
      ILoggerAdapter<ClientRepo> logger,
      IDbHelper dbHelper,
      IMetricService metricService,
      IClientQueries queries)
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

    public async Task<ClientEntity> GetByName(int userId, string clientName)
    {
      // TODO: [TESTS] (ClientRepo.GetByName) Add tests
      return await GetSingle<ClientEntity>(
        nameof(GetByName),
        _queries.GetByName(),
        new
        {
          UserId = userId,
          ClientName = clientName
        }
      );
    }

    public async Task<int> Add(ClientEntity entity)
    {
      // TODO: [TESTS] (ClientRepo.Add) Add tests
      return await ExecuteAsync(nameof(Add), _queries.Add(), entity);
    }

    public async Task<int> Update(ClientEntity entity)
    {
      // TODO: [TESTS] (ClientRepo.Update) Add tests
      return await ExecuteAsync(nameof(Update), _queries.Update(), entity);
    }

    public async Task<ClientEntity> GetById(int clientId)
    {
      // TODO: [TESTS] (ClientRepo.GetById) Add tests
      return await GetSingle<ClientEntity>(
        nameof(GetById),
        _queries.GetById(),
        new { ClientId = clientId }
      );
    }
  }
}
