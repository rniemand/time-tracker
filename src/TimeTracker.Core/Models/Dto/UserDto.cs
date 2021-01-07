using System;
using System.Linq.Expressions;
using TimeTracker.Core.Database.Entities;

namespace TimeTracker.Core.Models.Dto
{
  public class UserDto
  {
    public int UserId { get; set; }
    public bool Deleted { get; set; }
    public DateTime DateCreatedUtc { get; set; }
    public DateTime? DateModified { get; set; }
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
          DateCreatedUtc = entity.DateCreatedUtc,
          DateModified = entity.DateModified,
          Deleted = entity.Deleted,
          FirstName = entity.FirstName,
          LastLoginDate = entity.LastLoginDate,
          LastName = entity.LastName,
          UserEmail = entity.UserEmail,
          UserId = entity.UserId,
          Username = entity.Username
        };
      }
    }

    public UserDto()
    {
      // TODO: [TESTS] (UserDto) Add tests
      UserId = 0;
      Deleted = false;
      DateCreatedUtc = DateTime.UtcNow;
      DateModified = null;
      LastLoginDate = null;
      FirstName = string.Empty;
      LastName = string.Empty;
      Username = string.Empty;
      UserEmail = string.Empty;
    }
  }
}
