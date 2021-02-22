using System;
using FluentValidation;
using FluentValidation.Results;

namespace TimeTracker.Core.Models.Requests
{
  public class AddTimeSheetRowRequest
  {
    public DateTime StartDate { get; set; }
    public int NumberDays { get; set; }
    public int UserId { get; set; }
    public int ClientId { get; set; }
    public int ProductId { get; set; }
    public int ProjectId { get; set; }

    public AddTimeSheetRowRequest()
    {
      // TODO: [TESTS] (AddTimeSheetRowRequest) Add tests 
      StartDate = DateTime.Now;
      NumberDays = 8;
      UserId = 0;
      ClientId = 0;
      ProductId = 0;
      ProductId = 0;
    }
  }

  public class AddTimeSheetRowRequestValidator : AbstractValidator<AddTimeSheetRowRequest>
  {
    public AddTimeSheetRowRequestValidator()
    {
      RuleSet("Add", () =>
      {
        RuleFor(x => x.UserId).GreaterThan(0);
        RuleFor(x => x.ClientId).GreaterThan(0);
        RuleFor(x => x.ProductId).GreaterThan(0);
        RuleFor(x => x.ProjectId).GreaterThan(0);
        RuleFor(x => x.NumberDays).GreaterThan(0).LessThan(15);
        RuleFor(x => x.StartDate)
          .NotNull()
          .GreaterThan(DateTime.Now.AddYears(-1))
          .LessThan(DateTime.Now.AddYears(1));
      });
    }

    public static ValidationResult Update(AddTimeSheetRowRequest projectDto)
    {
      return new AddTimeSheetRowRequestValidator().Validate(projectDto,
        options => options.IncludeRuleSets("Add")
      );
    }
  }
}