namespace TimeTracker.Core.Database.Queries
{
  public interface ITrackedTimeQueries
  {
    string StartNew();
    string GetRunningExisting();
    string GetActive();
    string Pause();
    string GetByEntryId();
    string Stop();
    string GetProjectEntries();
    string UpdateDuration();
    string GetRunning();





    string GetCurrentEntry();
    string SpawnResumedTimer();
    string FlagAsResumed();
    string SetRootTimerId();
    string CompleteTimerSet();
    string GetUsersWithRunningTimers();
    string GetLongRunningTimers();
    string UpdateNotes();
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

    public string GetRunningExisting()
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

    public string Pause()
    {
      return @"UPDATE `TrackedTime`
      SET
	      `Running` = 0,
	      `EndReason` = @EndReason,
	      `EndTimeUtc` = CURRENT_TIMESTAMP(),
	      `TotalSeconds` = TIME_TO_SEC(TIMEDIFF(CURRENT_TIMESTAMP(), `StartTimeUtc`)),
	      `Notes` = @Notes
      WHERE
	      `EntryId` = @EntryId";
    }

    public string GetByEntryId()
    {
      return @"SELECT *
      FROM `TrackedTime`
      WHERE `EntryId` = @EntryId";
    }

    public string Stop()
    {
      return @"UPDATE `TrackedTime`
      SET
	      `Running` = 0,
	      `EndReason` = @EndReason,
	      `EndTimeUtc` = CURRENT_TIMESTAMP(),
	      `TotalSeconds` = TIME_TO_SEC(TIMEDIFF(CURRENT_TIMESTAMP(), `StartTimeUtc`))
      WHERE
	      `EntryId` = @EntryId";
    }

    public string GetProjectEntries()
    {
      return @"SELECT
	      tt.*,
	      prod.`ProductName`,
	      proj.`ProjectName`,
	      cli.`ClientName`
      FROM `TrackedTime` tt
	      INNER JOIN `Products` prod ON prod.`ProductId` = tt.`ProductId`
	      INNER JOIN `Projects` proj ON proj.`ProjectId` = tt.`ProjectId`
	      INNER JOIN `Clients` cli ON cli.`ClientId` = tt.`ClientId`
      WHERE
	      tt.`ProjectId` = @ProjectId AND
	      tt.`Deleted` = 0
      ORDER BY `EntryId` DESC";
    }

    public string UpdateDuration()
    {
      return @"UPDATE `TrackedTime`
      SET
	      `StartTimeUtc` = @StartTimeUtc,
	      `TotalSeconds` = @TotalSeconds,
	      `EndTimeUtc` = DATE_ADD(@StartTimeUtc, INTERVAL @TotalSeconds SECOND),
	      `Notes` = @Notes
      WHERE
	      `EntryId` = @EntryId";
    }

    public string GetRunning()
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
	      tt.`Running` = 1
      ORDER BY `EntryId` ASC";
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

    public string CompleteTimerSet()
    {
      return @"UPDATE `RawTimers`
      SET
	      `Running` = 0,
	      `Completed` = 1
      WHERE
	      `RootEntryId` = @RootEntryId";
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
  }
}
