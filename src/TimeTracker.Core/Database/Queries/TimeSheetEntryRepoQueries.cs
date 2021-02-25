namespace TimeTracker.Core.Database.Queries
{
  public interface ITimeSheetEntryRepoQueries
  {
    string GetReferencedProjects();
    string GetReferencedProducts();
    string GetProjectTimeSheetEntry();
    string AddEntry();
    string UpdateEntry();
    string GetEntries();
  }

  public class TimeSheetEntryRepoQueries : ITimeSheetEntryRepoQueries
  {
    public string GetReferencedProjects()
    {
      return @"SELECT *
      FROM `Projects`
      WHERE
	      `Deleted` = 0 AND
	      `ProjectId` IN (
		      SELECT DISTINCT(`ProjectId`)
		      FROM `TimeSheetEntries`
		      WHERE
			      `Deleted` = 0 AND
			      `ClientId` = @ClientId AND
			      `EntryDate` >= @FromDate AND
			      `EntryDate` <= @ToDate
	      )";
    }

    public string GetReferencedProducts()
    {
      return @"SELECT *
      FROM `Products`
      WHERE
	      `Deleted` = 0 AND
	      `ProductId` IN (
		      SELECT DISTINCT(`ProductId`)
		      FROM `Projects`
		      WHERE
			      `Deleted` = 0 AND
			      `ProjectId` IN (
				      SELECT DISTINCT(`ProjectId`)
				      FROM `TimeSheetEntries`
				      WHERE
					      `Deleted` = 0 AND
					      `ClientId` = @ClientId AND
					      `EntryDate` >= @FromDate AND
					      `EntryDate` <= @ToDate
			      )	
	      )";
    }

    public string GetProjectTimeSheetEntry()
    {
      return @"SELECT *
      FROM `TimeSheetEntries`
      WHERE
	      `Deleted` = 0 AND
	      `ProjectId` = @ProjectId AND
	      `EntryDate` = @EntryDate";
    }

    public string AddEntry()
    {
      return @"INSERT INTO `TimeSheetEntries`
	      (`UserId`, `ClientId`, `ProductId`, `ProjectId`, `EntryDate`, `EntryVersion`, `EntryTimeMin`)
      VALUES
	      (@UserId, @ClientId, @ProductId, @ProjectId, @EntryDate, @EntryVersion, @EntryTimeMin)";
    }

    public string UpdateEntry()
    {
      return @"UPDATE `TimeSheetEntries`
      SET
	      `DateUpdatedUtc` = utc_timestamp(4),
	      `EntryVersion` = @EntryVersion,
	      `EntryTimeMin` = @EntryTimeMin
      WHERE
	      `EntryId` = @EntryId";
    }

    public string GetEntries()
    {
      return @"SELECT *
      FROM `TimeSheetEntries`
      WHERE
	      `Deleted` = 0 AND
	      `ClientId` = @ClientId AND
	      `EntryDate` >= @FromDate AND
	      `EntryDate` <= @ToDate
      ORDER BY `ClientId`, `ProductId`, `ProjectId`, `EntryDate`";
    }
  }
}
