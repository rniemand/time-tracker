﻿using System;
using System.Linq.Expressions;
using FluentValidation;
using FluentValidation.Results;
using TimeTracker.Core.Database.Entities;

namespace TimeTracker.Core.Models.Dto
{
  public class ProductDto
  {
    public int ProductId { get; set; }
    public int ClientId { get; set; }
    public int UserId { get; set; }
    public bool Deleted { get; set; }
    public DateTime DateAddedUtc { get; set; }
    public DateTime? DateUpdatedUtc { get; set; }
    public DateTime? DateDeletedUtc { get; set; }
    public string ProductName { get; set; }

    public static Expression<Func<ProductEntity, ProductDto>> Projection
    {
      get
      {
        return entity => new ProductDto
        {
          UserId = entity.UserId,
          DateAddedUtc = entity.DateAddedUtc,
          ClientId = entity.ClientId,
          DateUpdatedUtc = entity.DateUpdatedUtc,
          Deleted = entity.Deleted,
          ProductName = entity.ProductName,
          ProductId = entity.ProductId,
          DateDeletedUtc = entity.DateDeletedUtc
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
      DateAddedUtc = DateTime.UtcNow;
      DateUpdatedUtc = null;
      DateDeletedUtc = null;
      ProductName = string.Empty;
    }

    public ProductEntity AsProductEntity()
    {
      // TODO: [TESTS] (ProductDto.AsProductEntity) Add tests
      return new ProductEntity
      {
        UserId = UserId,
        DateAddedUtc = DateAddedUtc,
        ClientId = ClientId,
        DateUpdatedUtc = DateUpdatedUtc,
        Deleted = Deleted,
        ProductName = ProductName,
        ProductId = ProductId,
        DateDeletedUtc = DateDeletedUtc
      };
    }
  }

  public class ProductDtoValidator : AbstractValidator<ProductDto>
  {
    public ProductDtoValidator()
    {
      RuleSet("Add", () =>
      {
        RuleFor(x => x.ClientId).GreaterThan(0);
        RuleFor(x => x.UserId).GreaterThan(0);
        RuleFor(x => x.ProductName).NotNull().MinimumLength(3);
      });

      RuleSet("Update", () =>
      {
        RuleFor(x => x.ProductName).NotNull().MinimumLength(3);
        RuleFor(x => x.ProductId).GreaterThan(0);
      });
    }

    public static ValidationResult Add(ProductDto productDto)
    {
      return new ProductDtoValidator().Validate(productDto,
        options => options.IncludeRuleSets("Add")
      );
    }

    public static ValidationResult Update(ProductDto productDto)
    {
      return new ProductDtoValidator().Validate(productDto,
        options => options.IncludeRuleSets("Update")
      );
    }
  }
}
