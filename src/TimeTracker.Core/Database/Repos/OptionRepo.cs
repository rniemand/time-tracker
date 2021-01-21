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
  public interface IOptionRepo
  {
    Task<OptionEntity> GetRawOption(string category, string key, int userId);
    Task<List<OptionEntity>> GetRawOptionsForCategory(string category, int userId);
  }

  public class OptionRepo : BaseRepo<OptionRepo>, IOptionRepo
  {
    private readonly IOptionQueries _queries;

    public OptionRepo(
      ILoggerAdapter<OptionRepo> logger,
      IDbHelper dbHelper,
      IMetricService metricService,
      IOptionQueries queries)
        : base(logger, dbHelper, metricService, nameof(OptionRepo), TargetDB.TimeTracker)
    {
      _queries = queries;
    }

    public async Task<OptionEntity> GetRawOption(string category, string key, int userId)
    {
      // TODO: [TESTS] (OptionRepo.GetRawOption) Add tests
      return await GetSingle<OptionEntity>(
        nameof(GetRawOption),
        _queries.GetRawOption(),
        new
        {
          OptionCategory = category,
          OptionKey = key,
          UserId = userId
        }
      );
    }

    public async Task<List<OptionEntity>> GetRawOptionsForCategory(string category, int userId)
    {
      // TODO: [TESTS] (OptionRepo.GetRawOptionsForCategory) Add tests
      return await GetList<OptionEntity>(
        nameof(GetRawOptionsForCategory),
        _queries.GetRawOptionsForCategory(),
        new
        {
          OptionCategory = category,
          UserId = userId
        }
      );
    }
  }
}
