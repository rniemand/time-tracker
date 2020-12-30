namespace TimeTracker.Core.Database.Queries
{
  public interface IProductRepoQueries
  {
    string GetAll();
  }

  public class ProductRepoQueries : IProductRepoQueries
  {
    public string GetAll()
    {
      return @"SELECT *
      FROM `Products`
      WHERE
	      `Deleted` = 0 AND
	      `UserId` = @UserId AND
	      `ClientId` = @ClientId
      ORDER BY `ProductName` ASC";
    }
  }
}
