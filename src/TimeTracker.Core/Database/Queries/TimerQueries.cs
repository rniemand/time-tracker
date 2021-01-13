namespace TimeTracker.Core.Database.Queries
{
  public interface ITimerQueries
  {
    string GetRunningTimer();
    string AddTimer();
    string StartNew(); // review
    string GetRunningExisting(); // review
    string GetActiveTimers();
    string PauseTimer();
    string Complete(); // review
    string GetTimerById();
    string Stop(); // review
    string GetProjectEntries(); // review
    string UpdateDuration(); // review
    string GetRunning(); // review
  }

  public class TimerQueries : ITimerQueries
  {
    public string GetRunningTimer()
    {
      return @"SELECT *
      FROM `Timers`
      WHERE
	      `ClientId` = @ClientId AND
	      `ProductId` = @ProductId AND
	      `ProjectId` = @ProjectId AND
	      `UserId` = @UserId AND
	      `Deleted` = 0 AND
	      `Running` = 1 AND
	      `EntryType` = @EntryType";
    }

    public string AddTimer()
    {
      return @"INSERT INTO `Timers`
	      (`ClientId`, `ProductId`, `ProjectId`, `UserId`, `EntryType`, `EntryState`)
      VALUES
	      (@ClientId, @ProductId, @ProjectId, @UserId, @EntryType, @EntryState)";
    }

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

    public string GetActiveTimers()
    {
      return @"SELECT
         t.*,
         prod.`ProductName`,
         proj.`ProjectName`,
         cli.`ClientName`
      FROM `Timers` t
         INNER JOIN `Products` prod ON t.`ProductId` = prod.`ProductId`
         INNER JOIN `Projects` proj ON t.`ProjectId` = proj.`ProjectId`
         INNER JOIN `Clients` cli ON t.`ClientId` = cli.`ClientId`
      WHERE
         t.`UserId` = @UserId AND
         t.`Deleted` = 0 AND
         t.`EntryState` != 1
      ORDER BY `EntryState`, `StartTimeUtc` ASC";
    }

    public string PauseTimer()
    {
      return @"UPDATE `TrackedTime`
      SET
         `Running` = 0,
         `EntryState` = @EntryState,
         `EndTimeUtc` = CURRENT_TIMESTAMP(),
         `TotalSeconds` = TIME_TO_SEC(TIMEDIFF(CURRENT_TIMESTAMP(), `StartTimeUtc`)),
         `Notes` = @Notes
      WHERE
         `EntryId` = @EntryId";
    }

    public string Complete()
    {
      return @"UPDATE `TrackedTime`
      SET
	      `Running` = 0,
	      `EntryState` = @EntryState,
	      `EndTimeUtc` = CURRENT_TIMESTAMP(),
	      `TotalSeconds` = TIME_TO_SEC(TIMEDIFF(CURRENT_TIMESTAMP(), `StartTimeUtc`)),
	      `Notes` = @Notes
      WHERE
	      `EntryId` = @EntryId";
    }

    public string GetTimerById()
    {
      return @"SELECT *
      FROM `Timers`
      WHERE `EntryId` = @EntryId";
    }

    public string Stop()
    {
      return @"UPDATE `TrackedTime`
      SET
	      `Running` = 0,
	      `EntryState` = @EntryState,
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
  }
}
