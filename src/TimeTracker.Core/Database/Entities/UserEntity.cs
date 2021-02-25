using System;

namespace TimeTracker.Core.Database.Entities
{
  public class UserEntity
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
    public string Password { get; set; }
    public string UserEmail { get; set; }

    public UserEntity()
    {
      // TODO: [TESTS] (UserEntity) Add tests
      UserId = 0;
      Deleted = false;
      DateAddedUtc = DateTime.UtcNow;
      DateUpdatedUtc = null;
      DateDeletedUtc = null;
      LastLoginDate = null;
      FirstName = string.Empty;
      LastName = string.Empty;
      Username = string.Empty;
      Password = string.Empty;
      UserEmail = string.Empty;
    }
  }
}
