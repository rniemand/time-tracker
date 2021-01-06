using System.Collections.Generic;
using FluentValidation.Results;

namespace TimeTracker.Core.WebApi
{
  public class AdHockValidator
  {
    public List<ValidationFailure> Errors { get; set; }

    public AdHockValidator()
    {
      // TODO: [TESTS] (AdHockValidator) Add tests
      Errors = new List<ValidationFailure>();
    }

    public AdHockValidator GreaterThanZero(string property, long actual)
    {
      return GreaterThan(property, 0, actual);
    }

    public AdHockValidator GreaterThan(string property, long amount, long actual)
    {
      if (actual <= amount)
      {
        Errors.Add(new ValidationFailure(
          property,
          $"'{property}' must be greater than '{amount}' (was {actual})",
          actual
        ));
      }

      return this;
    }

    public AdHockValidator NotNullOrWhiteSpace(string property, string actual)
    {
      if (string.IsNullOrWhiteSpace(actual))
      {
        Errors.Add(new ValidationFailure(
          property,
          $"'{property}' cannot be null or white space"
        ));
      }

      return this;
    }

    public ValidationResult Validate()
    {
      return new ValidationResult(Errors);
    }
  }
}