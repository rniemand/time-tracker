CREATE TABLE `TimeSheet_Date` (
	`DateId` INT(11) NOT NULL AUTO_INCREMENT,
	`UserId` INT(11) NOT NULL,
	`ClientId` INT(11) NOT NULL,
	`Deleted` BIT(1) NOT NULL DEFAULT b'0',
	`DateAddedUtc` DATETIME NOT NULL DEFAULT utc_timestamp(4),
	`DateUpdatedUtc` DATETIME NULL DEFAULT NULL,
	`EntryDate` DATE NOT NULL DEFAULT curdate(),
	`DayOfWeek` TINYINT(4) NOT NULL DEFAULT '0',
	PRIMARY KEY (`DateId`) USING BTREE,
	INDEX `Deleted` (`Deleted`) USING BTREE,
	INDEX `UserId` (`UserId`) USING BTREE,
	INDEX `ClientId` (`ClientId`) USING BTREE,
	INDEX `DayOfWeek` (`DayOfWeek`) USING BTREE,
	CONSTRAINT `FK_TimeSheet_Date_Clients` FOREIGN KEY (`ClientId`) REFERENCES `TimeTrackerDev`.`Clients` (`ClientId`) ON UPDATE RESTRICT ON DELETE RESTRICT,
	CONSTRAINT `FK_TimeSheet_Date_Users` FOREIGN KEY (`UserId`) REFERENCES `TimeTrackerDev`.`Users` (`UserId`) ON UPDATE RESTRICT ON DELETE RESTRICT
)
COLLATE='utf8_general_ci'
ENGINE=InnoDB
;

CREATE TABLE `TimeSheet_Rows` (
	`RowId` BIGINT(20) NOT NULL AUTO_INCREMENT,
	`DateId` INT(11) NOT NULL,
	`UserId` INT(11) NOT NULL,
	`ClientId` INT(11) NOT NULL,
	`ProductId` INT(11) NOT NULL,
	`ProjectId` INT(11) NOT NULL,
	`Deleted` BIT(1) NOT NULL DEFAULT b'0',
	`DateAddedUtc` DATETIME NOT NULL DEFAULT utc_timestamp(4),
	`DateUpdatedUtc` DATETIME NULL DEFAULT NULL,
	`EntryDate` DATE NOT NULL,
	`Description` VARCHAR(256) NOT NULL DEFAULT '' COLLATE 'utf8_general_ci',
	PRIMARY KEY (`RowId`) USING BTREE,
	INDEX `DayId` (`DateId`) USING BTREE,
	INDEX `Deleted` (`Deleted`) USING BTREE,
	INDEX `UserId` (`UserId`) USING BTREE,
	INDEX `ProductId` (`ProductId`) USING BTREE,
	INDEX `ProjectId` (`ProjectId`) USING BTREE,
	INDEX `ClientId` (`ClientId`) USING BTREE,
	CONSTRAINT `FK_TimeSheet_Rows_Clients` FOREIGN KEY (`ClientId`) REFERENCES `TimeTrackerDev`.`Clients` (`ClientId`) ON UPDATE RESTRICT ON DELETE RESTRICT,
	CONSTRAINT `FK_TimeSheet_Rows_Products` FOREIGN KEY (`ProductId`) REFERENCES `TimeTrackerDev`.`Products` (`ProductId`) ON UPDATE RESTRICT ON DELETE RESTRICT,
	CONSTRAINT `FK_TimeSheet_Rows_Projects` FOREIGN KEY (`ProjectId`) REFERENCES `TimeTrackerDev`.`Projects` (`ProjectId`) ON UPDATE RESTRICT ON DELETE RESTRICT,
	CONSTRAINT `FK_TimeSheet_Rows_TimeSheet_Date` FOREIGN KEY (`DateId`) REFERENCES `TimeTrackerDev`.`TimeSheet_Date` (`DateId`) ON UPDATE RESTRICT ON DELETE RESTRICT,
	CONSTRAINT `FK_TimeSheet_Rows_Users` FOREIGN KEY (`UserId`) REFERENCES `TimeTrackerDev`.`Users` (`UserId`) ON UPDATE RESTRICT ON DELETE RESTRICT
)
COLLATE='utf8_general_ci'
ENGINE=InnoDB
;

CREATE TABLE `TimeSheet_Entries` (
	`EntryId` BIGINT(20) NOT NULL DEFAULT '0',
	`RowId` BIGINT(20) NOT NULL,
	`DateId` INT(11) NOT NULL,
	`UserId` INT(11) NOT NULL,
	`ClientId` INT(11) NOT NULL,
	`ProductId` INT(11) NOT NULL,
	`ProjectId` INT(11) NOT NULL,
	`Deleted` BIT(1) NOT NULL DEFAULT b'0',
	`DateAddedUtc` DATETIME NOT NULL DEFAULT utc_timestamp(4),
	`DateUpdatedUtc` DATETIME NULL DEFAULT NULL,
	`EntryDate` DATE NOT NULL,
	`EntryVersion` TINYINT(4) NOT NULL DEFAULT '1',
	`EntryTimeMin` INT(11) NOT NULL DEFAULT '0',
	PRIMARY KEY (`EntryId`) USING BTREE,
	INDEX `RowId` (`RowId`) USING BTREE,
	INDEX `DateId` (`DateId`) USING BTREE,
	INDEX `Deleted` (`Deleted`) USING BTREE,
	INDEX `UserId` (`UserId`) USING BTREE,
	INDEX `ClientId` (`ClientId`) USING BTREE,
	INDEX `ProductId` (`ProductId`) USING BTREE,
	INDEX `ProjectId` (`ProjectId`) USING BTREE,
	CONSTRAINT `FK_TimeSheet_Entries_Clients` FOREIGN KEY (`ClientId`) REFERENCES `TimeTrackerDev`.`Clients` (`ClientId`) ON UPDATE RESTRICT ON DELETE RESTRICT,
	CONSTRAINT `FK_TimeSheet_Entries_Products` FOREIGN KEY (`ProductId`) REFERENCES `TimeTrackerDev`.`Products` (`ProductId`) ON UPDATE RESTRICT ON DELETE RESTRICT,
	CONSTRAINT `FK_TimeSheet_Entries_Projects` FOREIGN KEY (`ProjectId`) REFERENCES `TimeTrackerDev`.`Projects` (`ProjectId`) ON UPDATE RESTRICT ON DELETE RESTRICT,
	CONSTRAINT `FK_TimeSheet_Entries_TimeSheet_Date` FOREIGN KEY (`DateId`) REFERENCES `TimeTrackerDev`.`TimeSheet_Date` (`DateId`) ON UPDATE RESTRICT ON DELETE RESTRICT,
	CONSTRAINT `FK_TimeSheet_Entries_TimeSheet_Rows` FOREIGN KEY (`RowId`) REFERENCES `TimeTrackerDev`.`TimeSheet_Rows` (`RowId`) ON UPDATE RESTRICT ON DELETE RESTRICT,
	CONSTRAINT `FK_TimeSheet_Entries_Users` FOREIGN KEY (`UserId`) REFERENCES `TimeTrackerDev`.`Users` (`UserId`) ON UPDATE RESTRICT ON DELETE RESTRICT
)
COLLATE='utf8_general_ci'
ENGINE=InnoDB
;
