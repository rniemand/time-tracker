namespace TimeTracker.Core.Database.Queries
{
  public interface IUserRepoQueries
  {
    string GetUsingCredentials();
    string UpdateLastLoginDate();
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
  }
}
