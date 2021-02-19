using System;
using System.Linq.Expressions;
using TimeTracker.Core.Database.Entities;

namespace TimeTracker.Core.Models.Dto
{
  public class UserDto
  {
    public int UserId { get; set; }
    public bool Deleted { get; set; }
    public DateTime DateAddedUtc { get; set; }
    public DateTime? DateUpdatedUtc { get; set; }
    public DateTime? DateDeletedUtc { get; set; }
    public DateTime? LastLoginDate { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Username { get; set; }
    public string UserEmail { get; set; }

    public static Expression<Func<UserEntity, UserDto>> Projection
    {
      get
      {
        return entity => new UserDto
        {
          DateAddedUtc = entity.DateAddedUtc,
          DateUpdatedUtc = entity.DateUpdatedUtc,
          Deleted = entity.Deleted,
          FirstName = entity.FirstName,
          LastLoginDate = entity.LastLoginDate,
          LastName = entity.LastName,
          UserEmail = entity.UserEmail,
          UserId = entity.UserId,
          Username = entity.Username,
          DateDeletedUtc = entity.DateDeletedUtc
        };
      }
    }

    public UserDto()
    {
      // TODO: [TESTS] (UserDto) Add tests
      UserId = 0;
      Deleted = false;
      DateAddedUtc = DateTime.UtcNow;
      DateUpdatedUtc = null;
      LastLoginDate = null;
      DateDeletedUtc = null;
      FirstName = string.Empty;
      LastName = string.Empty;
      Username = string.Empty;
      UserEmail = string.Empty;
    }
  }
}
