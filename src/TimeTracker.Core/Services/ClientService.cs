using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Rn.NetCore.Common.Logging;
using TimeTracker.Core.Database.Repos;
using TimeTracker.Core.Models.Dto;

namespace TimeTracker.Core.Services
{
  public interface IClientService
  {
    Task<List<ClientDto>> GetAll(int userId);
    Task<ClientDto> AddClient(int userId, ClientDto clientDto);
    Task<ClientDto> GetById(int userId, int clientId);
    Task<ClientDto> Update(int userId, ClientDto clientDto);
    Task<List<IntListItem>> GetAsListItems(int userId);
  }

  public class ClientService : IClientService
  {
    private readonly ILoggerAdapter<ClientService> _logger;
    private readonly IClientRepo _clientRepo;

    public ClientService(
      ILoggerAdapter<ClientService> logger,
      IClientRepo clientRepo)
    {
      _logger = logger;
      _clientRepo = clientRepo;
    }

    public async Task<List<ClientDto>> GetAll(int userId)
    {
      // TODO: [TESTS] (ClientService.GetAll) Add tests
      var dbClients = await _clientRepo.GetAll(userId);

      if (dbClients == null || dbClients.Count == 0)
        return new List<ClientDto>();

      return dbClients
        .AsQueryable()
        .Select(ClientDto.Projection)
        .ToList();
    }

    public async Task<ClientDto> AddClient(int userId, ClientDto clientDto)
    {
      // TODO: [TESTS] (ClientService.AddClient) Add tests
      // TODO: [DUPLICATED] (ClientService.AddClient) Guard against

      var clientEntity = clientDto.ToDbEntity();
      clientEntity.UserId = userId;

      await _clientRepo.Add(clientEntity);
      var dbEntry = await _clientRepo.GetByName(userId, clientDto.ClientName);
      return ClientDto.FromEntity(dbEntry);
    }

    public async Task<ClientDto> GetById(int userId, int clientId)
    {
      // TODO: [TESTS] (ClientService.GetById) Add tests
      var dbClient = await _clientRepo.GetById(clientId);
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

    public async Task<ClientDto> Update(int userId, ClientDto clientDto)
    {
      // TODO: [TESTS] (ClientService.Update) Add tests
      var dbEntry = await _clientRepo.GetById(clientDto.ClientId);
      if (dbEntry == null)
      {
        // TODO: [HANDLE] (ClientService.Update) Handle not found
        return null;
      }

      if (dbEntry.UserId != userId)
      {
        // TODO: [HANDLE] (ClientService.Update) Handle wrong owner
        return null;
      }

      await _clientRepo.Update(clientDto.ToDbEntity());

      return ClientDto.FromEntity(
        await _clientRepo.GetById(clientDto.ClientId)
      );
    }

    public async Task<List<IntListItem>> GetAsListItems(int userId)
    {
      // TODO: [TESTS] (ClientService.GetAsListItems) Add tests
      var clientEntries = await _clientRepo.GetAll(userId);

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
}
