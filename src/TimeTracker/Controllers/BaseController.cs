using System;
using System.Collections.Generic;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Rn.NetCore.Common.Logging;
using Rn.NetCore.Common.Metrics;
using TimeTracker.Core.Models.Dto;
using TimeTracker.Core.Models.Responses;
using TimeTracker.Core.Services;

namespace TimeTracker.Controllers
{
  public class BaseController<TController> : ControllerBase
  {
    public ILoggerAdapter<TController> Logger { get; }
    public IMetricService Metrics { get; }
    public IUserService UserService { get; }

    public BaseController(
      ILoggerAdapter<TController> logger,
      IMetricService metrics,
      IUserService userService)
    {
      // TODO: [TESTS] (BaseController) Add tests
      Logger = logger;
      Metrics = metrics;
      UserService = userService;
    }

    protected ActionResult<TResponse> ProcessResponse<TResponse>(BaseResponse<TResponse> response)
    {
      // TODO: [TESTS] (BaseController.ProcessResponse) Add tests
      if (response.FailedValidation)
      {
        return BadRequest(new ValidationError(response.ValidationResult));
      }

      ExtendUserSession();

      return Ok(response.Response);
    }

    // Internal methods
    private void ExtendUserSession()
    {
      // TODO: [TESTS] (BaseController.ExtendUserSession) Add tests
      if (!HttpContext.Items.ContainsKey("User"))
        return;

      if (!(HttpContext.Items["User"] is UserDto user))
        return;

      var token = UserService.ExtendUserSession(user.UserId);
      if (string.IsNullOrWhiteSpace(token))
        return;

      HttpContext.Response.Headers.Add("x-tt-session", token);
    }
  }

  public class ValidationError
  {
    public bool IsValid { get; set; }
    public List<string> Errors { get; set; }
    public string[] RuleSetsExecuted { get; set; }
    public string Error { get; set; }

    public ValidationError()
    {
      // TODO: [TESTS] (ValidationError) Add tests
      IsValid = false;
      Errors = new List<string>();
      RuleSetsExecuted = new string [0];
      Error = string.Empty;
    }

    public ValidationError(ValidationResult result)
      : this()
    {
      // TODO: [TESTS] (ValidationError) Add tests
      IsValid = result.IsValid;
      
      foreach (var error in result.Errors)
        Errors.Add(error.ToString());

      RuleSetsExecuted = result.RuleSetsExecuted;
      Error = result.ToString(Environment.NewLine);
    }
  }
}
