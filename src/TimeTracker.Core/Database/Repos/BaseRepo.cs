using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Rn.NetCore.Common.Logging;
using Rn.NetCore.Common.Metrics;

namespace TimeTracker.Core.Database.Repos
{
  public abstract class BaseRepo<TRepo>
  {
    public string RepoName { get; }
    public string ConnectionName { get; }
    public ILoggerAdapter<TRepo> Logger { get; }
    public IDbHelper DbHelper { get; }
    public IMetricService MetricService { get; }


    // Constructor
    protected BaseRepo(
      ILoggerAdapter<TRepo> logger,
      IDbHelper dbHelper,
      IMetricService metricService,
      string repoName,
      string connectionName)
    {
      Logger = logger;
      DbHelper = dbHelper;
      MetricService = metricService;
      RepoName = repoName;
      ConnectionName = connectionName;
    }


    // Public methods
    protected async Task<List<T>> GetList<T>(string methodName, string sql, object param = null)
    {
      // TODO: [TESTS] (BaseRepo) Add tests
      LogSqlCommand(methodName, sql, param);

      // TODO: [METRICS] (BaseRepo.GetList) Revise metrics
      //using (var metric = CreateTimingMetric(methodName))
      //{

      //}

      try
      {
        using (var connection = DbHelper.GetConnection(ConnectionName))
        {
          var results = await connection.QueryAsync<T>(sql, param);
          var resultList = results.ToList();

          //metric.SetTag("success", "true");
          //metric.SetField("num_rows", resultList.Count);

          return resultList;
        }
      }
      catch (Exception ex)
      {
        //metric.SetTag("exception", ex.GetType().Name, false);
        Logger.Error(ex, "Error running SQL query: {sql}", sql);

        return new List<T>();
      }
    }

    protected async Task<T> GetSingle<T>(string methodName, string sql, object param = null)
    {
      // TODO: [TESTS] (BaseRepo.GetSingle) Add tests
      // TODO: [TESTS] (BaseRepo.GetSingle) Add logging

      LogSqlCommand(methodName, sql, param);

      // TODO: [METRICS] (BaseRepo.GetSingle) Revise metrics
      //using (var metric = CreateTimingMetric(methodName))
      //{

      //}

      try
      {
        using (var connection = DbHelper.GetConnection(ConnectionName))
        {
          var results = await connection.QueryAsync<T>(sql, param);
          var actualResult = results.FirstOrDefault();

          //metric.SetTag("success", "true");
          //metric.SetField("num_rows", actualResult == null ? 0 : 1);

          return actualResult;
        }
      }
      catch (Exception ex)
      {
        //metric.SetTag("exception", ex.GetType().Name, false);
        Logger.Error(ex, "Error running SQL query: {sql}", sql);

        return default(T);
      }
    }

    protected async Task<int> ExecuteAsync(string methodName, string sql, object param = null)
    {
      // TODO: [TESTS] (BaseRepo) Add tests

      LogSqlCommand(methodName, sql, param);

      // TODO: [METRICS] (BaseRepo.ExecuteAsync) Revise metrics
      //using (var metric = CreateTimingMetric(methodName))
      //{

      //}

      try
      {
        using (var connection = DbHelper.GetConnection(ConnectionName))
        {
          var numRows = await connection.ExecuteAsync(sql, param);

          //metric.SetTag("success", "true");
          //metric.SetField("num_rows", numRows);

          return numRows;
        }
      }
      catch (Exception ex)
      {
        //metric.SetTag("exception", ex.GetType().Name, false);
        Logger.Error(ex, "Error running SQL query: {sql}", sql);

        return 0;
      }
    }

    // Internal methods
    public void LogSqlCommand(string methodName, string sql, object param = null)
    {
      // TODO: [TESTS] (BaseRepo.LogSqlCommand) Add tests
      // TODO: [REVISE] (BaseRepo.LogSqlCommand) Add configuration to enable \ disable this

      Logger.Info(
        "[{repo}.{method}] Running SQL command: {sql}",
        RepoName, methodName,
        SqlFormatter.MergeParams(sql, param)
      );
    }
  }
}
