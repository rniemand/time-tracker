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

CREATE TABLE `Products` (
	`ProductId` INT(11) NOT NULL AUTO_INCREMENT,
	`ClientId` INT(11) NOT NULL DEFAULT '0',
	`UserId` INT(11) NOT NULL DEFAULT '0',
	`Deleted` BIT(1) NOT NULL DEFAULT b'0',
	`DateCreatedUtc` DATETIME NOT NULL DEFAULT current_timestamp(),
	`DateModifiedUtc` DATETIME NULL DEFAULT NULL,
	`ProductName` VARCHAR(256) NOT NULL DEFAULT '' COLLATE 'utf8mb4_general_ci',
	PRIMARY KEY (`ProductId`) USING BTREE,
	INDEX `ClientId` (`ClientId`) USING BTREE,
	INDEX `UserId` (`UserId`) USING BTREE,
	INDEX `Deleted` (`Deleted`) USING BTREE
)
COLLATE='utf8mb4_general_ci'
ENGINE=InnoDB
;

CREATE TABLE `Projects` (
	`ProjectId` INT(11) NOT NULL AUTO_INCREMENT,
	`ClientId` INT(11) NULL DEFAULT NULL,
	`ProductId` INT(11) NOT NULL DEFAULT '0',
	`UserId` INT(11) NOT NULL DEFAULT '0',
	`Deleted` BIT(1) NOT NULL DEFAULT b'0',
	`DateCreatedUtc` DATETIME NOT NULL DEFAULT current_timestamp(),
	`DateModifiedUtc` DATETIME NULL DEFAULT NULL,
	`ProjectName` VARCHAR(256) NOT NULL DEFAULT '' COLLATE 'utf8mb4_general_ci',
	PRIMARY KEY (`ProjectId`) USING BTREE,
	INDEX `UserId` (`UserId`) USING BTREE,
	INDEX `Deleted` (`Deleted`) USING BTREE,
	INDEX `ProductId` (`ProductId`) USING BTREE,
	INDEX `ClientId` (`ClientId`) USING BTREE
)
COLLATE='utf8mb4_general_ci'
ENGINE=InnoDB
;

CREATE TABLE `RawTimers` (
	`EntryId` BIGINT(20) NOT NULL AUTO_INCREMENT,
	`ParentEntryId` BIGINT(20) NOT NULL DEFAULT '0',
	`RootParentEntryId` BIGINT(20) NOT NULL DEFAULT '0',
	`ClientId` INT(11) NOT NULL DEFAULT '0',
	`ProductId` INT(11) NOT NULL DEFAULT '0',
	`ProjectId` INT(11) NOT NULL DEFAULT '0',
	`UserId` INT(11) NOT NULL DEFAULT '0',
	`Deleted` BIT(1) NOT NULL DEFAULT b'0',
	`Running` BIT(1) NOT NULL DEFAULT b'1',
	`Completed` BIT(1) NOT NULL DEFAULT b'0',
	`Processed` BIT(1) NOT NULL DEFAULT b'0',
	`EntryState` TINYINT(4) NOT NULL DEFAULT '1',
	`EntryRunningTimeSec` INT(11) NOT NULL DEFAULT '0',
	`EntryStartTimeUtc` TIMESTAMP NOT NULL DEFAULT current_timestamp(),
	`EntryEndTimeUtc` DATETIME NULL DEFAULT NULL,
	PRIMARY KEY (`EntryId`) USING BTREE,
	INDEX `UserId` (`UserId`) USING BTREE
)
COLLATE='utf8mb4_general_ci'
ENGINE=InnoDB
AUTO_INCREMENT=6
;

