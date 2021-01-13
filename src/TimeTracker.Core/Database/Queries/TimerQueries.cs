namespace TimeTracker.Core.Database.Queries
{
  public interface ITimerQueries
  {
    string GetRunningTimer();
    string AddTimer();
    string CompleteTimer();
    string GetActiveTimers();
    string PauseTimer();
    string GetTimerById();
    string StopTimer();
    string GetProjectTimers();
    string UpdateTimerDuration();
    string GetRunningTimers();
    string GetUsersWithRunningTimers();
    string GetLongRunningTimers();
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

    public string CompleteTimer()
    {
      return @"UPDATE `Timers`
      SET
         `Running` = 0,
         `EntryState` = @EntryState,
         `EndTimeUtc` = CURRENT_TIMESTAMP(),
         `TotalSeconds` = TIME_TO_SEC(TIMEDIFF(CURRENT_TIMESTAMP(), `StartTimeUtc`)),
         `Notes` = @Notes
      WHERE
         `EntryId` = @EntryId";
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
      return @"UPDATE `Timers`
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

    public string StopTimer()
    {
      return @"UPDATE `Timers`
      SET
	      `Running` = 0,
	      `EntryState` = @EntryState,
	      `EndTimeUtc` = CURRENT_TIMESTAMP(),
	      `TotalSeconds` = TIME_TO_SEC(TIMEDIFF(CURRENT_TIMESTAMP(), `StartTimeUtc`)),
	      `Notes` = @Notes
      WHERE
	      `EntryId` = @EntryId";
    }

    public string GetProjectTimers()
    {
      return @"SELECT
	      t.*,
	      prod.`ProductName`,
	      proj.`ProjectName`,
	      cli.`ClientName`
      FROM `Timers` t
	      INNER JOIN `Products` prod ON prod.`ProductId` = t.`ProductId`
	      INNER JOIN `Projects` proj ON proj.`ProjectId` = t.`ProjectId`
	      INNER JOIN `Clients` cli ON cli.`ClientId` = t.`ClientId`
      WHERE
	      t.`ProjectId` = @ProjectId AND
	      t.`Deleted` = 0
      ORDER BY t.`EntryId` DESC";
    }

    public string UpdateTimerDuration()
    {
      return @"UPDATE `Timers`
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
	      t.`Running` = 1
      ORDER BY t.`EntryId` ASC";
    }

    public string GetUsersWithRunningTimers()
    {
      return @"SELECT
	      t.`UserId` AS 'Key',
	      u.`Username` AS 'Value'
      FROM `Timers` t
	      INNER JOIN `Users` u ON u.`UserId` = t.`UserId`
      WHERE
	      t.`Deleted` = 0 AND
	      t.`Running` = 1";
    }

    public string GetLongRunningTimers()
    {
      return @"SELECT *
      FROM `Timers`
      WHERE
	      `UserId` = @UserId AND
	      `Deleted` = 0 AND
	      `Running` = 1 AND
	      `StartTimeUtc` <= DATE_ADD(CURRENT_TIMESTAMP(), INTERVAL -@ThresholdSec SECOND)";
    }
  }
}
