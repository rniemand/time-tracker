CREATE TABLE `Users` (
	`UserId` INT(11) NOT NULL AUTO_INCREMENT,
	`Deleted` BIT(1) NOT NULL DEFAULT b'0',
	`DateCreatedUtc` DATETIME NOT NULL DEFAULT current_timestamp(),
	`DateModifiedUtc` DATETIME NULL DEFAULT NULL,
	`LastLoginDateUtc` DATETIME NULL DEFAULT NULL,
	`FirstName` VARCHAR(64) NULL DEFAULT NULL COLLATE 'utf8mb4_general_ci',
	`LastName` VARCHAR(64) NULL DEFAULT NULL COLLATE 'utf8mb4_general_ci',
	`Username` VARCHAR(64) NOT NULL COLLATE 'utf8mb4_general_ci',
	`Password` VARCHAR(128) NOT NULL COLLATE 'utf8mb4_general_ci',
	`UserEmail` VARCHAR(128) NOT NULL COLLATE 'utf8mb4_general_ci',
	PRIMARY KEY (`UserId`) USING BTREE,
	INDEX `Deleted` (`Deleted`) USING BTREE
)
COLLATE='utf8mb4_general_ci'
ENGINE=InnoDB
;

CREATE TABLE `Clients` (
	`ClientId` INT(11) NOT NULL AUTO_INCREMENT,
	`Deleted` BIT(1) NOT NULL DEFAULT b'0',
	`UserId` INT(11) NOT NULL DEFAULT '0',
	`DateCreatedUtc` DATETIME NOT NULL DEFAULT current_timestamp(),
	`DateModifiedUtc` DATETIME NULL DEFAULT NULL,
	`ClientName` VARCHAR(128) NOT NULL DEFAULT '' COLLATE 'utf8mb4_general_ci',
	`ClientEmail` VARCHAR(256) NULL DEFAULT NULL COLLATE 'utf8mb4_general_ci',
	PRIMARY KEY (`ClientId`) USING BTREE,
	INDEX `Deleted` (`Deleted`) USING BTREE,
	INDEX `UserId` (`UserId`) USING BTREE
)
COLLATE='utf8mb4_general_ci'
ENGINE=InnoDB
;
