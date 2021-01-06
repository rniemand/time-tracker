using System;
using System.Linq.Expressions;
using FluentValidation;
using FluentValidation.Results;
using TimeTracker.Core.Database.Entities;

namespace TimeTracker.Core.Models.Dto
{
  public class ClientDto
  {
    public int ClientId { get; set; }
    public bool Deleted { get; set; }
    public int UserId { get; set; }
    public DateTime DateCreatedUtc { get; set; }
    public DateTime? DateModifiedUtc { get; set; }
    public string ClientName { get; set; }
    public string ClientEmail { get; set; }

    public static Expression<Func<ClientEntity, ClientDto>> Projection
    {
      get
      {
        return entity => new ClientDto
        {
          UserId = entity.UserId,
          DateCreatedUtc = entity.DateCreatedUtc,
          Deleted = entity.Deleted,
          ClientEmail = entity.ClientEmail,
          ClientId = entity.ClientId,
          ClientName = entity.ClientName,
          DateModifiedUtc = entity.DateModifiedUtc
        };
      }
    }

    public static ClientDto FromEntity(ClientEntity entity)
    {
      // TODO: [TESTS] (ClientDto.FromEntity) Add tests
      return entity == null ? null : Projection.Compile()(entity);
    }

    public ClientDto()
    {
      // TODO: [TESTS] (ClientDto) Add tests
      ClientId = 0;
      Deleted = false;
      UserId = 0;
      DateCreatedUtc = DateTime.UtcNow;
      DateModifiedUtc = null;
      ClientName = string.Empty;
      ClientEmail = string.Empty;
    }

    public ClientEntity ToDbEntity()
    {
      // TODO: [TESTS] (ClientDto.ToDbEntity) Add tests
      return new ClientEntity
      {
        UserId = UserId,
        DateCreatedUtc = DateCreatedUtc,
        ClientEmail = ClientEmail,
        ClientId = ClientId,
        DateModifiedUtc = DateModifiedUtc,
        ClientName = ClientName,
        Deleted = Deleted
      };
    }
  }

  public class ClientDtoValidator : AbstractValidator<ClientDto>
  {
    public ClientDtoValidator()
    {
      RuleSet("Add", () =>
      {
        RuleFor(x => x.UserId).GreaterThan(0);
        RuleFor(x => x.ClientName).NotNull().MinimumLength(3);
      });

      RuleSet("Update", () =>
      {
        RuleFor(x => x.ClientName).NotNull().MinimumLength(3);
        RuleFor(x => x.ClientEmail).EmailAddress().When(x => x.ClientEmail.Length > 0);
        RuleFor(x => x.ClientId).GreaterThan(0);
      });
    }

    public static ValidationResult Add(ClientDto clientDto)
    {
      return new ClientDtoValidator().Validate(clientDto,
        options => options.IncludeRuleSets("Add")
      );
    }

    public static ValidationResult Update(ClientDto clientDto)
    {
      return new ClientDtoValidator().Validate(clientDto,
        options => options.IncludeRuleSets("Update")
      );
    }
  }
}
