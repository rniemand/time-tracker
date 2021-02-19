using System.Collections.Generic;
using Rn.NetCore.Common.Extensions;
using TimeTracker.Core.Database.Entities;
using TimeTracker.Core.Enums;

namespace TimeTracker.Core.Models
{
  public class RawOptions
  {
    public Dictionary<string, OptionEntity> Options { get; set; }
    public string DefaultCategory { get; set; }


    // Constructors
    public RawOptions()
    {
      // TODO: [TESTS] (RawOptions) Add tests
      Options = new Dictionary<string, OptionEntity>();
      DefaultCategory = string.Empty;
    }

    public RawOptions(string defaultCategory)
      : this()
    {
      // TODO: [TESTS] (RawOptions) Add tests
      DefaultCategory = defaultCategory;
    }


    // Public methods
    public void AddOption(OptionEntity option)
    {
      // TODO: [TESTS] (RawOptions.AddOption) Add tests
      var dictionaryKey = GenerateDictionaryKey(option);

      // First time seeing this option
      if (!Options.ContainsKey(dictionaryKey))
      {
        Options[dictionaryKey] = option;
        return;
      }

      // User options always win
      if (Options[dictionaryKey].UserId > 0)
      {
        return;
      }

      // New user option - replace GLOBAL one
      Options[dictionaryKey] = option;
    }

    public bool HasOption(string category, string key)
    {
      // TODO: [TESTS] (RawOptions.HasOption) Add tests
      return Options.ContainsKey(GenerateDictionaryKey(category, key));
    }

    public bool HasOption(string key)
      => HasOption(DefaultCategory, key);

    public int GetIntOption(string category, string key, int fallback = 0)
    {
      // TODO: [TESTS] (RawOptions.GetIntOption) Add tests
      if (!HasOption(category, key))
        return fallback;

      var option = Options[GenerateDictionaryKey(category, key)];

      return option.OptionType != OptionType.Int
        ? fallback
        : option.OptionValue.AsInt(fallback);
    }

    public int GetIntOption(string key, int fallback = 0)
      => GetIntOption(DefaultCategory, key, fallback);

    public bool GetBoolOption(string category, string key, bool fallback)
    {
      // TODO: [TESTS] (RawOptions.GetIntOption) Add tests
      if (!HasOption(category, key))
        return fallback;

      var option = Options[GenerateDictionaryKey(category, key)];

      return option.OptionType != OptionType.Bool
        ? fallback
        : option.OptionValue.AsBool(fallback);
    }

    public bool GetBoolOption(string key, bool fallback)
      => GetBoolOption(DefaultCategory, key, fallback);

    public OptionEntity GenerateBoolOption(string category, string key, bool value, int userId = 0)
    {
      // TODO: [TESTS] (RawOptions.GenerateBoolOption) Add tests
      // Modify and return an existing option
      // ReSharper disable once InvertIf
      if (HasOption(key))
      {
        var option = Options[GenerateDictionaryKey(category, key)];
        option.OptionValue = value ? "true" : "false";
        return option;
      }

      // Create a new option and return in
      return new OptionEntity
      {
        UserId = userId,
        Deleted = false,
        OptionType = OptionType.Bool,
        OptionCategory = category,
        OptionKey = key,
        OptionValue = value ? "true" : "false",
        IsCollection = false
      };
    }

    public OptionEntity GenerateBoolOption(string key, bool value, int userId = 0)
      => GenerateBoolOption(DefaultCategory, key, value, userId);


    // Internal methods
    private static string GenerateDictionaryKey(OptionEntity option)
    {
      // TODO: [TESTS] (RawOptions.GenerateDictionaryKey) Add tests
      return GenerateDictionaryKey(option.OptionCategory, option.OptionKey);
    }

    private static string GenerateDictionaryKey(string category, string key)
    {
      // TODO: [TESTS] (RawOptions.GenerateDictionaryKey) Add tests
      return $"{category}.{key}".LowerTrim();
    }
  }
}