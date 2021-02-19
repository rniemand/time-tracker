using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Rn.NetCore.Common.Logging;
using Rn.NetCore.Common.Metrics;
using Rn.NetCore.Common.Metrics.Builders;
using TimeTracker.Core.Database.Entities;
using TimeTracker.Core.Database.Repos;
using TimeTracker.Core.Enums;
using TimeTracker.Core.Models;

namespace TimeTracker.Core.Services
{
  public interface IOptionsService
  {
    Task<RawOptions> GenerateOptions(string category, int userId);
  }

  public class OptionsService : IOptionsService
  {
    private readonly ILoggerAdapter<OptionsService> _logger;
    private readonly IMetricService _metrics;
    private readonly IOptionRepo _optionRepo;

    public OptionsService(
      ILoggerAdapter<OptionsService> logger,
      IMetricService metrics,
      IOptionRepo optionRepo)
    {
      _optionRepo = optionRepo;
      _logger = logger;
      _metrics = metrics;
    }


    // Interface methods
    public async Task<RawOptions> GenerateOptions(string category, int userId)
    {
      // TODO: [TESTS] (OptionsService.GetRawOptionsForCategory) Add tests
      var builder = new ServiceMetricBuilder(nameof(OptionsService), nameof(GenerateOptions))
        .WithCategory(MetricCategory.Option, MetricSubCategory.Generate)
        .WithCustomInt1(userId)
        .WithCustomTag1(category);

      var generated = new RawOptions(category);

      try
      {
        using (builder.WithTiming())
        {
          List<OptionEntity> dbOptions;
          using (builder.WithCustomTiming1())
          {
            builder.IncrementQueryCount();
            dbOptions = await _optionRepo.GetRawOptionsForCategory(category, userId);
            builder.WithResultsCount(dbOptions.Count);
          }

          foreach (var dbOption in dbOptions)
          {
            generated.AddOption(dbOption);
          }
        }
      }
      catch (Exception ex)
      {
        _logger.LogUnexpectedException(ex);
        builder.WithException(ex);
      }
      finally
      {
        await _metrics.SubmitPointAsync(builder.Build());
      }

      return generated;
    }
  }
}
