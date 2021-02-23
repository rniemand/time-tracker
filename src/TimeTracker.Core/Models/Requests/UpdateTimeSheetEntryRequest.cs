using FluentValidation;
using FluentValidation.Results;

namespace TimeTracker.Core.Models.Requests
{
  public class UpdateTimeSheetEntryRequest
  {
    public int DateId { get; set; }
    public int UserId { get; set; }
    public int ProjectId { get; set; }
    public int LoggedTimeMin { get; set; }

    public UpdateTimeSheetEntryRequest()
    {
      // TODO: [TESTS] (UpdateTimeSheetEntryRequest) Add tests
      DateId = 0;
      UserId = 0;
      ProjectId = 0;
      LoggedTimeMin = 0;
    }
  }

  public class UpdateTimeSheetEntryRequestValidator : AbstractValidator<UpdateTimeSheetEntryRequest>
  {
    public UpdateTimeSheetEntryRequestValidator()
    {
      RuleSet("Default", () =>
      {
        RuleFor(x => x.UserId).GreaterThan(0);
        RuleFor(x => x.DateId).GreaterThan(0);
        RuleFor(x => x.ProjectId).GreaterThan(0);
        RuleFor(x => x.LoggedTimeMin).GreaterThanOrEqualTo(0).LessThan(60 * 24);
      });
    }

    public static ValidationResult Default(UpdateTimeSheetEntryRequest request)
    {
      return new UpdateTimeSheetEntryRequestValidator().Validate(request,
        options => options.IncludeRuleSets("Default")
      );
    }
  }
}
