using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Rn.NetCore.Common.Logging;
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

  [ModelBinder(BinderType = typeof(TestModelBinder))]
  public class TestModel
  {
    public UserDto User { get; set; }
    public int UserId { get; set; }

    public TestModel()
    {
      // TODO: [TESTS] (TestModel) Add tests
      User = null;
      UserId = 0;
    }
  }

  public class TestModelBinder : IModelBinder
  {
    private readonly ILoggerAdapter<TestModelBinder> _logger;

    public TestModelBinder(ILoggerAdapter<TestModelBinder> logger)
    {
      _logger = logger;
    }

    public async Task BindModelAsync(ModelBindingContext bindingContext)
    {
      // TODO: [TESTS] (TestModelBinder.BindModelAsync) Add tests
      if (bindingContext == null)
      {
        throw new ArgumentNullException(nameof(bindingContext));
      }

      var model = new TestModel();

      if (bindingContext.HttpContext.Items.ContainsKey("User"))
      {
        model.User = (UserDto) bindingContext.HttpContext.Items["User"];
      }

      model.UserId = model?.User?.UserId ?? 0;
      await Task.CompletedTask;

      bindingContext.Result = ModelBindingResult.Success(model);
    }
  }
}
