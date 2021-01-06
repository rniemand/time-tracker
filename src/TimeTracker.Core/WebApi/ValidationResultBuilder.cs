using System.Collections.Generic;
using FluentValidation.Results;

namespace TimeTracker.Core.WebApi
{
  public class ValidationResultBuilder
  {
    public List<ValidationFailure> Errors { get; set; }

    public ValidationResultBuilder()
    {
      // TODO: [TESTS] (ValidationResultBuilder) Add tests
      Errors = new List<ValidationFailure>();
    }

    public ValidationResultBuilder MustBeGreaterThanZero(string property)
    {
      Errors.Add(new ValidationFailure(
        property,
        $"'{property}' must be greater than 0."
      ));
      return this;
    }

    public ValidationResult Build()
    {
      return new ValidationResult(Errors);
    }
  }
}