using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using TimeTracker.Core.Database.Entities;

namespace TimeTracker.Core.Caches
{
  public class TimeSheetDateCache
  {
    private readonly ConcurrentDictionary<string, TimeSheetDate> _cache;


    // Constructor
    public TimeSheetDateCache()
    {
      // TODO: [TESTS] (TimeSheetDateCache) Add tests
      _cache = new ConcurrentDictionary<string, TimeSheetDate>();
    }

    public TimeSheetDateCache(List<TimeSheetDate> entries)
      : this()
    {
      // TODO: [TESTS] (TimeSheetDateCache) Add tests
      CacheEntries(entries);
    }


    // Public methods
    public void CacheEntry(TimeSheetDate entry)
    {
      // TODO: [TESTS] (TimeSheetDateCache.CacheEntry) Add tests
      if (entry == null)
        return;

      _cache.AddOrUpdate(
        GenerateKey(entry),
        entry,
        (key, oldValue) => entry
      );
    }

    public void CacheEntries(List<TimeSheetDate> entries)
    {
      // TODO: [TESTS] (TimeSheetDateCache.CacheEntries) Add tests
      foreach (var entry in entries)
      {
        CacheEntry(entry);
      }
    }

    public TimeSheetDate GetCachedEntry(int userId, int clientId, DateTime date)
    {
      // TODO: [TESTS] (TimeSheetDateCache.GetCachedEntry) Add tests
      var cacheKey = GenerateKey(userId, clientId, date);
      return !_cache.ContainsKey(cacheKey) ? null : _cache[cacheKey];
    }


    // Internal methods
    private static string GenerateKey(int userId, int clientId, DateTime date)
    {
      // TODO: [TESTS] (TimeSheetDateCache.GenerateKey) Add tests
      return $"{userId:D}|{clientId:D}|{date:yyyy-MM-dd}";
    }

    private static string GenerateKey(TimeSheetDate entry)
      => GenerateKey(entry.UserId, entry.ClientId, entry.EntryDate);
  }
}
