namespace TimeTracker.Core.Database.Queries
{
  public interface IUserRepoQueries
  {
    string GetUsingCredentials();
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
  }
}
