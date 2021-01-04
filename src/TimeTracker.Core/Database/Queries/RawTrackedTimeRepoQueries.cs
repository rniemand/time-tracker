namespace TimeTracker.Core.Database.Queries
{
  public interface IRawTrackedTimeRepoQueries
  {
    string StartNew();
    string GetCurrentEntry();
    string GetRunningTimers();
  }

  public class RawRawTrackedTimeRepoQueries : IRawTrackedTimeRepoQueries
  {
    public string StartNew()
    {
      return @"INSERT INTO `RawTrackedTime`
	      (`ParentEntryId`, `RootParentEntryId`, `ClientId`, `ProductId`, `ProjectId`, `UserId`, `EntryState`, `Running`)
      VALUES
	      (@ParentEntryId, @RootParentEntryId, @ClientId, @ProductId, @ProjectId, @UserId, @EntryState, 1)";
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

    public string GetRunningTimers()
    {
      return @"SELECT
	      rtt.*,
	      prod.`ProductName`,
	      proj.`ProjectName`,
	      cli.`ClientName`
      FROM `RawTrackedTime` rtt
	      INNER JOIN `Products` prod ON rtt.`ProductId` = prod.`ProductId`
	      INNER JOIN `Projects` proj ON rtt.`ProjectId` = proj.`ProjectId`
	      INNER JOIN `Clients` cli ON prod.`ClientId` = cli.`ClientId`
      WHERE
	      rtt.`Deleted` = 0 AND
	      rtt.`UserId` = @UserId AND
	      rtt.`Running` = 1
      ORDER BY `EntryStartTimeUtc` ASC";
    }
  }
}
