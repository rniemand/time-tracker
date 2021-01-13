namespace TimeTracker.Core.Database.Queries
{
  public interface IOptionQueries
  {
    string GetRawOption();
    string GetRawOptionsForCategory();
  }

  public class OptionQueries : IOptionQueries
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
