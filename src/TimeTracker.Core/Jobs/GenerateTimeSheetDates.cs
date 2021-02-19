using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Rn.NetCore.Common.Logging;
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
    private readonly IOptionsService _optionsService;
    private readonly IUserRepo _userRepo;
    private readonly IClientRepo _clientRepo;

    // Constructor and Run()
    public GenerateTimeSheetDates(IServiceProvider serviceProvider)
    {
      // TODO: [TESTS] (GenerateTimeSheetDates) Add tests
      _logger = serviceProvider.GetRequiredService<ILoggerAdapter<GenerateTimeSheetDates>>();
      _optionsService = serviceProvider.GetRequiredService<IOptionsService>();
      _userRepo = serviceProvider.GetRequiredService<IUserRepo>();
      _clientRepo = serviceProvider.GetRequiredService<IClientRepo>();
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
      var config = new GenerateTimeSheetDateConfig(rawOptions);
      if (!config.Enabled)
        return;

      var clients = await _clientRepo.GetAll(user.UserId);


    }
  }

  public class GenerateTimeSheetDateConfig
  {
    public int DaysAhead { get; set; }
    public bool Enabled { get; set; }


    // Constructors
    public GenerateTimeSheetDateConfig()
    {
      // TODO: [TESTS] (GenerateTimeSheetDateConfig) Add tests
      DaysAhead = 7;
      Enabled = false;
    }

    public GenerateTimeSheetDateConfig(RawOptions options)
      : this()
    {
      // TODO: [TESTS] (GenerateTimeSheetDateConfig) Add tests
      DaysAhead = options.GetIntOption("DaysAhead", 7);
      Enabled = options.GetBoolOption("Enabled", false);
    }
  }
}
