using System.Collections.Generic;
using System.Threading.Tasks;
using Rn.NetCore.Common.Logging;
using Rn.NetCore.Common.Metrics;
using Rn.NetCore.DbCommon;
using TimeTracker.Core.Database.Entities;
using TimeTracker.Core.Database.Queries;

namespace TimeTracker.Core.Database.Repos
{
  public interface IProjectRepo
  {
    Task<List<ProjectEntity>> GetAllForProduct(int productId);
    Task<ProjectEntity> GetById(int projectId);
    Task<ProjectEntity> GetByName(int productId, string projectName);
    Task<int> Add(ProjectEntity entity);
    Task<int> Update(ProjectEntity entity);
  }

  public class ProjectRepo : BaseRepo<ProjectRepo>, IProjectRepo
  {
    private readonly IProjectQueries _queries;

    public ProjectRepo(
      ILoggerAdapter<ProjectRepo> logger,
      IDbHelper dbHelper,
      IMetricService metricService,
      IProjectQueries queries)
        : base(logger, dbHelper, metricService, nameof(ProjectRepo), TargetDB.TimeTracker)
    {
      _queries = queries;
    }

    public async Task<List<ProjectEntity>> GetAllForProduct(int productId)
    {
      // TODO: [TESTS] (ProjectRepo.GetAllForProduct) Add tests
      return await GetList<ProjectEntity>(
        nameof(GetAllForProduct),
        _queries.GetAllForProduct(),
        new { ProductId = productId }
      );
    }

    public async Task<ProjectEntity> GetById(int projectId)
    {
      // TODO: [TESTS] (ProjectRepo.GetById) Add tests
      return await GetSingle<ProjectEntity>(
        nameof(GetById),
        _queries.GetById(),
        new { ProjectId = projectId }
      );
    }

    public async Task<ProjectEntity> GetByName(int productId, string projectName)
    {
      // TODO: [TESTS] (ProjectRepo.GetByName) Add tests
      return await GetSingle<ProjectEntity>(
        nameof(GetByName),
        _queries.GetByName(),
        new
        {
          ProductId = productId,
          ProjectName = projectName
        }
      );
    }

    public async Task<int> Add(ProjectEntity entity)
    {
      // TODO: [TESTS] (ProjectRepo.Add) Add tests
      return await ExecuteAsync(nameof(Add), _queries.Add(), entity);
    }

    public async Task<int> Update(ProjectEntity entity)
    {
      // TODO: [TESTS] (ProjectRepo.Update) Add tests
      return await ExecuteAsync(nameof(Update), _queries.Update(), entity);
    }
  }
}
