using System.Collections.Generic;
using System.Threading.Tasks;
using Rn.NetCore.Common.Logging;
using Rn.NetCore.Common.Metrics;
using Rn.NetCore.DbCommon;
using TimeTracker.Core.Database.Entities;
using TimeTracker.Core.Database.Queries;

namespace TimeTracker.Core.Database.Repos
{
  public interface IProductRepo
  {
    Task<List<ProductEntity>> GetAll(int userId, int clientId);
  }

  public class ProductRepo : BaseRepo<ProductRepo>, IProductRepo
  {
    private readonly IProductRepoQueries _queries;

    public ProductRepo(
      ILoggerAdapter<ProductRepo> logger,
      IDbHelper dbHelper,
      IMetricService metricService,
      IProductRepoQueries queries) 
        : base(logger, dbHelper, metricService, nameof(ProductRepo), TargetDB.TimeTracker)
    {
      _queries = queries;
    }

    public async Task<List<ProductEntity>> GetAll(int userId, int clientId)
    {
      // TODO: [TESTS] (ProductRepo.GetAll) Add tests
      return await GetList<ProductEntity>(
        nameof(GetAll),
        _queries.GetAll(),
        new
        {
          UserId = userId,
          ClientId = clientId
        }
      );
    }
  }
}
