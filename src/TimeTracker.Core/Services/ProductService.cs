using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Rn.NetCore.Common.Logging;
using Rn.NetCore.Common.Metrics;
using Rn.NetCore.Common.Metrics.Builders;
using TimeTracker.Core.Database.Entities;
using TimeTracker.Core.Database.Repos;
using TimeTracker.Core.Enums;
using TimeTracker.Core.Models.Dto;

namespace TimeTracker.Core.Services
{
  public interface IProductService
  {
    Task<List<ProductDto>> GetAll(int userId, int clientId);
    Task<bool> AddProduct(int userId, ProductDto productDto);
    Task<bool> UpdateProduct(int userId, ProductDto productDto);
    Task<ProductDto> GetById(int userId, int productId);
    Task<List<IntListItem>> GetClientProductsListItems(int userId, int clientId);
  }

  public class ProductService : IProductService
  {
    private readonly ILoggerAdapter<ProductService> _logger;
    private readonly IMetricService _metrics;
    private readonly IProductRepo _productRepo;

    public ProductService(
      ILoggerAdapter<ProductService> logger,
      IMetricService metrics,
      IProductRepo productRepo)
    {
      _productRepo = productRepo;
      _logger = logger;
      _metrics = metrics;
    }

    public async Task<List<ProductDto>> GetAll(int userId, int clientId)
    {
      // TODO: [TESTS] (ProductService.GetAll) Add tests
      var builder = new ServiceMetricBuilder(nameof(ProductService), nameof(GetAll))
        .WithCategory(MetricCategory.Product, MetricSubCategory.GetAll)
        .WithCustomInt1(userId)
        .WithCustomInt2(clientId);

      try
      {
        using (builder.WithTiming())
        {
          List<ProductEntity> dbEntries;
          using (builder.WithCustomTiming1())
          {
            builder.IncrementQueryCount();
            dbEntries = await _productRepo.GetAll(clientId);
            builder.WithResultCount(dbEntries?.Count ?? 0);
          }

          if (dbEntries == null || dbEntries.Count == 0)
          {
            // TODO: [HANDLE] (ProductService.GetAll) Handle this
            return new List<ProductDto>();
          }

          if (dbEntries.First().UserId != userId)
          {
            // TODO: [HANDLE] (ProductService.GetAll) Handle this
            return new List<ProductDto>();
          }

          return dbEntries
            .AsQueryable()
            .Select(ProductDto.Projection)
            .ToList();
        }
      }
      catch (Exception ex)
      {
        _logger.LogUnexpectedException(ex);
        builder.WithException(ex);
        return new List<ProductDto>();
      }
      finally
      {
        await _metrics.SubmitPointAsync(builder);
      }
    }

    public async Task<bool> AddProduct(int userId, ProductDto productDto)
    {
      // TODO: [TESTS] (ProductService.AddProduct) Add tests
      // TODO: [VALIDATION] (ProductService.AddProduct) Ensure user owns this product
      var builder = new ServiceMetricBuilder(nameof(ProductService), nameof(AddProduct))
        .WithCategory(MetricCategory.Product, MetricSubCategory.Add)
        .WithCustomInt1(userId)
        .WithCustomInt2(productDto.ClientId);

      try
      {
        using (builder.WithTiming())
        {
          var productEntity = productDto.AsProductEntity();
          productEntity.UserId = userId;

          using (builder.WithCustomTiming1())
          {
            builder.IncrementQueryCount();
            if (await _productRepo.Add(productEntity) <= 0)
              return false;

            builder.CountResult(1);
            return true;
          }
        }
      }
      catch (Exception ex)
      {
        _logger.LogUnexpectedException(ex);
        builder.WithException(ex);
        return false;
      }
      finally
      {
        await _metrics.SubmitPointAsync(builder);
      }
    }

    public async Task<bool> UpdateProduct(int userId, ProductDto productDto)
    {
      // TODO: [TESTS] (ProductService.UpdateProduct) Add tests
      // TODO: [VALIDATION] (ProductService.UpdateProduct) Ensure that user owns this product
      var builder = new ServiceMetricBuilder(nameof(ProductService), nameof(UpdateProduct))
        .WithCategory(MetricCategory.Product, MetricSubCategory.Update)
        .WithCustomInt1(userId)
        .WithCustomInt2(productDto.ClientId);

      try
      {
        using (builder.WithTiming())
        {
          if (productDto.UserId != userId)
          {
            // TODO: [HANDLE] (ProductService.UpdateProduct) Handle this better
            return false;
          }

          using (builder.WithCustomTiming1())
          {
            builder.IncrementQueryCount();
            if (await _productRepo.Update(productDto.AsProductEntity()) <= 0)
              return false;

            builder.WithResultCount(1);
            return true;
          }
        }
      }
      catch (Exception ex)
      {
        _logger.LogUnexpectedException(ex);
        builder.WithException(ex);
        return false;
      }
      finally
      {
        await _metrics.SubmitPointAsync(builder);
      }
    }

    public async Task<ProductDto> GetById(int userId, int productId)
    {
      // TODO: [TESTS] (ProductService.GetById) Add tests
      var builder = new ServiceMetricBuilder(nameof(ProductService), nameof(GetById))
        .WithCategory(MetricCategory.Product, MetricSubCategory.GetById)
        .WithCustomInt1(userId)
        .WithCustomInt2(productId);

      try
      {
        using (builder.WithTiming())
        {
          ProductEntity dbEntry;
          using (builder.WithCustomTiming1())
          {
            builder.IncrementQueryCount();
            dbEntry = await _productRepo.GetById(productId);
            builder.CountResult(dbEntry);
          }

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
      catch (Exception ex)
      {
        _logger.LogUnexpectedException(ex);
        builder.WithException(ex);
        return null;
      }
      finally
      {
        await _metrics.SubmitPointAsync(builder);
      }
    }

    public async Task<List<IntListItem>> GetClientProductsListItems(int userId, int clientId)
    {
      // TODO: [TESTS] (ProductService.GetClientProductsListItems) Add tests
      var builder = new ServiceMetricBuilder(nameof(ProductService), nameof(GetClientProductsListItems))
        .WithCategory(MetricCategory.Product, MetricSubCategory.GetList)
        .WithCustomInt1(userId)
        .WithCustomInt2(clientId);

      try
      {
        using (builder.WithTiming())
        {
          List<ProductEntity> dbEntries;
          using (builder.WithCustomTiming1())
          {
            builder.IncrementQueryCount();
            dbEntries = await _productRepo.GetAll(clientId);
            builder.WithResultCount(dbEntries?.Count ?? 0);
          }

          if (dbEntries == null || dbEntries.Count == 0)
          {
            // TODO: [HANDLE] (ProductService.GetClientProductsListItems) Handle this
            return new List<IntListItem>();
          }

          if (dbEntries.First().UserId != userId)
          {
            // TODO: [HANDLE] (ProductService.GetClientProductsListItems) Handle this
            return new List<IntListItem>();
          }

          return dbEntries
            .AsQueryable()
            .Select(product => new IntListItem
            {
              Name = product.ProductName,
              Value = product.ProductId
            })
            .ToList();
        }
      }
      catch (Exception ex)
      {
        _logger.LogUnexpectedException(ex);
        builder.WithException(ex);
        return new List<IntListItem>();
      }
      finally
      {
        await _metrics.SubmitPointAsync(builder);
      }
    }
  }
}
