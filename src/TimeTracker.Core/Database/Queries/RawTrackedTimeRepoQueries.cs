namespace TimeTracker.Core.Database.Queries
{
  public interface IRawTrackedTimeRepoQueries
  {
    string StartNew();
  }

  public class RawRawTrackedTimeRepoQueries : IRawTrackedTimeRepoQueries
  {
    public string StartNew()
    {
      return @"INSERT INTO `RawTrackedTime`
	      (`ClientId`, `ProductId`, `ProjectId`, `UserId`, `EntryState`)
      VALUES
	      (@ClientId, @ProductId, @ProjectId, @UserId, @EntryState)";
    }
  }
}
