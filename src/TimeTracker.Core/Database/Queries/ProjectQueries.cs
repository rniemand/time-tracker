namespace TimeTracker.Core.Database.Queries
{
  public interface IProjectQueries
  {
    string GetAllForProduct();
    string GetById();
    string GetByName();
    string Add();
    string Update();
  }

  public class ProjectQueries : IProjectQueries
  {
    public string GetAllForProduct()
    {
      return @"SELECT *
      FROM `Projects`
      WHERE
	      `Deleted` = 0 AND
	      `ProductId` = @ProductId
      ORDER BY `ProjectName` ASC";
    }

    public string GetById()
    {
      return @"SELECT *
      FROM `Projects`
      WHERE
	      `ProjectId` = @ProjectId";
    }

    public string GetByName()
    {
      return @"SELECT *
      FROM `Projects`
      WHERE
	      `Deleted` = 0 AND
	      `ProductId` = @ProductId AND
	      `ProjectName` = @ProjectName";
    }

    public string Add()
    {
      return @"INSERT INTO `Projects`
	      (`ClientId`, `ProductId`, `UserId`, `ProjectName`)
      VALUES
	      (@ClientId, @ProductId, @UserId, @ProjectName)";
    }

    public string Update()
    {
      return @"UPDATE `Projects`
      SET
	      `DateModifiedUtc` = CURRENT_TIMESTAMP(),
	      `ProjectName` = @ProjectName
      WHERE
	      `ProjectId` = @ProjectId";
    }
  }
}
