using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using TimeTracker.Core.Models.Dto;
using TimeTracker.Core.Services;
using TimeTracker.Core.WebApi.Attributes;
using TimeTracker.Core.WebApi.Requests;

namespace TimeTracker.Controllers
{
  [ApiController, Route("api/[controller]")]
  public class ProductsController : ControllerBase
  {
    private readonly IProductService _productService;

    public ProductsController(IProductService productService)
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
  }
}
