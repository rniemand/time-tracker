namespace TimeTracker.Core.Database.Queries
{
  public interface ITimeSheetDateRepoQueries
  {
    string GetDatesForRange();
  }

  public class TimeSheetDateRepoQueries : ITimeSheetDateRepoQueries
  {
    public string GetDatesForRange()
    {
      return @"SELECT *
      FROM `TimeSheet_Date`
      WHERE
	      `UserId` = @UserId AND
	      `Deleted` = 0 AND
	      `EntryDate` >= @StartDate AND
	      `EntryDate` <= @EndDate
      ORDER BY `ClientId`, `EntryDate` ASC";
    }
  }
}
