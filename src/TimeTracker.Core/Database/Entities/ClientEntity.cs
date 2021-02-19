using System;

namespace TimeTracker.Core.Database.Entities
{
  public class ClientEntity
  {
    public int ClientId { get; set; }
    public bool Deleted { get; set; }
    public int UserId { get; set; }
    public DateTime DateAddedUtc { get; set; }
    public DateTime? DateUpdatedUtc { get; set; }
    public DateTime? DateDeletedUtc { get; set; }
    public string ClientName { get; set; }
    public string ClientEmail { get; set; }

    public ClientEntity()
    {
      // TODO: [TESTS] (ClientEntity) Add tests
      ClientId = 0;
      Deleted = false;
      UserId = 0;
      DateAddedUtc = DateTime.UtcNow;
      DateUpdatedUtc = null;
      DateDeletedUtc = null;
      ClientName = string.Empty;
      ClientEmail = string.Empty;
    }
  }
}
