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
  }
}
