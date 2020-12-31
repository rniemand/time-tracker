namespace TimeTracker.Core.Database.Queries
{
  public interface IProductRepoQueries
  {
    string GetAll();
    string Add();
    string GetById();
    string Update();
    string GetByName();
  }

  public class ProductRepoQueries : IProductRepoQueries
  {
    public string GetAll()
    {
      return @"SELECT *
      FROM `Products`
      WHERE
	      `Deleted` = 0 AND
	      `ClientId` = @ClientId
      ORDER BY `ProductName` ASC";
    }

    public string Add()
    {
      return @"INSERT INTO `Products`
	      (`ClientId`, `UserId`, `ProductName`)
      VALUES
	      (@ClientId, @UserId, @ProductName)";
    }

    public string GetById()
    {
      return @"SELECT *
      FROM `Products`
      WHERE
	      `ProductId` = @ProductId";
    }

    public string Update()
    {
      return @"UPDATE `Products`
      SET
	      `DateModifiedUtc` = CURRENT_TIMESTAMP(),
	      `ProductName` = @ProductName
      WHERE
	      `ProductId` = @ProductId";
    }

    public string GetByName()
    {
      return @"SELECT *
      FROM `Products`
      WHERE
	      `Deleted` = 0 AND
	      `ClientId` = @ClientId AND
	      `ProductName` = @ProductName
      LIMIT 1";
    }
  }
}
