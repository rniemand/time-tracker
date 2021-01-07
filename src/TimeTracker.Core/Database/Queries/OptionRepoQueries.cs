namespace TimeTracker.Core.Database.Queries
{
  public interface IOptionRepoQueries
  {
    string GetRawOption();
    string GetRawOptionsForCategory();
  }

  public class OptionRepoQueries : IOptionRepoQueries
  {
    public string GetRawOption()
    {
      return @"SELECT *
      FROM `Options`
      WHERE
	      `Deleted` = 0 AND
	      `OptionCategory` = @OptionCategory AND
	      `OptionKey` = @OptionKey AND
	      `UserId` IN (0, @UserId)
      ORDER BY `UserId` DESC
      LIMIT 1";
    }

    public string GetRawOptionsForCategory()
    {
      return @"SELECT *
      FROM `Options`
      WHERE
	      `Deleted` = 0 AND
	      `OptionCategory` = @OptionCategory AND
	      `UserId` IN (0, @UserId)
      ORDER BY `UserId` ASC";
    }
  }
}
