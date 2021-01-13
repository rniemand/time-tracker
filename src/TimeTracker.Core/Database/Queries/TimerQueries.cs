namespace TimeTracker.Core.Database.Queries
{
  public interface ITimerQueries
  {
    string StartNew();
    string GetRunningExisting();
    string GetActive();
    string Pause();
    string Complete();
    string GetByEntryId();
    string Stop();
    string GetProjectEntries();
    string UpdateDuration();
    string GetRunning();
  }

  public class TimerQueries : ITimerQueries
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
	      tt.`EntryState` != 1
      ORDER BY `EntryState`, `StartTimeUtc` ASC";
    }

    public string Pause()
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
