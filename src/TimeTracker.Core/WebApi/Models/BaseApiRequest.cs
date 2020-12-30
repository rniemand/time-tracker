using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using TimeTracker.Core.Models.Dto;
using TimeTracker.Core.WebApi.ModelBinding;

namespace TimeTracker.Core.WebApi.Models
{
  [ModelBinder(BinderType = typeof(BaseApiRequestModelBinder))]
  public abstract class BaseApiRequest
  {
    [OpenApiIgnore]
    public UserDto User { get; set; }

    [OpenApiIgnore]
    public int UserId { get; set; }

    protected BaseApiRequest()
    {
      // TODO: [TESTS] (BaseApiRequest) Add tests
      User = null;
      UserId = 0;
    }
  }
}