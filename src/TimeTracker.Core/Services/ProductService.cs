using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TimeTracker.Core.Database.Repos;
using TimeTracker.Core.Models.Dto;

namespace TimeTracker.Core.Services
{
  public interface IProductService
  {
    Task<List<ProductDto>> GetAll(int userId, int clientId);
    Task<ProductDto> AddProduct(int userId, ProductDto productDto);
    Task<ProductDto> UpdateProduct(int userId, ProductDto productDto);
    Task<ProductDto> GetById(int userId, int productId);
  }

  public class ProductService : IProductService
  {
    private readonly IProductRepo _productRepo;

    public ProductService(IProductRepo productRepo)
    {
      _productRepo = productRepo;
    }

    public async Task<List<ProductDto>> GetAll(int userId, int clientId)
    {
      // TODO: [TESTS] (ProductService.GetAll) Add tests

      var dbEntries = await _productRepo.GetAll(userId, clientId);

      return dbEntries
        .AsQueryable()
        .Select(ProductDto.Projection)
        .ToList();
    }

    public async Task<ProductDto> AddProduct(int userId, ProductDto productDto)
    {
      // TODO: [TESTS] (ProductService.AddProduct) Add tests
      // TODO: [VALIDATION] (ProductService.AddProduct) Ensure user owns this product

      var productEntity = productDto.AsProductEntity();
      productEntity.UserId = userId;

      if (await _productRepo.Add(productEntity) < 1)
      {
        // TODO: [HANDLE] (ProductService.AddProduct) Handle this better
        return null;
      }

      var dbEntry = await _productRepo.GetByName(productDto.ClientId, productEntity.ProductName);
      return ProductDto.FromEntity(dbEntry);
    }

    public async Task<ProductDto> UpdateProduct(int userId, ProductDto productDto)
    {
      // TODO: [TESTS] (ProductService.UpdateProduct) Add tests
      // TODO: [VALIDATION] (ProductService.UpdateProduct) Ensure that user owns this product

      if (productDto.UserId != userId)
      {
        // TODO: [HANDLE] (ProductService.UpdateProduct) Handle this better
        return null;
      }

      await _productRepo.Update(productDto.AsProductEntity());
      var dbEntry = await _productRepo.GetById(productDto.ProductId);
      return ProductDto.FromEntity(dbEntry);
    }

    public async Task<ProductDto> GetById(int userId, int productId)
    {
      // TODO: [TESTS] (ProductService.GetById) Add tests

      var dbEntry = await _productRepo.GetById(productId);
      if (dbEntry == null)
      {
        // TODO: [HANDLE] (ProductService.GetById) Handle this better
        return null;
      }

      if (dbEntry.UserId != userId)
      {
        // TODO: [HANDLE] (ProductService.GetById) Handle this better
        return null;
      }

      return ProductDto.FromEntity(dbEntry);
    }
  }
}
