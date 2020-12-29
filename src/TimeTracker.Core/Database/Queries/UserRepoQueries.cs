namespace TimeTracker.Core.Database.Queries
{
  public interface IUserRepoQueries
  {
    string GetUsingCredentials();
    string UpdateLastLoginDate();
    string GetUserById();
  }

  public class UserRepoQueries : IUserRepoQueries
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
	      `LastLoginDateUtc` = current_timestamp()
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
