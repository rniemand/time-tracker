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
    Task<List<ProductEntity>> GetAll(int clientId);
    Task<int> Add(ProductEntity entity);
    Task<ProductEntity> GetById(int productId);
    Task<int> Update(ProductEntity entity);
    Task<ProductEntity> GetByName(int clientId, string productName);
  }

  public class ProductRepo : BaseRepo<ProductRepo>, IProductRepo
  {
    private readonly IProductQueries _queries;

    public ProductRepo(
      ILoggerAdapter<ProductRepo> logger,
      IDbHelper dbHelper,
      IMetricService metricService,
      IProductQueries queries)
        : base(logger, dbHelper, metricService, nameof(ProductRepo), TargetDB.TimeTracker)
    {
      _queries = queries;
    }

    public async Task<List<ProductEntity>> GetAll(int clientId)
    {
      // TODO: [TESTS] (ProductRepo.GetAll) Add tests
      return await GetList<ProductEntity>(
        nameof(GetAll),
        _queries.GetAll(),
        new { ClientId = clientId }
      );
    }

    public async Task<int> Add(ProductEntity entity)
    {
      // TODO: [TESTS] (ProductRepo.Add) Add tests
      return await ExecuteAsync(nameof(Add), _queries.Add(), entity);
    }

    public async Task<ProductEntity> GetById(int productId)
    {
      // TODO: [TESTS] (ProductRepo.GetById) Add tests
      return await GetSingle<ProductEntity>(
        nameof(GetById),
        _queries.GetById(),
        new { ProductId = productId }
      );
    }

    public async Task<int> Update(ProductEntity entity)
    {
      // TODO: [TESTS] (ProductRepo.Update) Add tests
      return await ExecuteAsync(nameof(Update), _queries.Update(), entity);
    }

    public async Task<ProductEntity> GetByName(int clientId, string productName)
    {
      // TODO: [TESTS] (ProductRepo.GetByName) Add tests
      return await GetSingle<ProductEntity>(
        nameof(GetByName),
        _queries.GetByName(),
        new
        {
          ClientId = clientId,
          ProductName = productName
        }
      );
    }
  }
}
