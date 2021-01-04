using System;
using System.Transactions;
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
using Rn.NetCore.DbCommon;
using TimeTracker.Core.Database.Queries;
using TimeTracker.Core.Database.Repos;
using TimeTracker.Core.Jobs;
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

      ConfigureServices_Core(services);
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

      app.UseStaticFiles();
      if (!env.IsDevelopment())
      {
        app.UseSpaStaticFiles();
      }

      app.UseRouting();
      app.UseHangfireDashboard();
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
        .AddSingleton<IMetricService, MetricService>()
        .AddSingleton<IUserService, UserService>()
        .AddSingleton<IClientService, ClientService>()
        .AddSingleton<IProductService, ProductService>()
        .AddSingleton<IProjectService, ProjectService>()
        .AddSingleton<IRawTimerService, RawTimerService>()
        .AddSingleton<IOptionsService, OptionsService>();
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
            QueuePollInterval = TimeSpan.FromSeconds(15),
            JobExpirationCheckInterval = TimeSpan.FromHours(1),
            CountersAggregateInterval = TimeSpan.FromMinutes(5),
            PrepareSchemaIfNecessary = true,
            DashboardJobListLimit = 50000,
            TransactionTimeout = TimeSpan.FromMinutes(1),
            TablesPrefix = "Hangfire"
          })));

      services.AddHangfireServer(options =>
      {
        options.WorkerCount = 1;
      });
    }
  }
}
