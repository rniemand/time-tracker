using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using Rn.NetCore.Common.Logging;
using Rn.NetCore.Common.Metrics;
using TimeTracker.Core.Models.Dto;
using TimeTracker.Core.Services;
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
    public async Task<ActionResult<List<ProductDto>>> GetAll(
      [FromRoute] int clientId,
      [OpenApiIgnore] CoreApiRequest request)
    {
      // TODO: [TESTS] (ProductsController.GetAll) Add tests
      return Ok(await _productService.GetAll(request.UserId, clientId));
    }

    [HttpGet, Route("products/{clientId}/list-items"), Authorize]
    public async Task<ActionResult<List<IntListItem>>> GetClientProductsListItems(
      [FromRoute] int clientId,
      [OpenApiIgnore] CoreApiRequest request)
    {
      // TODO: [TESTS] (ProductsController.GetClientProductsListItems) Add tests
      return Ok(await _productService.GetClientProductsListItems(request.UserId, clientId));
    }


    [HttpPost, Route("product/add"), Authorize]
    public async Task<ActionResult<ProductDto>> AddProduct(
      [FromBody] ProductDto product,
      [OpenApiIgnore] CoreApiRequest request)
    {
      // TODO: [TESTS] (ProductsController.AddProduct) Add tests
      return Ok(await _productService.AddProduct(request.UserId, product));
    }

    [HttpPatch, Route("product/update"), Authorize]
    public async Task<ActionResult<ProductDto>> UpdateProduct(
      [FromBody] ProductDto product,
      [OpenApiIgnore] CoreApiRequest request)
    {
      // TODO: [TESTS] (ProductsController.UpdateProduct) Add tests
      return Ok(await _productService.UpdateProduct(request.UserId, product));
    }

    [HttpGet, Route("product/{productId}"), Authorize]
    public async Task<ActionResult<ProductDto>> GetProductById(
      [FromRoute] int productId,
      [OpenApiIgnore] CoreApiRequest request)
    {
      // TODO: [TESTS] (ProductsController.GetProductById) Add tests
      return Ok(await _productService.GetById(request.UserId, productId));
    }
  }
}
