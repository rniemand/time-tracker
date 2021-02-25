using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Rn.NetCore.Common.Logging;
using Rn.NetCore.Common.Metrics;
using Rn.NetCore.Common.Metrics.Builders;
using TimeTracker.Core.Caches;
using TimeTracker.Core.Database.Entities;
using TimeTracker.Core.Database.Repos;
using TimeTracker.Core.Enums;
using TimeTracker.Core.Models;

namespace TimeTracker.Core.Services
{
  public interface IOptionsService
  {
    Task<RawOptions> GenerateOptions(string category, int userId);
    Task<OptionEntity> UpsertOption(OptionEntityCache cache, OptionEntity option);
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

    public async Task<OptionEntity> UpsertOption(OptionEntityCache cache, OptionEntity option)
    {
      // TODO: [TESTS] (OptionsService.UpsertOption) Add tests
      var category = option.OptionCategory;
      var optionKey = option.OptionKey;
      var userId = option.UserId;
      var dbOption = cache.GetCachedEntry(option)
                     ?? await _optionRepo.GetRawOption(category, optionKey, userId);

      if (dbOption == null)
      {
        if (await _optionRepo.Add(option) == 0)
          return null;
      }
      else
      {
        option.OptionId = dbOption.OptionId;
        option.UserId = dbOption.UserId;

        if (await _optionRepo.Update(option) == 0)
          return null;
      }

      dbOption = await _optionRepo.GetRawOption(category, optionKey, userId);
      cache.CacheEntry(dbOption);
      return dbOption;
    }
  }
}
