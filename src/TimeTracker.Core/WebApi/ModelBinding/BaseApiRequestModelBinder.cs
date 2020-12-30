using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Rn.NetCore.Common.Helpers;
using Rn.NetCore.Common.Logging;
using TimeTracker.Core.Models.Dto;
using TimeTracker.Core.WebApi.Models;

namespace TimeTracker.Core.WebApi.ModelBinding
{
  public class BaseApiRequestModelBinder : IModelBinder
  {
    private readonly ILoggerAdapter<BaseApiRequestModelBinder> _logger;
    private readonly IJsonHelper _jsonHelper;

    public BaseApiRequestModelBinder(
      ILoggerAdapter<BaseApiRequestModelBinder> logger,
      IJsonHelper jsonHelper)
    {
      _logger = logger;
      _jsonHelper = jsonHelper;
    }

    public async Task BindModelAsync(ModelBindingContext bindingContext)
    {
      // TODO: [TESTS] (BaseApiRequestModelBinder.BindModelAsync) Add tests
      if (bindingContext == null)
      {
        throw new ArgumentNullException(nameof(bindingContext));
      }

      var bodyJson = await GetBody(bindingContext);
      var modelType = bindingContext.ModelType;
      var model = _jsonHelper.DeserializeObject(bodyJson, modelType);
      AppendUser(model as BaseApiRequest, bindingContext);

      bindingContext.Result = ModelBindingResult.Success(model);
    }

    // Helper methods
    private async Task<string> GetBody(ModelBindingContext bindingContext)
    {
      // TODO: [TESTS] (TestingBinder.GetBody) Add tests

      try
      {
        using var reader = new StreamReader(
          bindingContext.ActionContext.HttpContext.Request.Body,
          Encoding.UTF8
        );

        var rawBody = await reader.ReadToEndAsync();
        return string.IsNullOrWhiteSpace(rawBody) ? "{}" : rawBody;
      }
      catch (Exception ex)
      {
        _logger.Error(ex, "Unable to read body: {msg}", ex.Message);
        return "{}";
      }
    }

    private static void AppendUser(BaseApiRequest model, ModelBindingContext bindingContext)
    {
      if (bindingContext.HttpContext.Items.ContainsKey("User"))
      {
        model.User = (UserDto)bindingContext.HttpContext.Items["User"];
      }

      model.UserId = model?.User?.UserId ?? 0;
    }
  }
}