namespace TimeTracker.Core.Database.Queries
{
  public interface ITimeSheetDateRepoQueries
  {
    string GetUserDates();
    string GetClientDates();
    string GetEntry();
    string Add();
    string GetById();
  }

  public class TimeSheetDateRepoQueries : ITimeSheetDateRepoQueries
  {
    public string GetUserDates()
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

    public string GetClientDates()
    {
      return @"SELECT *
      FROM `TimeSheet_Date`
      WHERE
	      `ClientId` = @ClientId AND
	      `Deleted` = 0 AND
	      `EntryDate` >= @StartDate AND
	      `EntryDate` <= @EndDate
      ORDER BY `ClientId`, `EntryDate` ASC";
    }

    public string GetEntry()
    {
      return @"SELECT *
      FROM `TimeSheet_Date`
      WHERE
	      `UserId` = @UserId AND
	      `ClientId` = @ClientId AND
	      `Deleted` = 0 AND
	      `EntryDate` = @EntryDate";
    }

    public string Add()
    {
      return @"INSERT INTO `TimeSheet_Date`
	      (`UserId`, `ClientId`, `EntryDate`, `DayOfWeek`)
      VALUES
	      (@UserId, @ClientId, @EntryDate, @DayOfWeek)";
    }

    public string GetById()
    {
      return @"SELECT *
      FROM `TimeSheet_Date`
      WHERE `DateId` = @DateId";
    }
  }
}
