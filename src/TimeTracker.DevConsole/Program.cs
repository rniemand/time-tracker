using System;
using System.IO;
using System.Linq;
using Dapper;
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
using TimeTracker.Core.Database;
using TimeTracker.Core.Database.Queries;
using TimeTracker.Core.Database.Repos;
using TimeTracker.Core.Jobs;
using TimeTracker.Core.Models.Configuration;
using TimeTracker.Core.Models.Requests;
using TimeTracker.Core.Models.Responses;
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

      var response = new BaseResponse<RichardResponse>()
        .WithValidationError("error 1")
        .WithValidationError("error 2")
        .WithValidationMessage("FFS");

      var error = response.Validation.GenerateValidationError();

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

      ConfigureDI_Configuration(services, config);
      ConfigureDI_Core(services, config);
      ConfigureDI_DBCore(services);
      ConfigureDI_Repos(services);
      ConfigureDI_Services(services);

      // https://dapper-tutorial.net/knowledge-base/12510299/get-datetime-as-utc-with-dapper
      SqlMapper.AddTypeHandler(new DateTimeHandler());

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

    private static void ConfigureDI_Configuration(IServiceCollection services, IConfiguration config)
    {
      var mappedConfig = new TimeTrackerConfig();
      var configSection = config.GetSection("TimeTracker");
      
      if(configSection.Exists())
        configSection.Bind(mappedConfig);

      services.AddSingleton(mappedConfig);
    }
  }
}
