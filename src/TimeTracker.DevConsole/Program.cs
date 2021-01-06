﻿using System;
using System.IO;
using Dapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using Rn.NetCore.Common.Abstractions;
using Rn.NetCore.Common.Encryption;
using Rn.NetCore.Common.Extensions;
using Rn.NetCore.Common.Helpers;
using Rn.NetCore.Common.Logging;
using Rn.NetCore.Common.Metrics;
using Rn.NetCore.DbCommon;
using TimeTracker.Core.Database;
using TimeTracker.Core.Database.Queries;
using TimeTracker.Core.Database.Repos;
using TimeTracker.Core.Models.Configuration;
using TimeTracker.Core.Services;
using TimeTracker.DevConsole.Setup.Config;

namespace TimeTracker.DevConsole
{
  class Program
  {
    private static IServiceProvider _serviceProvider;
    private static ILoggerAdapter<Program> _logger;

    static void Main(string[] args)
    {
      ConfigureDI();
      
      GenerateSampleConfig();


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
        .AddSingleton<IEnvironmentAbstraction, EnvironmentAbstraction>()
        .AddSingleton<IDirectoryAbstraction, DirectoryAbstraction>()
        .AddSingleton<IFileAbstraction, FileAbstraction>()
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


    // GenerateSampleConfig() and supporting methods
    private static void GenerateSampleConfig()
    {
      var jsonHelper = _serviceProvider.GetRequiredService<IJsonHelper>();
      var encryptionUtils = _serviceProvider.GetRequiredService<IEncryptionUtils>();
      var environment = _serviceProvider.GetRequiredService<IEnvironmentAbstraction>();
      var directory = _serviceProvider.GetRequiredService<IDirectoryAbstraction>();
      var file = _serviceProvider.GetRequiredService<IFileAbstraction>();

      // Generate sample configuration
      var config = new SampleAppSettings();
      config.RnCore.Encryption.Enabled = true;
      config.RnCore.Encryption.Key = RandomBytesString(encryptionUtils, 8);
      config.RnCore.Encryption.IV = RandomBytesString(encryptionUtils, 128);
      config.TimeTracker.Authentication.Secret = RandomBytesString(encryptionUtils, 32);
      config.TimeTracker.Authentication.SessionLengthMin = 10080;
      var jsonConfig = jsonHelper.SerializeObject(config, true);

      // Ensure that the output directory exists
      var rootDir = environment.CurrentDirectory.AppendIfMissing("\\");
      var generatedDir = $"{rootDir}generated\\";
      var sampleAppSettingsFile = $"{generatedDir}appsettings.json";
      if (!directory.Exists(generatedDir))
        directory.CreateDirectory(generatedDir);

      // Dump generated "appsettings.json" file
      if (file.Exists(sampleAppSettingsFile))
        file.Delete(sampleAppSettingsFile);
      file.WriteAllText(sampleAppSettingsFile, jsonConfig);

      // Log that we are done
      Console.WriteLine("=======================================");
      Console.WriteLine("= Sample configuration file generated =");
      Console.WriteLine("=======================================");
      Console.WriteLine();
      Console.WriteLine("A sample appsettings.json file was saved to:");
      Console.WriteLine();
      Console.WriteLine($"  {sampleAppSettingsFile}");
    }

    private static string RandomBytesString(IEncryptionUtils utils, int length)
    {
      return utils.ToBase64String(utils.GetRandomBytes(length));
    }
  }
}
