namespace TimeTracker.Core.Database.Queries
{
  public interface IRawTimersRepoQueries
  {
    string StartNew();
    string GetCurrentEntry();
    string GetRunningTimers();
    string PauseTimer();
    string GetByRawTimerId();
    string SpawnResumedTimer();
    string FlagAsResumed();
    string SetRootTimerId();
    string StopTimer();
    string CompleteTimerSet();
  }

  public class RawTimersRepoQueries : IRawTimersRepoQueries
  {
    public string StartNew()
    {
      return @"INSERT INTO `RawTimers`
	      (`ParentTimerId`, `RootTimerId`, `ClientId`, `ProductId`, `ProjectId`, `UserId`, `EntryState`, `Running`)
      VALUES
	      (@ParentTimerId, @RootTimerId, @ClientId, @ProductId, @ProjectId, @UserId, @EntryState, 1)";
    }

    public string GetCurrentEntry()
    {
      return @"SELECT *
      FROM `RawTimers`
      WHERE
	      `ParentTimerId` = @ParentTimerId AND
	      `RootTimerId` = @RootTimerId AND
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
      FROM `RawTimers` rtt
	      INNER JOIN `Products` prod ON rtt.`ProductId` = prod.`ProductId`
	      INNER JOIN `Projects` proj ON rtt.`ProjectId` = proj.`ProjectId`
	      INNER JOIN `Clients` cli ON prod.`ClientId` = cli.`ClientId`
      WHERE
	      rtt.`Deleted` = 0 AND
	      rtt.`UserId` = @UserId AND
	      rtt.`Completed` = 0 AND
        rtt.`Running` = 1
      ORDER BY `RootTimerId`, `EntryStartTimeUtc` ASC";
    }

    public string PauseTimer()
    {
      return @"UPDATE `RawTimers`
      SET
	      `Running` = 1,
	      `EntryState` = 2,
        `Completed` = 0,
	      `EntryEndTimeUtc` = CURRENT_TIMESTAMP(),
	      `EntryRunningTimeSec` = TIME_TO_SEC(TIMEDIFF(CURRENT_TIMESTAMP(), `EntryStartTimeUtc`))
      WHERE
	      `RawTimerId` = @RawTimerId";
    }

    public string GetByRawTimerId()
    {
      return @"SELECT *
      FROM `RawTimers`
      WHERE
	      `RawTimerId` = @RawTimerId";
    }

    public string SpawnResumedTimer()
    {
      return @"INSERT INTO `RawTimers`
	      (
          `ParentTimerId`, `RootTimerId`, `ClientId`, `ProductId`,
          `ProjectId`, `UserId`, `Running`, `EntryState`, `Completed`
        )
      VALUES
	      (
          @ParentTimerId, @RootTimerId, @ClientId, @ProductId,
          @ProjectId, @UserId, @Running, @EntryState, @Completed
        )";
    }

    public string FlagAsResumed()
    {
      return @"UPDATE `RawTimers`
      SET
	      `Running` = 0,
	      `Completed` = 0
      WHERE
	      `RawTimerId` = @RawTimerId";
    }

    public string SetRootTimerId()
    {
      return @"UPDATE `RawTimers`
      SET
	      `RootTimerId` = @RootTimerId
      WHERE
	      `RawTimerId` = @RawTimerId";
    }

    public string StopTimer()
    {
      return @"UPDATE `RawTimers`
      SET
         `Running` = 0,
         `EntryState` = 3,
	      `Completed` = 0,
         `EntryEndTimeUtc` = CURRENT_TIMESTAMP(),
         `EntryRunningTimeSec` = TIME_TO_SEC(TIMEDIFF(CURRENT_TIMESTAMP(), `EntryStartTimeUtc`))
      WHERE
         `RawTimerId` = @RawTimerId";
    }

    public string CompleteTimerSet()
    {
      return @"UPDATE `RawTimers`
      SET
	      `Running` = 0,
	      `Completed` = 1
      WHERE
	      `RootTimerId` = @RootTimerId";
    }
  }
}
