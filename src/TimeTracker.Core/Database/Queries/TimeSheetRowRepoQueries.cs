namespace TimeTracker.Core.Database.Queries
{
  public interface ITimeSheetRowRepoQueries
  {
    string GetRow();
    string AddRow();
    string GetProjectRows();
    string GetClientRows();
    string GetTimeSheetProjects();
    string GetTimeSheetProducts();
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
	      (`DateId`, `UserId`, `ClientId`, `ProductId`, `ProjectId`, `EntryDate`)
      VALUES
	      (@DateId, @UserId, @ClientId, @ProductId, @ProjectId, @EntryDate)";
    }

    public string GetProjectRows()
    {
      return @"SELECT *
      FROM `TimeSheet_Rows`
      WHERE
	      `Deleted` = 0 AND
	      `ProjectId` = @ProjectId AND
        `EntryDate` >= @FromDate AND
        `EntryDate` <= @ToDate
      ORDER BY `DateId` ASC";
    }

    public string GetClientRows()
    {
      return @"SELECT *
      FROM `TimeSheet_Rows`
      WHERE
	      `Deleted` = 0 AND
	      `ClientId` = @ClientId AND
        `EntryDate` >= @FromDate AND
        `EntryDate` <= @ToDate
      ORDER BY `DateId` DESC";
    }

    public string GetTimeSheetProjects()
    {
      return @"SELECT *
      FROM `Projects`
      WHERE
	      `Deleted` = 0 AND
	      `ProjectId` IN (
		      SELECT DISTINCT(`ProjectId`)
		      FROM `TimeSheet_Rows`
		      WHERE
			      `Deleted` = 0 AND
			      `ClientId` = @ClientId AND
            `EntryDate` >= @FromDate AND
            `EntryDate` <= @ToDate
	      )";
    }

    public string GetTimeSheetProducts()
    {
      return @"SELECT *
      FROM `Products`
      WHERE `ProductId` IN (
         SELECT DISTINCT(`ProductId`)
         FROM `TimeSheet_Rows`
         WHERE
            `Deleted` = 0 AND
            `ClientId` = @ClientId AND
            `EntryDate` >= @FromDate AND
            `EntryDate` <= @ToDate
      )";
    }
  }
}
