namespace TimeTracker.Core.Database.Queries
{
  public interface ITimeSheetRowRepoQueries
  {
    string GetRow();
    string AddRow();
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
  }
}
