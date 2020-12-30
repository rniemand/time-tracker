using System;
using System.Linq.Expressions;
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
  }
}
