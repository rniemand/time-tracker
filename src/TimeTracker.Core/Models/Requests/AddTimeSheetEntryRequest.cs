using System;
using FluentValidation;
using FluentValidation.Results;

namespace TimeTracker.Core.Models.Requests
{
  public class AddTimeSheetEntryRequest
  {
    public int ProjectId { get; set; }
    public int LoggedTimeMin { get; set; }
    public DateTime EntryDate { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }

    public AddTimeSheetEntryRequest()
    {
      // TODO: [TESTS] (AddTimeSheetEntryRequest) Add tests
      ProjectId = 0;
      LoggedTimeMin = 0;
      EntryDate = DateTime.Now;
      StartDate = DateTime.Now;
      EndDate = DateTime.Now.AddDays(7);
    }
  }

  public class UpdateTimeSheetEntryRequestValidator : AbstractValidator<AddTimeSheetEntryRequest>
  {
    public UpdateTimeSheetEntryRequestValidator()
    {
      RuleSet("Default", () =>
      {
        RuleFor(x => x.ProjectId).GreaterThan(0);
        RuleFor(x => x.EntryDate).NotNull();
        RuleFor(x => x.LoggedTimeMin).GreaterThanOrEqualTo(0).LessThan(60 * 24);
      });
    }

    public static ValidationResult Default(AddTimeSheetEntryRequest request)
    {
      return new UpdateTimeSheetEntryRequestValidator().Validate(request,
        options => options.IncludeRuleSets("Default")
      );
    }
  }
}
