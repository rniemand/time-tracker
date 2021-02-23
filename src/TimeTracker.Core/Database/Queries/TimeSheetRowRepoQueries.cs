namespace TimeTracker.Core.Database.Queries
{
  public interface ITimeSheetRowRepoQueries
  {
    string GetRow();
    string AddRow();
    string GetProjectRows();
    string GetClientRows();
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

    public string GetProjectRows()
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
			      `ClientId` = (SELECT `ClientId` FROM `Projects` WHERE `ProjectId` = @ProjectId) AND
			      `EntryDate` >= @FromDate AND
			      `EntryDate` <= @ToDate
	      )
      ORDER BY `DateId` ASC";
    }

    public string GetClientRows()
    {
      return @"SELECT *
      FROM `TimeSheet_Rows`
      WHERE
	      `Deleted` = 0 AND
	      `ClientId` = @ClientId AND
	      `DateId` IN (
		      SELECT `DateId`
		      FROM `TimeSheet_Date`
		      WHERE
			      `ClientId` = @ClientId AND
			      `Deleted` = 0 AND
			      `EntryDate` >= @FromDate AND
			      `EntryDate` <= @ToDate
	      )
      ORDER BY `DateId` DESC";
    }
  }
}
