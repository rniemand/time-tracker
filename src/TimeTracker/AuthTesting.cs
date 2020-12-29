using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using TimeTracker.Core.Database.Entities;

namespace TimeTracker
{
  [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
  public class AuthorizeAttribute : Attribute, IAuthorizationFilter
  {
    public void OnAuthorization(AuthorizationFilterContext context)
    {
      // https://jasonwatmore.com/post/2019/10/11/aspnet-core-3-jwt-authentication-tutorial-with-example-api
      // TODO: [COMPLETE] (AuthorizeAttribute.OnAuthorization) Handle no user logged in

      var user = (UserEntity) context.HttpContext.Items["User"];

      if (user != null)
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
}
