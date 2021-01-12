using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Rn.NetCore.Common.Logging;
using Rn.NetCore.Common.Metrics;
using Rn.NetCore.Common.Metrics.Builders;
using TimeTracker.Core.Database.Entities;
using TimeTracker.Core.Database.Repos;
using TimeTracker.Core.Enums;
using TimeTracker.Core.Models.Dto;

namespace TimeTracker.Core.Services
{
  public interface IClientService
  {
    Task<List<ClientDto>> GetAll(int userId);
    Task<bool> AddClient(int userId, ClientDto clientDto);
    Task<ClientDto> GetById(int userId, int clientId);
    Task<bool> Update(int userId, ClientDto clientDto);
    Task<List<IntListItem>> GetAsListItems(int userId);
  }

  public class ClientService : IClientService
  {
    private readonly ILoggerAdapter<ClientService> _logger;
    private readonly IMetricService _metrics;
    private readonly IClientRepo _clientRepo;

    public ClientService(
      ILoggerAdapter<ClientService> logger,
      IClientRepo clientRepo,
      IMetricService metrics)
    {
      _logger = logger;
      _clientRepo = clientRepo;
      _metrics = metrics;
    }

    public async Task<List<ClientDto>> GetAll(int userId)
    {
      // TODO: [TESTS] (ClientService.GetAll) Add tests
      var builder = new ServiceMetricBuilder(nameof(ClientService), nameof(GetAll))
        .WithCategory(MetricCategory.Client, MetricSubCategory.GetAll)
        .WithCustomInt1(userId);

      try
      {
        using (builder.WithTiming())
        {
          List<ClientEntity> dbClients;
          using (builder.WithCustomTiming1())
          {
            builder.IncrementQueryCount();
            dbClients = await _clientRepo.GetAll(userId);
            builder.WithResultsCount(dbClients?.Count ?? 0);
          }

          if (dbClients == null || dbClients.Count == 0)
            return new List<ClientDto>();

          return dbClients
            .AsQueryable()
            .Select(ClientDto.Projection)
            .ToList();
        }
      }
      catch (Exception ex)
      {
        _logger.LogUnexpectedException(ex);
        builder.WithException(ex);
        return new List<ClientDto>();
      }
      finally
      {
        await _metrics.SubmitPointAsync(builder.Build());
      }
    }

    public async Task<bool> AddClient(int userId, ClientDto clientDto)
    {
      // TODO: [TESTS] (ClientService.AddClient) Add tests
      var builder = new ServiceMetricBuilder(nameof(ClientService), nameof(AddClient))
        .WithCategory(MetricCategory.Client, MetricSubCategory.Add)
        .WithCustomInt1(userId);

      try
      {
        using (builder.WithTiming())
        {
          var clientEntity = clientDto.ToDbEntity();
          clientEntity.UserId = userId;

          using (builder.WithCustomTiming1())
          {
            builder.IncrementQueryCount();
            if (await _clientRepo.Add(clientEntity) <= 0)
              return false;

            builder.WithResultsCount(1);
            return true;
          }
        }
      }
      catch (Exception ex)
      {
        _logger.LogUnexpectedException(ex);
        builder.WithException(ex);
        return false;
      }
      finally
      {
        await _metrics.SubmitPointAsync(builder.Build());
      }
    }

    public async Task<ClientDto> GetById(int userId, int clientId)
    {
      // TODO: [TESTS] (ClientService.GetById) Add tests
      var builder = new ServiceMetricBuilder(nameof(ClientService), nameof(GetById))
        .WithCategory(MetricCategory.Client, MetricSubCategory.GetById)
        .WithCustomInt1(userId)
        .WithCustomInt2(clientId);

      try
      {
        using (builder.WithTiming())
        {
          ClientEntity dbClient;
          using (builder.WithCustomTiming2())
          {
            builder.IncrementQueryCount();
            dbClient = await _clientRepo.GetById(clientId);
            builder.CountResult(dbClient);
          }

          if (dbClient == null)
            return null;

          // ReSharper disable once InvertIf
          if (dbClient.UserId != userId)
          {
            // TODO: [HANDLE] (ClientService.GetById) Handle this better
            _logger.Warning("Requested client '{cname}' ({cid}) does not belong to user ({uid})",
              dbClient.ClientName,
              dbClient.ClientId,
              userId
            );

            return null;
          }

          return ClientDto.FromEntity(dbClient);
        }
      }
      catch (Exception ex)
      {
        _logger.LogUnexpectedException(ex);
        builder.WithException(ex);
        return null;
      }
      finally
      {
        await _metrics.SubmitPointAsync(builder.Build());
      }
    }

    public async Task<bool> Update(int userId, ClientDto clientDto)
    {
      // TODO: [TESTS] (ClientService.Update) Add tests
      var builder = new ServiceMetricBuilder(nameof(ClientService), nameof(Update))
        .WithCategory(MetricCategory.Client, MetricSubCategory.Update)
        .WithCustomInt1(userId)
        .WithCustomInt2(clientDto.ClientId);

      try
      {
        using (builder.WithTiming())
        {
          ClientEntity dbEntry;
          using (builder.WithCustomTiming1())
          {
            builder.IncrementQueryCount();
            dbEntry = await _clientRepo.GetById(clientDto.ClientId);
            builder.CountResult(dbEntry);
          }

          if (dbEntry == null)
          {
            // TODO: [HANDLE] (ClientService.Update) Handle not found
            return false;
          }

          if (dbEntry.UserId != userId)
          {
            // TODO: [HANDLE] (ClientService.Update) Handle wrong owner
            return false;
          }

          using (builder.WithCustomTiming2())
          {
            builder.IncrementQueryCount();
            if (await _clientRepo.Update(clientDto.ToDbEntity()) <= 0)
              return false;

            builder.IncrementResultsCount();
            return true;
          }
        }
      }
      catch (Exception ex)
      {
        _logger.LogUnexpectedException(ex);
        builder.WithException(ex);
        return false;
      }
      finally
      {
        await _metrics.SubmitPointAsync(builder.Build());
      }
    }

    public async Task<List<IntListItem>> GetAsListItems(int userId)
    {
      // TODO: [TESTS] (ClientService.GetAsListItems) Add tests
      var builder = new ServiceMetricBuilder(nameof(ClientService), nameof(GetAsListItems))
        .WithCategory(MetricCategory.Client, MetricSubCategory.GetList)
        .WithCustomInt1(userId);

      try
      {
        using (builder.WithTiming())
        {
          List<ClientEntity> clientEntries;
          using (builder.WithCustomTiming1())
          {
            builder.IncrementQueryCount();
            clientEntries = await _clientRepo.GetAll(userId);
            builder.WithResultsCount(clientEntries.Count);
          }

          return clientEntries
            .AsQueryable()
            .Select(clientEntry => new IntListItem
            {
              Value = clientEntry.ClientId,
              Name = clientEntry.ClientName
            })
            .ToList();
        }
      }
      catch (Exception ex)
      {
        _logger.LogUnexpectedException(ex);
        builder.WithException(ex);
        return new List<IntListItem>();
      }
      finally
      {
        await _metrics.SubmitPointAsync(builder.Build());
      }
    }
  }
}
