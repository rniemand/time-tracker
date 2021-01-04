namespace TimeTracker.Core.Database.Queries
{
  public interface IRawTrackedTimeRepoQueries
  {
    string StartNew();
    string GetCurrentEntry();
    string GetRunningTimers();
    string PauseTimer();
    string GetByEntryId();
    string SpawnResumedTimer();
    string FlagAsResumed();
    string SetRootParentEntryId();
    string StopTimer();
    string CompleteTimer();
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
	      rtt.`Completed` = 0 AND
        rtt.`Running` = 1
      ORDER BY `RootParentEntryId`, `EntryStartTimeUtc` ASC";
    }

    public string PauseTimer()
    {
      return @"UPDATE `RawTrackedTime`
      SET
	      `Running` = 1,
	      `EntryState` = 2,
        `Completed` = 0,
	      `EntryEndTimeUtc` = CURRENT_TIMESTAMP(),
	      `EntryRunningTimeSec` = TIME_TO_SEC(TIMEDIFF(CURRENT_TIMESTAMP(), `EntryStartTimeUtc`))
      WHERE
	      `EntryId` = @EntryId";
    }

    public string GetByEntryId()
    {
      return @"SELECT *
      FROM `RawTrackedTime`
      WHERE
	      `EntryId` = @EntryId";
    }

    public string SpawnResumedTimer()
    {
      return @"INSERT INTO `RawTrackedTime`
	      (
          `ParentEntryId`, `RootParentEntryId`, `ClientId`, `ProductId`,
          `ProjectId`, `UserId`, `Running`, `EntryState`, `Completed`
        )
      VALUES
	      (
          @ParentEntryId, @RootParentEntryId, @ClientId, @ProductId,
          @ProjectId, @UserId, @Running, @EntryState, @Completed
        )";
    }

    public string FlagAsResumed()
    {
      return @"UPDATE `RawTrackedTime`
      SET
	      `Running` = 0,
	      `Completed` = 0
      WHERE
	      `EntryId` = @EntryId";
    }

    public string SetRootParentEntryId()
    {
      return @"UPDATE `RawTrackedTime`
      SET
	      `RootParentEntryId` = @RootParentEntryId
      WHERE
	      `EntryId` = @EntryId";
    }

    public string StopTimer()
    {
      return @"UPDATE `RawTrackedTime`
      SET
         `Running` = 0,
         `EntryState` = 3,
	      `Completed` = 0,
         `EntryEndTimeUtc` = CURRENT_TIMESTAMP(),
         `EntryRunningTimeSec` = TIME_TO_SEC(TIMEDIFF(CURRENT_TIMESTAMP(), `EntryStartTimeUtc`))
      WHERE
         `EntryId` = @EntryId";
    }

    public string CompleteTimer()
    {
      return @"UPDATE `RawTrackedTime`
      SET
	      `Running` = 0,
	      `Completed` = 1
      WHERE
	      `RootParentEntryId` = @RootParentEntryId";
    }
  }
}
