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
