namespace TimeTracker.Core.Database.Queries
{
  public interface ITimeSheetRowRepoQueries
  {
    string GetRow();
    string AddRow();
    string GetRows();
  }

  public class TimeSheetRowRepoQueries : ITimeSheetRowRepoQueries
  {
    public string GetRow()
    {
      return @"SELECT *
      FROM `TimeSheet_Rows`
      WHERE
	      `Deleted` = 0 AND
	      `DateId` = @DateId AND
	      `UserId` = @UserId AND
	      `ClientId` = @ClientId AND
	      `ProductId` = @ProductId AND
	      `ProjectId` = @ProjectId";
    }

    public string AddRow()
    {
      return @"INSERT INTO `TimeSheet_Rows`
	      (`DateId`, `UserId`, `ClientId`, `ProductId`, `ProjectId`)
      VALUES
	      (@DateId, @UserId, @ClientId, @ProductId, @ProjectId)";
    }

    public string GetRows()
    {
      return @"SELECT *
      FROM `TimeSheet_Rows`
      WHERE
	      `Deleted` = 0 AND
	      `ProjectId` = @ProjectId AND
	      `DateId` IN (
		      SELECT `DateId`
		      FROM `TimeSheet_Date`
		      WHERE
			      `Deleted` = 0 AND
			      `ClientId` = 1 AND
			      `EntryDate` >= @FromDate AND
			      `EntryDate` <= @ToDate
	      )
      ORDER BY `DateId` ASC";
    }
  }
}
