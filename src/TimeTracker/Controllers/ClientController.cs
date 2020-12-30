using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using TimeTracker.Core.Models.Dto;
using TimeTracker.Core.Services;
using TimeTracker.Core.WebApi.Attributes;
using TimeTracker.Core.WebApi.Requests;

namespace TimeTracker.Controllers
{
  [ApiController, Route("api/[controller]")]
  public class ClientController : ControllerBase
  {
    private readonly IClientService _clientService;

    public ClientController(IClientService clientService)
    {
      _clientService = clientService;
    }

    [HttpGet, Route("list-all"), Authorize]
    public async Task<ActionResult<List<ClientDto>>> GetAll([OpenApiIgnore] CoreApiRequest request)
    {
      // TODO: [TESTS] (ClientController.GetAll) Add tests
      return Ok(await _clientService.GetAll(request.UserId));
    }

    [HttpPost, Route(""), Authorize]
    public async Task<ActionResult<ClientDto>> AddClient(
      [FromBody] ClientDto clientDto,
      [OpenApiIgnore] CoreApiRequest request)
    {
      // TODO: [TESTS] (ClientController.AddClient) Add tests
      var addedClient = await _clientService.AddClient(request.UserId, clientDto);
      return Ok(addedClient);
    }
  }
}
