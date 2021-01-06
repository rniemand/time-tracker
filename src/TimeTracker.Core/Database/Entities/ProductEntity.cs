using System;

namespace TimeTracker.Core.Database.Entities
{
  public class ProductEntity
  {
    public int ProductId { get; set; }
    public int ClientId { get; set; }
    public int UserId { get; set; }
    public bool Deleted { get; set; }
    public DateTime DateCreatedUtc { get; set; }
    public DateTime? DateModifiedUtc { get; set; }
    public string ProductName { get; set; }

    public ProductEntity()
    {
      // TODO: [TESTS] (ProductEntity) Add tests
      ProductId = 0;
      ClientId = 0;
      UserId = 0;
      Deleted = false;
      DateCreatedUtc = DateTime.UtcNow;
      DateModifiedUtc = null;
      ProductName = string.Empty;
    }
  }
}
