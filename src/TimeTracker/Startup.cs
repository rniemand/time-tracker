using Dapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Rn.NetCore.Common.Abstractions;
using Rn.NetCore.Common.Encryption;
using Rn.NetCore.Common.Factories;
using Rn.NetCore.Common.Helpers;
using Rn.NetCore.Common.Logging;
using Rn.NetCore.Common.Metrics;
using Rn.NetCore.Common.Metrics.Interfaces;
using Rn.NetCore.DbCommon.Helpers;
using Rn.NetCore.Metrics.Rabbit;
using Rn.NetCore.WebCommon.Filters;
using Rn.NetCore.WebCommon.Middleware;
using TimeTracker.Core.Database;
using TimeTracker.Core.Database.Queries;
using TimeTracker.Core.Database.Repos;
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
      services.AddControllersWithViews(options =>
      {
        options.Filters.Add<ApiMetricActionFilter>();
        options.Filters.Add<ApiMetricExceptionFilter>();
        options.Filters.Add<ApiMetricResultFilter>();
        options.Filters.Add<ApiMetricResourceFilter>();
      });

      ConfigureServices_Configuration(services);
      ConfigureServices_Core(services);
      ConfigureServices_Metrics(services);
      ConfigureServices_Services(services);
      ConfigureServices_Helpers(services);
      ConfigureServices_DbCore(services);
      ConfigureServices_Repos(services);

      services.AddMvc();

      services.AddSpaStaticFiles(configuration =>
      {
        configuration.RootPath = "ClientApp/dist";
      });
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
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
      app.UseMiddleware<JwtMiddleware>();
      app.UseMiddleware<ApiMetricsMiddleware>();
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


    // ConfigureServices() related methods
    private void ConfigureServices_Configuration(IServiceCollection services)
    {
      var mappedConfig = new TimeTrackerConfig();
      var configSection = Configuration.GetSection("TimeTracker");

      if (configSection.Exists())
        configSection.Bind(mappedConfig);

      services.AddSingleton(mappedConfig);
    }

    private static void ConfigureServices_Core(IServiceCollection services)
    {
      services
        .AddSingleton(typeof(ILoggerAdapter<>), typeof(LoggerAdapter<>))
        .AddSingleton<IDateTimeAbstraction, DateTimeAbstraction>()
        .AddSingleton<ITimerFactory, TimerFactory>();
    }

    private static void ConfigureServices_Services(IServiceCollection services)
    {
      services
        .AddSingleton<IEncryptionService, EncryptionService>()
        .AddSingleton<IUserService, UserService>()
        .AddSingleton<IClientService, ClientService>()
        .AddSingleton<IProductService, ProductService>()
        .AddSingleton<IProjectService, ProjectService>()
        .AddSingleton<IOptionsService, OptionsService>()
        .AddSingleton<ITimeSheetService, TimeSheetService>();
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
        .AddSingleton<IDbHelper, MySqlHelper>();
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
        .AddSingleton<IOptionRepo, OptionRepo>()
        .AddSingleton<IOptionQueries, OptionQueries>()
        .AddSingleton<ITimeSheetEntryRepo, TimeSheetEntryRepo>()
        .AddSingleton<ITimeSheetEntryRepoQueries, TimeSheetEntryRepoQueries>();
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
  }
}
