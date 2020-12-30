namespace TimeTracker.Core.Database.Queries
{
  public interface IClientRepoQueries
  {
    string GetAll();
  }

  public class ClientRepoQueries : IClientRepoQueries
  {
    public string GetAll()
    {
      return @"SELECT *
      FROM `Clients`
      WHERE
	      `Deleted` = 0 AND
	      `UserId` = @UserId
      ORDER BY `ClientName`";
    }
  }
}
