﻿namespace TimeTracker.Core.Database.Queries
{
  public interface IClientRepoQueries
  {
    string GetAll();
    string GetByName();
    string Add();
    string Update();
    string GetById();
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

    public string GetByName()
    {
      return @"SELECT *
      FROM `Clients`
      WHERE
	      `UserId` = @UserId AND
	      `ClientName` = @ClientName AND
	      `Deleted` = 0";
    }

    public string Add()
    {
      return @"INSERT INTO `Clients`
	      (`UserId`, `ClientName`)
      VALUES
	      (@UserId, @ClientName)";
    }

    public string Update()
    {
      return @"UPDATE `Clients`
      SET
	      `DateModifiedUtc` = CURRENT_TIMESTAMP(),
	      `ClientName` = @ClientName,
	      `ClientEmail` = @ClientEmail
      WHERE
	      `ClientId` = @ClientId";
    }

    public string GetById()
    {
      return @"SELECT *
      FROM `Clients`
      WHERE
	      `ClientId` = @ClientId";
    }
  }
}
