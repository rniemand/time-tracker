using System;
using System.Collections.Generic;
using FluentValidation.Results;

namespace TimeTracker.Core.Models
{
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