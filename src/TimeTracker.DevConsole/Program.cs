using System;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using Rn.NetCore.Common.Abstractions;
using Rn.NetCore.Common.Encryption;
using Rn.NetCore.Common.Helpers;
using Rn.NetCore.Common.Logging;
using Rn.NetCore.Common.Metrics;
using Rn.NetCore.DbCommon;
using TimeTracker.Core.Database.Queries;
using TimeTracker.Core.Database.Repos;
using TimeTracker.Core.Models.Requests;
using TimeTracker.Core.Services;

namespace TimeTracker.DevConsole
{
  class Program
  {
    private static IServiceProvider _serviceProvider;
    private static ILoggerAdapter<Program> _logger;

    static void Main(string[] args)
    {
      ConfigureDI();

      // https://jasonwatmore.com/post/2019/10/11/aspnet-core-3-jwt-authentication-tutorial-with-example-api

      var optionsService = _serviceProvider.GetService<IOptionsService>();


      var options = optionsService.GenerateRawOptions("RunningTimers", 1)
        .ConfigureAwait(false)
        .GetAwaiter()
        .GetResult();

      var hasOption = options.HasOption("RunningTimers","MaxLength.Min");
      var intOption = options.GetIntOption("RunningTimers","MaxLength.Min");
      var boolOption = options.GetBoolOption("RunningTimers","Logging.Enabled", true);


      Console.WriteLine("Hello World!");
    }


    // Helper methods
    private static string EncryptPassword(string password)
    {
      var encryptionService = _serviceProvider.GetService<IEncryptionService>();
      return encryptionService.Encrypt(password);
    }


    // DI related methods
    private static void ConfigureDI()
    {
      var services = new ServiceCollection();

      var config = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
        .Build();

      ConfigureDI_Core(services, config);
      ConfigureDI_DBCore(services);
      ConfigureDI_Repos(services);
      ConfigureDI_Services(services);

      _serviceProvider = services.BuildServiceProvider();
      _logger = _serviceProvider.GetService<ILoggerAdapter<Program>>();
    }

    private static void ConfigureDI_Core(IServiceCollection services, IConfiguration config)
    {
      services
        .AddSingleton(config)
        .AddSingleton(typeof(ILoggerAdapter<>), typeof(LoggerAdapter<>))
        .AddSingleton<IEncryptionService, EncryptionService>()
        .AddSingleton<IEncryptionUtils, EncryptionUtils>()
        .AddSingleton<IMetricService, MetricService>()
        .AddSingleton<IDateTimeAbstraction, DateTimeAbstraction>()
        .AddSingleton<IJsonHelper, JsonHelper>()
        .AddLogging(loggingBuilder =>
        {
          // configure Logging with NLog
          loggingBuilder.ClearProviders();
          loggingBuilder.SetMinimumLevel(LogLevel.Trace);
          loggingBuilder.AddNLog(config);
        });
    }

    private static void ConfigureDI_DBCore(IServiceCollection services)
    {
      services
        .AddSingleton<IDbHelper, DbHelper>();
    }

    private static void ConfigureDI_Repos(IServiceCollection services)
    {
      services
        .AddSingleton<IUserRepo, UserRepo>()
        .AddSingleton<IUserRepoQueries, UserRepoQueries>()
        .AddSingleton<IClientRepo, ClientRepo>()
        .AddSingleton<IClientRepoQueries, ClientRepoQueries>()
        .AddSingleton<IProductRepo, ProductRepo>()
        .AddSingleton<IProductRepoQueries, ProductRepoQueries>()
        .AddSingleton<IProjectRepo, ProjectRepo>()
        .AddSingleton<IProjectRepoQueries, ProjectRepoQueries>()
        .AddSingleton<IRawTimersRepo, RawTimersRepo>()
        .AddSingleton<IRawTimersRepoQueries, RawTimersRepoQueries>()
        .AddSingleton<IOptionRepo, OptionRepo>()
        .AddSingleton<IOptionRepoQueries, OptionRepoQueries>();
    }

    private static void ConfigureDI_Services(IServiceCollection services)
    {
      services
        .AddSingleton<IUserService, UserService>()
        .AddSingleton<IClientService, ClientService>()
        .AddSingleton<IProductService, ProductService>()
        .AddSingleton<IProjectService, ProjectService>()
        .AddSingleton<IRawTimerService, RawTimerService>()
        .AddSingleton<IOptionsService, OptionsService>();
    }
  }
}
