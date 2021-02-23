using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Rn.NetCore.Common.Abstractions;
using Rn.NetCore.Common.Factories;
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
    private readonly ITimerFactory _timerFactory;
    private readonly IUserRepo _userRepo;
    private readonly IServiceProvider _serviceProvider;

    // Constructor and Run()
    public GenerateTimeSheetDates(IServiceProvider serviceProvider)
    {
      // TODO: [TESTS] (GenerateTimeSheetDates) Add tests
      _serviceProvider = serviceProvider;
      _logger = serviceProvider.GetRequiredService<ILoggerAdapter<GenerateTimeSheetDates>>();
      _timerFactory = serviceProvider.GetRequiredService<ITimerFactory>();
      _userRepo = serviceProvider.GetRequiredService<IUserRepo>();
    }

    public async Task Run()
    {
      // TODO: [TESTS] (GenerateTimeSheetDates.Run) Add tests
      var stopwatch = _timerFactory.NewStopwatch();
      stopwatch.Start();

      var users = await _userRepo.GetEnabledUsers();
      foreach (var user in users)
      {
        await ProcessUser(user);
      }

      _logger.Info("Processed {count} users in {time} ms.",
        users.Count,
        stopwatch.ElapsedMilliseconds
      );
    }


    // Processing methods
    private async Task ProcessUser(UserEntity user)
    {
      // TODO: [TESTS] (GenerateTimeSheetDates.ProcessUser) Add tests
      // Check to see if we are able to run
      var state = await (new GenerateTimeSheetDateState(_serviceProvider)).SetUser(user);
      if (!state.CanRun())
        return;

      foreach (var client in state.Clients)
      {
        if (!state.CanProcessClient(client))
          return;

        var startDate = state.Config.StartDate;
        for (var i = 0; i < state.Config.DaysAhead; i++)
        {
          await state.EnsureTimeSheetDateExists(client, startDate.AddDays(i));
        }
      }
    }
  }

  public class GenerateTimeSheetDateState
  {
    public UserEntity User { get; private set; }
    public GenerateTimeSheetDateConfig Config { get; private set; }
    public List<ClientEntity> Clients { get; private set; }

    private readonly IOptionsService _optionsService;
    private readonly IDateTimeAbstraction _dateTime;
    private readonly ITimeSheetDateRepo _timeSheetDateRepo;
    private readonly IClientRepo _clientRepo;
    private readonly ITimeSheetService _timeSheetService;
    private readonly TimeSheetDateCache _tsDateCache;

    // Constructor
    public GenerateTimeSheetDateState(IServiceProvider serviceProvider)
    {
      // TODO: [TESTS] (GenerateTimeSheetDateState.GenerateTimeSheetDateState) Add tests
      _optionsService = serviceProvider.GetRequiredService<IOptionsService>();
      _dateTime = serviceProvider.GetRequiredService<IDateTimeAbstraction>();
      _timeSheetDateRepo = serviceProvider.GetRequiredService<ITimeSheetDateRepo>();
      _clientRepo = serviceProvider.GetRequiredService<IClientRepo>();
      _timeSheetService = serviceProvider.GetRequiredService<ITimeSheetService>();

      _tsDateCache = new TimeSheetDateCache();

      Clients = new List<ClientEntity>();
    }

    public async Task<GenerateTimeSheetDateState> SetUser(UserEntity user)
    {
      // TODO: [TESTS] (GenerateTimeSheetDateState.SetUser) Add tests
      var rawOptions = await _optionsService.GenerateOptions(GenerateTimeSheetDates.Category, user.UserId);

      Config = new GenerateTimeSheetDateConfig(rawOptions);
      User = user;

      await BootstrapIfEnabled();
      return this;
    }


    // Public methods
    public bool CanRun() => Config.Enabled;

    public async Task<List<ClientEntity>> GetClients()
      => await _clientRepo.GetAll(User.UserId);

    public bool CanProcessClient(ClientEntity client)
      => Config.CanProcessClient(client);

    public async Task<bool> EnsureTimeSheetDateExists(ClientEntity client, DateTime date)
      => await _timeSheetService.EnsureTimeSheetDateExists(_tsDateCache, client, date);


    // Internal methods
    private async Task BootstrapIfEnabled()
    {
      // TODO: [TESTS] (GenerateTimeSheetDateState.BootstrapIfEnabled) Add tests
      if (!Config.Enabled)
        return;

      Config.StartDate = _dateTime.Now;
      Config.EndDate = Config.StartDate.AddDays(Config.DaysAhead);

      _tsDateCache.CacheEntries(
        await _timeSheetDateRepo.GetUserDates(User.UserId, Config.StartDate, Config.EndDate)
      );

      await LoadUserClients();
    }

    private async Task LoadUserClients()
    {
      // TODO: [TESTS] (GenerateTimeSheetDateState.LoadUserClients) Add tests
      Clients.AddRange(await _clientRepo.GetAll(User.UserId));

      foreach (var client in Clients)
      {
        await Config.EnsureClientOptionExists(_optionsService, client);
      }
    }
  }

  public class GenerateTimeSheetDateConfig
  {
    public int DaysAhead { get; }
    public bool Enabled { get; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }

    private readonly RawOptions _rawOptions;
    private readonly OptionEntityCache _optionCache;


    // Constructors
    private GenerateTimeSheetDateConfig()
    {
      // TODO: [TESTS] (GenerateTimeSheetDateConfig) Add tests
      DaysAhead = 7;
      Enabled = false;

      _rawOptions = null;
      _optionCache = new OptionEntityCache();
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
  }
}
