using System;
using System.Transactions;
using Dapper;
using Hangfire;
using Hangfire.MySql;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Rn.NetCore.Common.Abstractions;
using Rn.NetCore.Common.Encryption;
using Rn.NetCore.Common.Helpers;
using Rn.NetCore.Common.Logging;
using Rn.NetCore.Common.Metrics;
using Rn.NetCore.Common.Metrics.Interfaces;
using Rn.NetCore.DbCommon;
using Rn.NetCore.Metrics.Rabbit;
using TimeTracker.Core.Database;
using TimeTracker.Core.Database.Queries;
using TimeTracker.Core.Database.Repos;
using TimeTracker.Core.Jobs;
using TimeTracker.Core.Models.Configuration;
using TimeTracker.Core.Services;
using TimeTracker.Core.WebApi.Middleware;

namespace TimeTracker
{
  public class Startup
  {
    public Startup(IConfiguration configuration)
    {
      Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    public void ConfigureServices(IServiceCollection services)
    {
      services.AddControllersWithViews();

      ConfigureServices_Configuration(services);
      ConfigureServices_Core(services);
      ConfigureServices_Metrics(services);
      ConfigureServices_Services(services);
      ConfigureServices_Helpers(services);
      ConfigureServices_DbCore(services);
      ConfigureServices_Repos(services);
      ConfigureServices_Hangfire(services);

      services.AddMvc();

      services.AddSpaStaticFiles(configuration =>
      {
        configuration.RootPath = "ClientApp/dist";
      });
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IServiceProvider serviceProvider)
    {
      if (env.IsDevelopment())
      {
        app.UseDeveloperExceptionPage();
      }
      else
      {
        app.UseExceptionHandler("/Error");
      }

      // https://dapper-tutorial.net/knowledge-base/12510299/get-datetime-as-utc-with-dapper
      SqlMapper.AddTypeHandler(new DateTimeHandler());

      app.UseStaticFiles();
      if (!env.IsDevelopment())
      {
        app.UseSpaStaticFiles();
      }

      app.UseRouting();

      Configure_HangfireDashboard(app);
      Configure_HangfireJobs(serviceProvider);

      app.UseMiddleware<JwtMiddleware>();
      app.UseAuthorization();

      app.UseEndpoints(endpoints =>
      {
        endpoints.MapControllerRoute(
          name: "default",
          pattern: "{controller}/{action=Index}/{id?}"
        );
      });

      app.UseSpa(spa =>
      {
        spa.Options.SourcePath = "ClientApp";

        if (env.IsDevelopment())
        {
          //spa.UseAngularCliServer(npmScript: "start");
          spa.UseProxyToSpaDevelopmentServer("http://localhost:4200/");
        }
      });
    }


    // Configure() related methods
    private void Configure_HangfireDashboard(IApplicationBuilder app)
    {
      // Generate configuration to use
      var hangfireConfig = new HangfireConfiguration();
      var configSection = Configuration.GetSection("TimeTracker:Hangfire");
      if (configSection.Exists())
        configSection.Bind(hangfireConfig);

      // Configure Hangfire dashboard
      app.UseHangfireDashboard(hangfireConfig.PathMatch, new DashboardOptions
      {
        // Used for proxying requests
        // https://discuss.hangfire.io/t/dashboard-returns-403-on-stats-call/7831
        IgnoreAntiforgeryToken = hangfireConfig.IgnoreAntiforgeryToken,
        DashboardTitle = hangfireConfig.DashboardTitle
      });
    }

    private static void Configure_HangfireJobs(IServiceProvider serviceProvider)
    {
      // http://corntab.com/ <- UI for building CRON expressions

      RecurringJob.AddOrUpdate(
        "Sweep long running timers",
        () => new SweepLongRunningTimers(serviceProvider).Run(),
        "* * * * *"
      );
    }



    // ConfigureServices() related methods
    private void ConfigureServices_Configuration(IServiceCollection services)
    {
      var mappedConfig = new TimeTrackerConfig();
      var configSection = Configuration.GetSection("TimeTracker");
      
      if(configSection.Exists())
        configSection.Bind(mappedConfig);

      services.AddSingleton(mappedConfig);
    }

    private static void ConfigureServices_Core(IServiceCollection services)
    {
      services
        .AddSingleton(typeof(ILoggerAdapter<>), typeof(LoggerAdapter<>))
        .AddSingleton<IDateTimeAbstraction, DateTimeAbstraction>();
    }

    private static void ConfigureServices_Services(IServiceCollection services)
    {
      services
        .AddSingleton<IEncryptionService, EncryptionService>()
        .AddSingleton<IUserService, UserService>()
        .AddSingleton<IClientService, ClientService>()
        .AddSingleton<IProductService, ProductService>()
        .AddSingleton<IProjectService, ProjectService>()
        .AddSingleton<ITimerService, TimerService>()
        .AddSingleton<IOptionsService, OptionsService>()
        .AddSingleton<IDailyTasksService, DailyTasksService>();
    }

    private static void ConfigureServices_Helpers(IServiceCollection services)
    {
      services
        .AddSingleton<IEncryptionUtils, EncryptionUtils>()
        .AddSingleton<IJsonHelper, JsonHelper>();
    }

    private static void ConfigureServices_DbCore(IServiceCollection services)
    {
      services
        .AddSingleton<IDbHelper, DbHelper>();
    }

    private static void ConfigureServices_Repos(IServiceCollection services)
    {
      services
        .AddSingleton<IUserRepo, UserRepo>()
        .AddSingleton<IUserQueries, UserQueries>()
        .AddSingleton<IClientRepo, ClientRepo>()
        .AddSingleton<IClientQueries, ClientQueries>()
        .AddSingleton<IProductRepo, ProductRepo>()
        .AddSingleton<IProductQueries, ProductQueries>()
        .AddSingleton<IProjectRepo, ProjectRepo>()
        .AddSingleton<IProjectQueries, ProjectQueries>()
        .AddSingleton<ITimerRepo, TimerRepo>()
        .AddSingleton<ITimerQueries, TimerQueries>()
        .AddSingleton<IOptionRepo, OptionRepo>()
        .AddSingleton<IOptionQueries, OptionQueries>()
        .AddSingleton<IDailyTasksRepo, DailyTasksRepo>()
        .AddSingleton<IDailyTasksQueries, DailyTasksQueries>();
    }

    private static void ConfigureServices_Metrics(IServiceCollection services)
    {
      services
        .AddSingleton<IMetricService, MetricService>()
        // RabbitMQ
        .AddSingleton<IMetricOutput, RabbitMetricOutput>()
        .AddSingleton<IRabbitFactory, RabbitFactory>()
        .AddSingleton<IRabbitConnection, RabbitConnection>();
    }

    private void ConfigureServices_Hangfire(IServiceCollection services)
    {
      services.AddHangfire(configuration => configuration
        .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
        .UseSimpleAssemblyNameTypeSerializer()
        .UseRecommendedSerializerSettings()
        .UseStorage(new MySqlStorage(
          Configuration.GetConnectionString("TimeTracker"),
          new MySqlStorageOptions
          {
            TransactionIsolationLevel = IsolationLevel.ReadCommitted,
            QueuePollInterval = TimeSpan.FromSeconds(20),
            JobExpirationCheckInterval = TimeSpan.FromHours(1),
            CountersAggregateInterval = TimeSpan.FromMinutes(5),
            PrepareSchemaIfNecessary = true,
            DashboardJobListLimit = 50000,
            TransactionTimeout = TimeSpan.FromSeconds(30),
            TablesPrefix = "Hangfire"
          })));

      services.AddHangfireServer(options =>
      {
        options.WorkerCount = 3;
      });
    }
  }
}
