using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Rn.NetCore.Common.Abstractions;
using Rn.NetCore.Common.Logging;
using TimeTracker.Core.Caches;
using TimeTracker.Core.Database.Entities;
using TimeTracker.Core.Database.Repos;
using TimeTracker.Core.Models;
using TimeTracker.Core.Services;

namespace TimeTracker.Core.Jobs
{
  public class GenerateTimeSheetDates
  {
    public const string Category = "TimeSheet.Cron.GenerateDates";

    private readonly ILoggerAdapter<GenerateTimeSheetDates> _logger;
    private readonly IDateTimeAbstraction _dateTime;
    private readonly IOptionsService _optionsService;
    private readonly IUserRepo _userRepo;
    private readonly IClientRepo _clientRepo;
    private readonly ITimeSheetDateRepo _timeSheetDateRepo;
    private readonly ITimeSheetService _timeSheetService;

    // Constructor and Run()
    public GenerateTimeSheetDates(IServiceProvider serviceProvider)
    {
      // TODO: [TESTS] (GenerateTimeSheetDates) Add tests
      _logger = serviceProvider.GetRequiredService<ILoggerAdapter<GenerateTimeSheetDates>>();
      _dateTime = serviceProvider.GetRequiredService<IDateTimeAbstraction>();
      _optionsService = serviceProvider.GetRequiredService<IOptionsService>();
      _userRepo = serviceProvider.GetRequiredService<IUserRepo>();
      _clientRepo = serviceProvider.GetRequiredService<IClientRepo>();
      _timeSheetDateRepo = serviceProvider.GetRequiredService<ITimeSheetDateRepo>();
      _timeSheetService = serviceProvider.GetRequiredService<ITimeSheetService>();
    }

    public async Task Run()
    {
      // TODO: [TESTS] (GenerateTimeSheetDates.Run) Add tests
      var users = await _userRepo.GetEnabledUsers();

      foreach (var user in users)
      {
        await ProcessUser(user);
      }
    }


    // Processing methods
    private async Task ProcessUser(UserEntity user)
    {
      // TODO: [TESTS] (GenerateTimeSheetDates.ProcessUser) Add tests
      // Check to see if we are able to run
      var rawOptions = await _optionsService.GenerateOptions(Category, user.UserId);
      var config = new GenerateTimeSheetDateConfig(rawOptions).SetUser(user);
      if (!config.Enabled)
        return;

      // Set the date range and process all enabled clients for this user
      config.SetDateRange(_dateTime.Now);
      await config.CacheTimeSheetDates(_timeSheetDateRepo);

      foreach (var client in await _clientRepo.GetAll(user.UserId))
      {
        await ProcessClient(config, client);
      }

    }

    private async Task ProcessClient(GenerateTimeSheetDateConfig config, ClientEntity client)
    {
      // TODO: [TESTS] (GenerateTimeSheetDates.ProcessClient) Add tests
      // Ensure that we can process this client (disabled by default)
      await config.EnsureClientOptionExists(_optionsService, client);
      if (!config.CanProcessClient(client))
        return;

      for (var i = 0; i < config.DaysAhead; i++)
      {
        var currentDate = config.StartDate.AddDays(i);
        await config.GetDbTimeSheetDate(_timeSheetService, client, currentDate);
      }
    }

  }

  public class GenerateTimeSheetDateConfig
  {
    public int DaysAhead { get; }
    public bool Enabled { get; }
    public UserEntity User { get; private set; }
    public DateTime StartDate { get; private set; }
    public DateTime EndDate { get; private set; }

    private readonly RawOptions _rawOptions;
    private readonly OptionEntityCache _optionCache;
    private readonly TimeSheetDateCache _tsDateCache;


    // Constructors
    private GenerateTimeSheetDateConfig()
    {
      // TODO: [TESTS] (GenerateTimeSheetDateConfig) Add tests
      DaysAhead = 7;
      Enabled = false;

      _rawOptions = null;
      _optionCache = new OptionEntityCache();
      _tsDateCache = new TimeSheetDateCache();
    }

    public GenerateTimeSheetDateConfig(RawOptions options)
      : this()
    {
      // TODO: [TESTS] (GenerateTimeSheetDateConfig) Add tests
      DaysAhead = options.GetIntOption("DaysAhead", 7);
      Enabled = options.GetBoolOption("Enabled", false);

      _rawOptions = options;
    }


    // Public methods
    public GenerateTimeSheetDateConfig SetUser(UserEntity user)
    {
      // TODO: [TESTS] (GenerateTimeSheetDateConfig.SetUser) Add tests
      User = user;
      return this;
    }

    public void SetDateRange(DateTime now)
    {
      // TODO: [TESTS] (GenerateTimeSheetDateConfig.SetDateRange) Add tests
      StartDate = now;
      EndDate = now.AddDays(DaysAhead);
    }

    public async Task CacheTimeSheetDates(ITimeSheetDateRepo timeSheetDateRepo)
    {
      // TODO: [TESTS] (GenerateTimeSheetDateConfig.CacheTimeSheetDates) Add tests
      _tsDateCache.CacheEntries(
        await timeSheetDateRepo.GetDatesForRange(User.UserId, StartDate, EndDate)
      );
    }

    public async Task EnsureClientOptionExists(IOptionsService optionsService, ClientEntity client)
    {
      // TODO: [TESTS] (GenerateTimeSheetDateConfig.EnsureClientOptionExists) Add tests
      // Do we need to add this option
      var key = $"client.{client.ClientId:D}.generate";
      if (_rawOptions.HasOption(GenerateTimeSheetDates.Category, key))
        return;

      // Were we able to add this option
      var stubbedOption = _rawOptions.GenerateBoolOption(key, false, client.UserId);
      var dbOption = await optionsService.UpsertOption(_optionCache, stubbedOption);
      if (dbOption == null)
        return;

      // Cache option (we will need it later)
      _rawOptions.AddOption(dbOption);
    }

    public bool CanProcessClient(ClientEntity client)
    {
      // TODO: [TESTS] (GenerateTimeSheetDateConfig.CanProcessClient) Add tests
      var key = $"client.{client.ClientId:D}.generate";
      return _rawOptions.GetBoolOption(key, false);
    }

    public async Task<TimeSheetDate> GetDbTimeSheetDate(ITimeSheetService timeSheetService, ClientEntity client, DateTime date)
      => await timeSheetService.EnsureTimeSheetDateExists(_tsDateCache, client, date);
  }
}
