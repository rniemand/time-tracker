namespace TimeTracker.Core.Database.Queries
{
  public interface IUserQueries
  {
    string GetUsingCredentials();
    string UpdateLastLoginDate();
    string GetUserById();
  }

  public class UserQueries : IUserQueries
  {
    public string GetUsingCredentials()
    {
      return @"SELECT *
      FROM `Users`
      WHERE
	      `Username` = @Username AND
	      `Password` = @Password AND
	      `Deleted` = 0";
    }

    public string UpdateLastLoginDate()
    {
      return @"UPDATE `Users`
      SET
	      `LastLoginDateUtc` = utc_timestamp(4)
      WHERE
	      `UserId` = @UserId";
    }

    public string GetUserById()
    {
      return @"SELECT *
      FROM `Users`
      WHERE `UserId` = @UserId";
    }
  }
}
