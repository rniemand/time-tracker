using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using TimeTracker.Core.Models.Dto;
using TimeTracker.Core.Services;

namespace TimeTracker
{
  [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
  public class AuthorizeAttribute : Attribute, IAuthorizationFilter
  {
    public void OnAuthorization(AuthorizationFilterContext context)
    {
      // https://jasonwatmore.com/post/2019/10/11/aspnet-core-3-jwt-authentication-tutorial-with-example-api
      // TODO: [COMPLETE] (AuthorizeAttribute.OnAuthorization) Handle no user logged in

      var user = (UserDto)context.HttpContext.Items["User"];

      if (user == null)
      {
        // not logged in
        context.Result = new JsonResult(new
        {
          message = "Unauthorized"
        })
        {
          StatusCode = StatusCodes.Status401Unauthorized
        };
      }
    }
  }

  public class JwtMiddleware
  {
    private readonly RequestDelegate _next;

    public JwtMiddleware(RequestDelegate next)
    {
      _next = next;
    }

    public async Task Invoke(HttpContext context, IUserService userService)
    {
      var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

      if (token != null)
        await AttachUserToContext(context, userService, token);

      await _next(context);
    }

    private static async Task AttachUserToContext(HttpContext context, IUserService userService, string token)
    {
      try
      {
        context.Items["User"] = await userService.GetFromToken(token);
      }
      catch
      {
        // do nothing if jwt validation fails
        // user is not attached to context so request won't have access to secure routes
      }
    }
  }

}
