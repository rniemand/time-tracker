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
      
      if(dbClients == null || dbClients.Count == 0)
        return new List<ClientDto>();

      return dbClients
        .AsQueryable()
        .Select(ClientDto.Projection)
        .ToList();
    }
  }
}
