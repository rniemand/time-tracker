﻿using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TimeTracker.Core.Models.Requests;
using TimeTracker.Core.Models.Responses;
using TimeTracker.Core.Services;

namespace TimeTracker.Controllers
{
  [ApiController, Route("api/[controller]")]
  public class AuthController : ControllerBase
  {
    private readonly IUserService _userService;

    public AuthController(IUserService userService)
    {
      _userService = userService;
    }

    [HttpPost, Route("authenticate")]
    public async Task<ActionResult<AuthenticationResponse>> Authenticate([FromBody] AuthenticationRequest request)
    {
      // TODO: [TESTS] (AuthController.Authenticate) Add tests
      var response = await _userService.Authenticate(request);

      if (response == null)
        return BadRequest(new
        {
          message = "Username or password is incorrect"
        });

      return Ok(response);
    }

    [HttpGet, Route("locked"), Authorize]
    public ActionResult<string> Locked()
    {
      return Ok("woot");
    }
  }
}
