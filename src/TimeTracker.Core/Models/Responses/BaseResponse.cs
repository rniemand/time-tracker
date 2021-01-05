using FluentValidation.Results;

namespace TimeTracker.Core.Models.Responses
{
  public class BaseResponse<TResponse>
  {
    public TResponse Response { get; set; }
    public ValidationResult ValidationResult { get; set; }
    public bool FailedValidation => !(ValidationResult?.IsValid ?? true);
    public bool PassedValidation => ValidationResult?.IsValid ?? true;

    public BaseResponse()
    {
      // TODO: [TESTS] (BaseResponse) Add tests
      Response = default;
      ValidationResult = new ValidationResult();
    }

    public BaseResponse<TResponse> WithResponse(TResponse response)
    {
      Response = response;
      return this;
    }

    public BaseResponse<TResponse> WithValidation(ValidationResult result)
    {
      ValidationResult = result;
      return this;
    }
  }
}
