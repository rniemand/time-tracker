using System;
using System.Linq.Expressions;
using TimeTracker.Core.Database.Entities;

namespace TimeTracker.Core.Models.Dto
{
  public class ProductDto
  {
    public int ProductId { get; set; }
    public int ClientId { get; set; }
    public int UserId { get; set; }
    public bool Deleted { get; set; }
    public DateTime DateCreatedUtc { get; set; }
    public DateTime? DateModifiedUtc { get; set; }
    public string ProductName { get; set; }

    public static Expression<Func<ProductEntity, ProductDto>> Projection
    {
      get
      {
        return entity => new ProductDto
        {
          UserId = entity.UserId,
          DateCreatedUtc = entity.DateCreatedUtc,
          ClientId = entity.ClientId,
          DateModifiedUtc = entity.DateModifiedUtc,
          Deleted = entity.Deleted,
          ProductName = entity.ProductName,
          ProductId = entity.ProductId
        };
      }
    }

    public static ProductDto FromEntity(ProductEntity entity)
    {
      // TODO: [TESTS] (ProductDto.FromEntity) Add tests
      return entity == null ? null : Projection.Compile()(entity);
    }

    public ProductDto()
    {
      // TODO: [TESTS] (ProductDto) Add tests
      ProductId = 0;
      ClientId = 0;
      UserId = 0;
      Deleted = false;
      DateCreatedUtc = DateTime.UtcNow;
      DateModifiedUtc = null;
      ProductName = string.Empty;
    }

    public ProductEntity AsProductEntity()
    {
      // TODO: [TESTS] (ProductDto.AsProductEntity) Add tests
      return new ProductEntity
      {
        UserId = UserId,
        DateCreatedUtc = DateCreatedUtc,
        ClientId = ClientId,
        DateModifiedUtc = DateModifiedUtc,
        Deleted = Deleted,
        ProductName = ProductName,
        ProductId = ProductId
      };
    }
  }
}
