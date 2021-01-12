using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using Rn.NetCore.Common.Logging;
using Rn.NetCore.Common.Metrics;
using TimeTracker.Core.Models.Dto;
using TimeTracker.Core.Models.Responses;
using TimeTracker.Core.Services;
using TimeTracker.Core.WebApi;
using TimeTracker.Core.WebApi.Attributes;
using TimeTracker.Core.WebApi.Requests;

namespace TimeTracker.Controllers
{
  [ApiController, Route("api/[controller]")]
  public class ProductsController : BaseController<ProductsController>
  {
    private readonly IProductService _productService;

    public ProductsController(
      ILoggerAdapter<ProductsController> logger,
      IMetricService metrics,
      IUserService userService,
      IProductService productService
    ) : base(logger, metrics, userService)
    {
      _productService = productService;
    }

    [HttpGet, Route("products/{clientId}"), Authorize]
    public async Task<ActionResult<List<ProductDto>>> GetAllProducts(
      [FromRoute] int clientId,
      [OpenApiIgnore] CoreApiRequest request)
    {
      // TODO: [TESTS] (ProductsController.GetAllProducts) Add tests
      var response = new BaseResponse<List<ProductDto>>()
        .WithValidation(new AdHockValidator()
          .GreaterThanZero(nameof(clientId), clientId)
        );

      if (response.PassedValidation)
        response.WithResponse(await _productService.GetAll(
          request.UserId,
          clientId
        ));

      return ProcessResponse(response);
    }

    [HttpGet, Route("products/list/{clientId}"), Authorize]
    public async Task<ActionResult<List<IntListItem>>> ListClientProducts(
      [FromRoute] int clientId,
      [OpenApiIgnore] CoreApiRequest request)
    {
      // TODO: [TESTS] (ProductsController.ListClientProducts) Add tests
      var response = new BaseResponse<List<IntListItem>>()
        .WithValidation(new AdHockValidator()
          .GreaterThanZero(nameof(clientId), clientId)
        );

      if (response.PassedValidation)
        response.WithResponse(await _productService.GetClientProductsListItems(
          request.UserId,
          clientId
        ));

      return ProcessResponse(response);
    }

    [HttpPost, Route("product/add"), Authorize]
    public async Task<ActionResult<bool>> AddProduct(
      [FromBody] ProductDto productDto,
      [OpenApiIgnore] CoreApiRequest request)
    {
      // TODO: [TESTS] (ProductsController.AddProduct) Add tests
      var response = new BaseResponse<bool>()
        .WithValidation(ProductDtoValidator.Add(productDto));

      if (response.PassedValidation)
        response.WithResponse(await _productService.AddProduct(request.UserId, productDto));

      return ProcessResponse(response);
    }

    [HttpPatch, Route("product/update"), Authorize]
    public async Task<ActionResult<bool>> UpdateProduct(
      [FromBody] ProductDto productDto,
      [OpenApiIgnore] CoreApiRequest request)
    {
      // TODO: [TESTS] (ProductsController.UpdateProduct) Add tests
      var response = new BaseResponse<bool>()
        .WithValidation(ProductDtoValidator.Update(productDto));

      if (response.PassedValidation)
        response.WithResponse(await _productService.UpdateProduct(request.UserId, productDto));

      return ProcessResponse(response);
    }

    [HttpGet, Route("product/{productId}"), Authorize]
    public async Task<ActionResult<ProductDto>> GetProductById(
      [FromRoute] int productId,
      [OpenApiIgnore] CoreApiRequest request)
    {
      // TODO: [TESTS] (ProductsController.GetProductById) Add tests
      var response = new BaseResponse<ProductDto>()
        .WithValidation(new AdHockValidator()
          .GreaterThanZero(nameof(productId), productId)
        );

      if (response.PassedValidation)
        response.WithResponse(await _productService.GetById(
          request.UserId,
          productId
        ));

      return ProcessResponse(response);
    }
  }
}
