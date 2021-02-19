using System.Collections.Concurrent;
using System.Collections.Generic;
using Rn.NetCore.Common.Extensions;
using TimeTracker.Core.Database.Entities;

namespace TimeTracker.Core.Caches
{
  public class OptionEntityCache
  {
    private readonly ConcurrentDictionary<string, OptionEntity> _cache;


    // Constructor
    public OptionEntityCache()
    {
      // TODO: [TESTS] (OptionEntityCache) Add tests
      _cache = new ConcurrentDictionary<string, OptionEntity>();
    }


    // Public methods
    public void CacheEntry(OptionEntity option)
    {
      // TODO: [TESTS] (DbPolicyRuleCache.CacheEntry) Add tests
      if(option == null)
        return;

      _cache.AddOrUpdate(
        GenerateKey(option),
        option,
        (key, oldValue) => option
      );
    }

    public void CacheEntries(List<OptionEntity> options)
    {
      // TODO: [TESTS] (OptionEntityCache.CacheEntries) Add tests
      foreach (var option in options)
      {
        CacheEntry(option);
      }
    }

    public OptionEntity GetCachedEntry(string category, string key, int userId = 0)
    {
      // TODO: [TESTS] (OptionEntityCache.GetCachedEntry) Add tests
      var cacheKey = GenerateKey(category, key, userId);
      return !_cache.ContainsKey(cacheKey) ? null : _cache[cacheKey];
    }

    public OptionEntity GetCachedEntry(OptionEntity option)
      => GetCachedEntry(option.OptionCategory, option.OptionKey, option.UserId);


    // Internal methods
    private static string GenerateKey(string category, string key, int userId = 0)
    {
      // TODO: [TESTS] (OptionEntityCache.GenerateKey) Add tests
      return $"{category.LowerTrim()}|{key.LowerTrim()}|{userId:D}";
    }

    private static string GenerateKey(OptionEntity option)
      => GenerateKey(option.OptionCategory, option.OptionKey, option.UserId);
  }
}
