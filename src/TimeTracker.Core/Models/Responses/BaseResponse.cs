using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TimeTracker.Core.Models.Responses
{
  public class BaseResponse<TResponse>
  {
    public bool Succeeded { get; set; }
    public TResponse Response { get; set; }
    public ValidationOutcome Validation { get; set; }

    public BaseResponse()
    {
      // TODO: [TESTS] (BaseResponse) Add tests
      Succeeded = true;
      Response = default;
      Validation = new ValidationOutcome(typeof(TResponse).FullName);
    }

    public BaseResponse<TResponse> WithResponse(TResponse response)
    {
      Response = response;
      return this;
    }

    public BaseResponse<TResponse> AsFailure()
    {
      // TODO: [TESTS] (BaseResponse.AsFailure) Add tests
      Succeeded = false;
      return this;
    }

    public BaseResponse<TResponse> WithValidationError(string error)
    {
      Validation.AddError(error);
      return this;
    }

    public BaseResponse<TResponse> WithValidationMessage(string message)
    {
      // TODO: [TESTS] (BaseResponse.WithValidationMessage) Add tests
      Validation.SetMessage(message);
      return this;
    }
  }

  public class ValidationError
  {
    public string ModelName { get; set; }
    public string ModelFullName { get; set; }
    public List<string> Errors { get; set; }
    public string Message { get; set; }

    public ValidationError()
    {
      // TODO: [TESTS] (ValidationError) Add tests
      ModelName = string.Empty;
      ModelFullName = string.Empty;
      Errors = new List<string>();
      Message = string.Empty;
    }
  }

  public class ValidationOutcome
  {
    public List<string> Errors { get; }
    public string ModelName { get; }
    public string ModelFullName { get; }
    public string Message { get; private set; }

    public ValidationOutcome(string modelFullName)
    {
      // TODO: [TESTS] (ValidationOutcome) Add tests
      Errors = new List<string>();
      ModelFullName = modelFullName;
      ModelName = modelFullName.Split('.').Last();
    }

    public void AddError(string error)
    {
      // TODO: [TESTS] (ValidationOutcome.AddError) Add tests
      Errors.Add(error);
    }

    public void SetMessage(string message)
    {
      // TODO: [TESTS] (ValidationOutcome.SetMessage) Add tests
      Message = message;
    }

    public bool FailedValidation()
    {
      // TODO: [TESTS] (ValidationOutcome.FailedValidation) Add tests
      return Errors.Count > 0;
    }

    public ValidationError GenerateValidationError()
    {
      // TODO: [TESTS] (ValidationOutcome.GenerateValidationError) Add tests
      return new ValidationError
      {
        ModelName = ModelName,
        ModelFullName = ModelFullName,
        Errors = Errors,
        Message = string.IsNullOrWhiteSpace(Message)
          ? $"Model validation for \"{ModelName}\" failed"
          : Message
      };
    }
  }

  public class RichardResponse
  {
    public string Hello { get; set; }

    public RichardResponse()
    {
      Hello = string.Empty;
    }

    public RichardResponse(string hello)
      : this()
    {
      Hello = hello;
    }
  }
}
