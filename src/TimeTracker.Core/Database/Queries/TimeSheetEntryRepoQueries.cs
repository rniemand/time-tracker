namespace TimeTracker.Core.Database.Queries
{
  public interface ITimeSheetEntryRepoQueries
  {
    string GetReferencedProjects();
    string GetReferencedProducts();
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
  }
}
