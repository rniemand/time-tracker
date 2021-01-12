using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using Rn.NetCore.Common.Logging;
using Rn.NetCore.Common.Metrics;
using TimeTracker.Core.Models.Dto;
using TimeTracker.Core.Models.Responses;
using TimeTracker.Core.Services;
using TimeTracker.Core.WebApi;
using TimeTracker.Core.WebApi.Attributes;
using TimeTracker.Core.WebApi.Requests;

namespace TimeTracker.Controllers
{
  [ApiController, Route("api/[controller]")]
  public class ClientsController : BaseController<ClientsController>
  {
    private readonly IClientService _clientService;

    public ClientsController(
      ILoggerAdapter<ClientsController> logger,
      IMetricService metrics,
      IUserService userService,
      IClientService clientService
    ) : base(logger, metrics, userService)
    {
      _clientService = clientService;
    }

    [HttpPost, Route("client/add"), Authorize]
    public async Task<ActionResult<bool>> AddClient(
      [FromBody] ClientDto clientDto,
      [OpenApiIgnore] CoreApiRequest request)
    {
      // TODO: [TESTS] (ClientsController.AddClient) Add tests
      var response = new BaseResponse<bool>()
        .WithValidation(ClientDtoValidator.Add(clientDto));

      if (response.PassedValidation)
        response.WithResponse(await _clientService.AddClient(request.UserId, clientDto));

      return ProcessResponse(response);
    }

    [HttpGet, Route("client/{clientId}"), Authorize]
    public async Task<ActionResult<ClientDto>> GetClientById(
      [FromRoute] int clientId,
      [OpenApiIgnore] CoreApiRequest request)
    {
      // TODO: [TESTS] (ClientsController.GetClientById) Add tests
      var response = new BaseResponse<ClientDto>()
        .WithValidation(new AdHockValidator().GreaterThanZero(nameof(clientId), clientId));

      if (response.PassedValidation)
        response.WithResponse(await _clientService.GetById(
          request.UserId,
          clientId
        ));

      return ProcessResponse(response);
    }

    [HttpPatch, Route("client/update"), Authorize]
    public async Task<ActionResult<bool>> UpdateClient(
      [FromBody] ClientDto clientDto,
      [OpenApiIgnore] CoreApiRequest request)
    {
      // TODO: [TESTS] (ClientsController.UpdateClient) Add tests
      var response = new BaseResponse<bool>()
        .WithValidation(ClientDtoValidator.Update(clientDto));

      if (response.PassedValidation)
        response.WithResponse(await _clientService.Update(request.UserId, clientDto));

      return ProcessResponse(response);
    }

    [HttpGet, Route("clients"), Authorize]
    public async Task<ActionResult<List<ClientDto>>> GetAllClients(
      [OpenApiIgnore] CoreApiRequest request)
    {
      // TODO: [TESTS] (ClientsController.GetAllClients) Add tests
      var response = new BaseResponse<List<ClientDto>>()
        .WithResponse(await _clientService.GetAll(request.UserId));

      return ProcessResponse(response);
    }

    [HttpGet, Route("clients/list"), Authorize]
    public async Task<ActionResult<List<IntListItem>>> ListAllClients(
      [OpenApiIgnore] CoreApiRequest request)
    {
      // TODO: [TESTS] (ClientsController.ListAllClients) Add tests
      var response = new BaseResponse<List<IntListItem>>()
        .WithResponse(await _clientService.GetAsListItems(request.UserId));

      return ProcessResponse(response);
    }
  }
}
