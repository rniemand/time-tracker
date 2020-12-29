using System;

namespace TimeTracker.Core.Database.Entities
{
  public class UserEntity
  {
    public int UserId { get; set; }
    public bool Deleted { get; set; }
    public DateTime DateCreatedUtc { get; set; }

		/*4
	`DateModified` DATETIME NULL DEFAULT NULL,
	`LastLoginDate` DATETIME NULL DEFAULT NULL,
	`FirstName` VARCHAR(64) NOT NULL COLLATE 'utf8mb4_general_ci',
	`LastName` VARCHAR(64) NOT NULL COLLATE 'utf8mb4_general_ci',
	`Username` VARCHAR(64) NOT NULL COLLATE 'utf8mb4_general_ci',
	`Password` VARCHAR(128) NOT NULL COLLATE 'utf8mb4_general_ci',
	`UserEmail` VARCHAR(128) NOT NULL COLLATE 'utf8mb4_general_ci',
     */

    public UserEntity()
    {
			// TODO: [TESTS] (UserEntity) Add tests
      UserId = 0;
      Deleted = false;
      DateCreatedUtc = DateTime.UtcNow;
    }
	}
}
