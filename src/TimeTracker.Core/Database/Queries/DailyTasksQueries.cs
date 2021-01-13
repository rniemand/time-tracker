namespace TimeTracker.Core.Database.Queries
{
  public interface IDailyTasksQueries
  {
    string AddTask();
    string SearchByName();
    string ListClientTasks();
  }

  public class DailyTasksQueries : IDailyTasksQueries
  {
    public string AddTask()
    {
      return @"INSERT INTO `DailyTasks`
	      (`UserId`, `ClientId`, `TaskName`)
      VALUES
	      (@UserId, @ClientId, @TaskName)";
    }

    public string SearchByName()
    {
      return @"SELECT *
      FROM `DailyTasks`
      WHERE
	      `UserId` = @UserId AND
	      `ClientId` = @ClientId AND
	      `Deleted` = 0 AND
	      `TaskName` = @TaskName";
    }

    public string ListClientTasks()
    {
      return @"SELECT *
      FROM `DailyTasks`
      WHERE
	      `Deleted` = 0 AND
	      `ClientId` = @ClientId
      ORDER BY `TaskName` ASC";
    }
  }
}
