using System;
using FluentValidation;
using FluentValidation.Results;

namespace TimeTracker.Core.Models.Requests
{
  public class AddTimeSheetEntryRequest
  {
    public int UserId { get; set; }
    public int ProjectId { get; set; }
    public int LoggedTimeMin { get; set; }
    public DateTime EntryDate { get; set; }

    public AddTimeSheetEntryRequest()
    {
      // TODO: [TESTS] (AddTimeSheetEntryRequest) Add tests
      UserId = 0;
      ProjectId = 0;
      LoggedTimeMin = 0;
      EntryDate = DateTime.Now;
    }
  }

  public class UpdateTimeSheetEntryRequestValidator : AbstractValidator<AddTimeSheetEntryRequest>
  {
    public UpdateTimeSheetEntryRequestValidator()
    {
      RuleSet("Default", () =>
      {
        RuleFor(x => x.UserId).GreaterThan(0);
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
