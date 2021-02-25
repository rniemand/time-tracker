namespace TimeTracker.Core.Database.Queries
{
  public interface IProductQueries
  {
    string GetAll();
    string Add();
    string GetById();
    string Update();
    string GetByName();
  }

  public class ProductQueries : IProductQueries
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
	      `DateUpdatedUtc` = utc_timestamp(4),
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
