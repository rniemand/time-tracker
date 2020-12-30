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
using TimeTracker.Core.Services;

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

      app.UseStaticFiles();
      if (!env.IsDevelopment())
      {
        app.UseSpaStaticFiles();
      }

      app.UseRouting();

      app.UseMiddleware<JwtMiddleware>();

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
        .AddSingleton<IUserService, UserService>();
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
        .AddSingleton<IUserRepoQueries, UserRepoQueries>();
    }
  }
}
