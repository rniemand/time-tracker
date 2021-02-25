using System;

namespace TimeTracker.Core.Database.Entities
{
  public class ProjectEntity
  {
    public int ProjectId { get; set; }
    public int ClientId { get; set; }
    public int ProductId { get; set; }
    public int UserId { get; set; }
    public bool Deleted { get; set; }
    public DateTime DateAddedUtc { get; set; }
    public DateTime? DateUpdatedUtc { get; set; }
    public DateTime? DateDeletedUtc { get; set; }
    public string ProjectName { get; set; }

    public ProjectEntity()
    {
      // TODO: [TESTS] (ProjectEntity) Add tests
      ProjectId = 0;
      ClientId = 0;
      ProductId = 0;
      UserId = 0;
      Deleted = false;
      DateAddedUtc = DateTime.UtcNow;
      DateUpdatedUtc = null;
      DateDeletedUtc = null;
      ProjectName = string.Empty;
    }
  }
}
