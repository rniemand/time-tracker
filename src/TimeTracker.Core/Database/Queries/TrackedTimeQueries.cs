namespace TimeTracker.Core.Database.Queries
{
  public interface ITrackedTimeQueries
  {
    string StartNew();
    string GetExisting();
    string GetActive();


    string GetCurrentEntry();
    string PauseTimer();
    string GetByRawTimerId();
    string SpawnResumedTimer();
    string FlagAsResumed();
    string SetRootTimerId();
    string StopTimer();
    string CompleteTimerSet();
    string GetTimerSeries();
    string GetUsersWithRunningTimers();
    string GetLongRunningTimers();
    string UpdateNotes();
    string UpdateTimerDuration();
    string GetRunningTimers();
  }

  public class TrackedTimeQueries : ITrackedTimeQueries
  {
    public string StartNew()
    {
      return @"INSERT INTO `TrackedTime`
	      (`ClientId`, `ProductId`, `ProjectId`, `UserId`)
      VALUES
	      (@ClientId, @ProductId, @ProjectId, @UserId)";
    }

    public string GetExisting()
    {
      return @"SELECT *
      FROM `TrackedTime`
      WHERE
	      `ClientId` = @ClientId AND
	      `ProductId` = @ProductId AND
	      `ProjectId` = @ProjectId AND
	      `UserId` = @UserId AND
	      `Deleted` = 0 AND
	      `Running` = 1";
    }

    public string GetActive()
    {
      return @"SELECT
	      tt.*,
	      prod.`ProductName`,
	      proj.`ProjectName`,
	      cli.`ClientName`
      FROM `TrackedTime` tt
	      INNER JOIN `Products` prod ON tt.`ProductId` = prod.`ProductId`
	      INNER JOIN `Projects` proj ON tt.`ProjectId` = proj.`ProjectId`
	      INNER JOIN `Clients` cli ON tt.`ClientId` = cli.`ClientId`
      WHERE
	      tt.`UserId` = @UserId AND
	      tt.`Deleted` = 0 AND
	      tt.`EndReason` != 1
      ORDER BY `EndReason`, `StartTimeUtc` ASC";
    }




    public string GetCurrentEntry()
    {
      return @"SELECT *
      FROM `RawTimers`
      WHERE
	      `ParentEntryId` = @ParentEntryId AND
	      `RootEntryId` = @RootEntryId AND
	      `ClientId` = @ClientId AND
	      `ProductId` = @ProductId AND
	      `ProjectId` = @ProjectId AND
	      `UserId` = @UserId AND
	      `Deleted` = 0 AND
	      `EntryState` = @EntryState
      LIMIT 1";
    }

    public string PauseTimer()
    {
      return @"UPDATE `RawTimers`
      SET
	      `Running` = 1,
	      `EntryState` = @EntryState,
        `Completed` = 0,
	      `EndTimeUtc` = CURRENT_TIMESTAMP(),
	      `TotalSeconds` = TIME_TO_SEC(TIMEDIFF(CURRENT_TIMESTAMP(), `StartTimeUtc`)),
        `Notes` = @Notes
      WHERE
	      `EntryId` = @EntryId";
    }

    public string GetByRawTimerId()
    {
      return @"SELECT *
      FROM `RawTimers`
      WHERE
	      `EntryId` = @EntryId";
    }

    public string SpawnResumedTimer()
    {
      return @"INSERT INTO `RawTimers`
	      (
          `ParentEntryId`, `RootEntryId`, `ClientId`, `ProductId`, `ProjectId`,
          `UserId`, `Running`, `EntryState`, `Completed`, `Notes`
        )
      VALUES
	      (
          @ParentEntryId, @RootEntryId, @ClientId, @ProductId, @ProjectId,
          @UserId, @Running, @EntryState, @Completed, @Notes
        )";
    }

    public string FlagAsResumed()
    {
      return @"UPDATE `RawTimers`
      SET
	      `Running` = 0,
	      `Completed` = 0
      WHERE
	      `EntryId` = @EntryId";
    }

    public string SetRootTimerId()
    {
      return @"UPDATE `RawTimers`
      SET
	      `RootEntryId` = @RootEntryId
      WHERE
	      `EntryId` = @EntryId";
    }

    public string StopTimer()
    {
      return @"UPDATE `RawTimers`
      SET
         `Running` = 0,
         `EntryState` = 3,
         `Completed` = 0,
         `EndTimeUtc` = CURRENT_TIMESTAMP(),
         `TotalSeconds` = TIME_TO_SEC(TIMEDIFF(CURRENT_TIMESTAMP(), `StartTimeUtc`))
      WHERE
         `EntryId` = @EntryId";
    }

    public string CompleteTimerSet()
    {
      return @"UPDATE `RawTimers`
      SET
	      `Running` = 0,
	      `Completed` = 1
      WHERE
	      `RootEntryId` = @RootEntryId";
    }

    public string GetTimerSeries()
    {
      return @"SELECT
	      raw.*,
	      prod.`ProductName`,
	      proj.`ProjectName`,
	      cli.`ClientName`
      FROM `RawTimers` raw
	      INNER JOIN `Products` prod ON prod.`ProductId` = raw.`ProductId`
	      INNER JOIN `Projects` proj ON proj.`ProjectId` = raw.`ProjectId`
	      INNER JOIN `Clients` cli ON cli.`ClientId` = raw.`ClientId`
      WHERE
	      `RootEntryId` = @RootEntryId
      ORDER BY `EntryId` DESC";
    }

    public string GetUsersWithRunningTimers()
    {
      return @"SELECT
 	      DISTINCT(u.`UserId`) AS 'Key',
	      u.`Username` AS 'Value'
      FROM `RawTimers` t
	      INNER JOIN `Users` u ON u.`UserId` = t.`UserId`
      WHERE
	      `Completed` = 0 AND
	      `Running` = 1 AND
	      `EntryState` = 1";
    }

    public string GetLongRunningTimers()
    {
      return @"SELECT *
      FROM `RawTimers`
      WHERE
	      `UserId` = @UserId AND
	      `Deleted` = 0 AND
	      `Completed` = 0 AND
	      `Running` = 1 AND
	      `EntryState` = 1 AND
	      `StartTimeUtc` <= DATE_ADD(CURRENT_TIMESTAMP(), INTERVAL -@ThresholdSec SECOND)";
    }

    public string UpdateNotes()
    {
      return @"UPDATE `RawTimers`
      SET
	      `Notes` = @Notes
      WHERE
	      `EntryId` = @EntryId";
    }

    public string UpdateTimerDuration()
    {
      return @"UPDATE `RawTimers`
      SET
	      `StartTimeUtc` = @StartTimeUtc,
	      `TotalSeconds` = @TotalSeconds,
	      `EndTimeUtc` = DATE_ADD(@StartTimeUtc, INTERVAL @TotalSeconds SECOND),
        `Notes` = @Notes
      WHERE
	      `EntryId` = @EntryId";
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
         rtt.`Running` = 1 AND
         rtt.`EntryState` = 1
      ORDER BY `EntryState`, `RootEntryId`, `StartTimeUtc` ASC";
    }
  }
}
