namespace TimeTracker.Core.Database.Queries
{
  public interface IRawTrackedTimeRepoQueries
  {
    string StartNew();
    string GetCurrentEntry();
  }

  public class RawRawTrackedTimeRepoQueries : IRawTrackedTimeRepoQueries
  {
    public string StartNew()
    {
      return @"INSERT INTO `RawTrackedTime`
	      (`ParentEntryId`, `RootParentEntryId`, `ClientId`, `ProductId`, `ProjectId`, `UserId`, `EntryState`)
      VALUES
	      (@ParentEntryId, @RootParentEntryId, @ClientId, @ProductId, @ProjectId, @UserId, @EntryState)";
    }

    public string GetCurrentEntry()
    {
      return @"SELECT *
      FROM `RawTrackedTime`
      WHERE
	      `ParentEntryId` = @ParentEntryId AND
	      `RootParentEntryId` = @RootParentEntryId AND
	      `ClientId` = @ClientId AND
	      `ProductId` = @ProductId AND
	      `ProjectId` = @ProjectId AND
	      `UserId` = @UserId AND
	      `Deleted` = 0 AND
	      `EntryState` = @EntryState
      LIMIT 1";
    }
  }
}
