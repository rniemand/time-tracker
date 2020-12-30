using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TimeTracker.Core.Models.Dto;
using TimeTracker.Core.Services;

namespace TimeTracker.Controllers
{
  [ApiController, Route("api/[controller]")]
  public class ClientsController : ControllerBase
  {
    private readonly IClientService _clientService;

    public ClientsController(IClientService clientService)
    {
      _clientService = clientService;
    }

    [HttpPost, Route(""), Authorize]
    public async Task<ActionResult<List<ClientDto>>> GetAllClients([FromBody] DerivedTestModel test)
    {
      return Ok(await _clientService.GetAll(test.UserId));
    }
  }
}
