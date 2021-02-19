ALTER TABLE `Clients`
	CHANGE COLUMN `UserId` `UserId` INT(11) NOT NULL DEFAULT '0' AFTER `ClientId`,
	CHANGE COLUMN `Deleted` `Deleted` BIT(1) NOT NULL DEFAULT b'0' AFTER `UserId`,
	CHANGE COLUMN `DateCreatedUtc` `DateCreatedUtc` DATETIME NOT NULL DEFAULT utc_timestamp(4) AFTER `Deleted`,
	ADD CONSTRAINT `FK_Clients_Users` FOREIGN KEY (`UserId`) REFERENCES `Users` (`UserId`);
	
ALTER TABLE `Options`
	CHANGE COLUMN `UserId` `UserId` INT(11) NOT NULL DEFAULT '0' AFTER `OptionId`;
	
ALTER TABLE `Options`
	CHANGE COLUMN `UserId` `UserId` INT(11) NOT NULL DEFAULT '0' AFTER `OptionId`;
	
ALTER TABLE `Products`
	CHANGE COLUMN `UserId` `UserId` INT(11) NOT NULL DEFAULT '0' AFTER `ProductId`,
	CHANGE COLUMN `DateCreatedUtc` `DateCreatedUtc` DATETIME NOT NULL DEFAULT utc_timestamp(4) AFTER `Deleted`,
	ADD CONSTRAINT `FK_Products_Users` FOREIGN KEY (`UserId`) REFERENCES `Users` (`UserId`),
	ADD CONSTRAINT `FK_Products_Clients` FOREIGN KEY (`ClientId`) REFERENCES `Clients` (`ClientId`);
	
ALTER TABLE `Projects`
	CHANGE COLUMN `UserId` `UserId` INT(11) NOT NULL AFTER `ProjectId`,
	CHANGE COLUMN `ClientId` `ClientId` INT(11) NOT NULL AFTER `UserId`,
	CHANGE COLUMN `ProductId` `ProductId` INT(11) NOT NULL AFTER `ClientId`;
	
ALTER TABLE `Projects`
	CHANGE COLUMN `DateCreatedUtc` `DateCreatedUtc` DATETIME NOT NULL DEFAULT utc_timestamp(4) AFTER `Deleted`,
	ADD CONSTRAINT `FK_Projects_Users` FOREIGN KEY (`UserId`) REFERENCES `Users` (`UserId`),
	ADD CONSTRAINT `FK_Projects_Clients` FOREIGN KEY (`ClientId`) REFERENCES `Clients` (`ClientId`),
	ADD CONSTRAINT `FK_Projects_Products` FOREIGN KEY (`ProductId`) REFERENCES `Products` (`ProductId`);
	
ALTER TABLE `Users`
	CHANGE COLUMN `DateCreatedUtc` `DateCreatedUtc` DATETIME NOT NULL DEFAULT utc_timestamp(4) AFTER `Deleted`,
	CHANGE COLUMN `FirstName` `FirstName` VARCHAR(64) NOT NULL COLLATE 'utf8mb4_general_ci' AFTER `LastLoginDateUtc`;
	
ALTER TABLE `Clients`
	CHANGE COLUMN `UserId` `UserId` INT(11) NOT NULL AFTER `ClientId`,
	CHANGE COLUMN `ClientName` `ClientName` VARCHAR(128) NOT NULL COLLATE 'utf8mb4_general_ci' AFTER `DateModifiedUtc`;
	
ALTER TABLE `DailyTasks`
	CHANGE COLUMN `UserId` `UserId` INT(11) NOT NULL AFTER `TaskId`,
	CHANGE COLUMN `ClientId` `ClientId` INT(11) NOT NULL AFTER `UserId`,
	CHANGE COLUMN `DateCreatedUtc` `DateCreatedUtc` DATETIME NOT NULL DEFAULT utc_timestamp(4) AFTER `Deleted`,
	CHANGE COLUMN `TaskName` `TaskName` VARCHAR(128) NOT NULL COLLATE 'utf8mb4_general_ci' AFTER `DateModifiedUtc`,
	ADD CONSTRAINT `FK_DailyTasks_Users` FOREIGN KEY (`UserId`) REFERENCES `Users` (`UserId`),
	ADD CONSTRAINT `FK_DailyTasks_Clients` FOREIGN KEY (`ClientId`) REFERENCES `Clients` (`ClientId`);
	
ALTER TABLE `Products`
	CHANGE COLUMN `UserId` `UserId` INT(11) NOT NULL AFTER `ProductId`,
	CHANGE COLUMN `ClientId` `ClientId` INT(11) NOT NULL AFTER `UserId`,
	CHANGE COLUMN `ProductName` `ProductName` VARCHAR(256) NOT NULL COLLATE 'utf8mb4_general_ci' AFTER `DateModifiedUtc`;
	
ALTER TABLE `Projects`
	CHANGE COLUMN `ProjectName` `ProjectName` VARCHAR(256) NOT NULL COLLATE 'utf8mb4_general_ci' AFTER `DateModifiedUtc`;
	
ALTER TABLE `Timers`
	CHANGE COLUMN `ClientId` `ClientId` INT(11) NOT NULL AFTER `EntryId`,
	CHANGE COLUMN `ProductId` `ProductId` INT(11) NOT NULL DEFAULT 0 AFTER `ClientId`,
	CHANGE COLUMN `ProjectId` `ProjectId` INT(11) NOT NULL DEFAULT 0 AFTER `ProductId`,
	CHANGE COLUMN `TaskId` `TaskId` INT(11) NOT NULL DEFAULT 0 AFTER `ProjectId`,
	CHANGE COLUMN `UserId` `UserId` INT(11) NOT NULL AFTER `TaskId`,
	CHANGE COLUMN `Notes` `Notes` VARCHAR(512) NOT NULL DEFAULT '' COLLATE 'utf8mb4_general_ci' AFTER `EndTimeUtc`,
	ADD CONSTRAINT `FK_Timers_Clients` FOREIGN KEY (`ClientId`) REFERENCES `Clients` (`ClientId`),
	ADD CONSTRAINT `FK_Timers_Users` FOREIGN KEY (`UserId`) REFERENCES `Users` (`UserId`);
	
ALTER TABLE `Timers`
	CHANGE COLUMN `StartTimeUtc` `StartTimeUtc` TIMESTAMP NOT NULL DEFAULT utc_timestamp(4) AFTER `TotalSeconds`;
	
ALTER TABLE `ToDoCategory`
	CHANGE COLUMN `UserId` `UserId` INT(11) NOT NULL AFTER `CategoryId`,
	CHANGE COLUMN `DateCreatedUtc` `DateCreatedUtc` TIMESTAMP NOT NULL DEFAULT utc_timestamp(4) AFTER `Deleted`,
	CHANGE COLUMN `Category` `Category` VARCHAR(128) NOT NULL COLLATE 'utf8mb4_general_ci' AFTER `DateModifiedUtc`,
	ADD INDEX `Deleted` (`Deleted`),
	ADD CONSTRAINT `FK_ToDoCategory_Users` FOREIGN KEY (`UserId`) REFERENCES `Users` (`UserId`);
	
ALTER TABLE `ToDoList`
	CHANGE COLUMN `UserId` `UserId` INT(11) NOT NULL AFTER `ToDoId`,
	CHANGE COLUMN `CategoryId` `CategoryId` INT(11) NOT NULL AFTER `UserId`,
	CHANGE COLUMN `SubCategoryId` `SubCategoryId` INT(11) NOT NULL AFTER `CategoryId`,
	CHANGE COLUMN `DateAddedUtc` `DateAddedUtc` DATETIME NOT NULL DEFAULT utc_timestamp(4) AFTER `Deleted`,
	CHANGE COLUMN `ToDo` `ToDo` VARCHAR(256) NOT NULL COLLATE 'utf8mb4_general_ci' AFTER `DateModifiedUtc`,
	ADD CONSTRAINT `FK_ToDoList_Users` FOREIGN KEY (`UserId`) REFERENCES `Users` (`UserId`),
	ADD CONSTRAINT `FK_ToDoList_ToDoCategory` FOREIGN KEY (`CategoryId`) REFERENCES `ToDoCategory` (`CategoryId`),
	ADD CONSTRAINT `FK_ToDoList_ToDoSubCategory` FOREIGN KEY (`SubCategoryId`) REFERENCES `ToDoSubCategory` (`SubCategoryId`);
	
ALTER TABLE `ToDoSubCategory`
	CHANGE COLUMN `UserId` `UserId` INT(11) NOT NULL AFTER `SubCategoryId`,
	CHANGE COLUMN `CategoryId` `CategoryId` INT(11) NOT NULL AFTER `UserId`,
	CHANGE COLUMN `DateCreatedUtc` `DateCreatedUtc` DATETIME NOT NULL DEFAULT utc_timestamp(4) AFTER `Deleted`,
	CHANGE COLUMN `SubCategory` `SubCategory` VARCHAR(128) NOT NULL COLLATE 'utf8mb4_general_ci' AFTER `DateModifiedUtc`,
	ADD CONSTRAINT `FK_ToDoSubCategory_Users` FOREIGN KEY (`UserId`) REFERENCES `Users` (`UserId`),
	ADD CONSTRAINT `FK_ToDoSubCategory_ToDoCategory` FOREIGN KEY (`CategoryId`) REFERENCES `ToDoCategory` (`CategoryId`);
	
ALTER TABLE `Clients`
	CHANGE COLUMN `DateCreatedUtc` `DateAddedUtc` DATETIME NOT NULL DEFAULT utc_timestamp(4) AFTER `Deleted`,
	CHANGE COLUMN `DateModifiedUtc` `DateUpdatedUtc` DATETIME NULL DEFAULT NULL AFTER `DateAddedUtc`;
	
ALTER TABLE `DailyTasks`
	CHANGE COLUMN `DateCreatedUtc` `DateAddedUtc` DATETIME NOT NULL DEFAULT utc_timestamp(4) AFTER `Deleted`,
	CHANGE COLUMN `DateModifiedUtc` `DateUpdatedUtc` DATETIME NULL DEFAULT NULL AFTER `DateAddedUtc`;
	
ALTER TABLE `Products`
	CHANGE COLUMN `DateCreatedUtc` `DateAddedUtc` DATETIME NOT NULL DEFAULT utc_timestamp(4) AFTER `Deleted`,
	CHANGE COLUMN `DateModifiedUtc` `DateUpdatedUtc` DATETIME NULL DEFAULT NULL AFTER `DateAddedUtc`;
	
ALTER TABLE `Projects`
	CHANGE COLUMN `DateCreatedUtc` `DateAddedUtc` DATETIME NOT NULL DEFAULT utc_timestamp(4) AFTER `Deleted`,
	CHANGE COLUMN `DateModifiedUtc` `DateUpdatedUtc` DATETIME NULL DEFAULT NULL AFTER `DateAddedUtc`;
	
ALTER TABLE `ToDoCategory`
	CHANGE COLUMN `DateCreatedUtc` `DateAddedUtc` TIMESTAMP NOT NULL DEFAULT utc_timestamp(4) AFTER `Deleted`,
	CHANGE COLUMN `DateModifiedUtc` `DateUpdatedUtc` TIMESTAMP NULL DEFAULT NULL AFTER `DateAddedUtc`;
	
ALTER TABLE `ToDoList`
	CHANGE COLUMN `DateModifiedUtc` `DateUpdatedUtc` DATETIME NULL DEFAULT NULL AFTER `DateAddedUtc`;
	
ALTER TABLE `ToDoSubCategory`
	CHANGE COLUMN `DateCreatedUtc` `DateAddedUtc` DATETIME NOT NULL DEFAULT utc_timestamp(4) AFTER `Deleted`,
	CHANGE COLUMN `DateModifiedUtc` `DateUpdatedUtc` DATETIME NULL DEFAULT NULL AFTER `DateAddedUtc`;
	
ALTER TABLE `Users`
	CHANGE COLUMN `DateCreatedUtc` `DateAddedUtc` DATETIME NOT NULL DEFAULT utc_timestamp(4) AFTER `Deleted`,
	CHANGE COLUMN `DateModifiedUtc` `DateUpdatedUtc` DATETIME NULL DEFAULT NULL AFTER `DateAddedUtc`;
	
ALTER TABLE `Clients`
	ADD COLUMN `DateDeletedUtc` DATETIME NULL DEFAULT NULL AFTER `DateUpdatedUtc`;
	
ALTER TABLE `DailyTasks`
	ADD COLUMN `DateDeletedUtc` DATETIME NULL DEFAULT NULL AFTER `DateUpdatedUtc`;
	
ALTER TABLE `Products`
	ADD COLUMN `DateDeletedUtc` DATETIME NULL DEFAULT NULL AFTER `DateUpdatedUtc`;
	
ALTER TABLE `Projects`
	ADD COLUMN `DateDeletedUtc` DATETIME NULL DEFAULT NULL AFTER `DateUpdatedUtc`;
	
ALTER TABLE `TimeSheet_Date`
	ADD COLUMN `DateDeletedUtc` DATETIME NULL DEFAULT NULL AFTER `DateUpdatedUtc`;
	
ALTER TABLE `TimeSheet_Entries`
	ADD COLUMN `DateDeletedUtc` DATETIME NULL DEFAULT NULL AFTER `DateUpdatedUtc`;
	
ALTER TABLE `TimeSheet_Rows`
	ADD COLUMN `DateDeletedUtc` DATETIME NULL DEFAULT NULL AFTER `DateUpdatedUtc`;
	
ALTER TABLE `ToDoCategory`
	ADD COLUMN `DateDeletedUtc` TIMESTAMP NULL DEFAULT NULL AFTER `DateUpdatedUtc`;
	
ALTER TABLE `ToDoList`
	ADD COLUMN `DateDeletedUtc` DATETIME NULL DEFAULT NULL AFTER `DateUpdatedUtc`;
	
ALTER TABLE `ToDoSubCategory`
	ADD COLUMN `DateDeletedUtc` DATETIME NULL DEFAULT NULL AFTER `DateUpdatedUtc`;
	
ALTER TABLE `Users`
	ADD COLUMN `DateDeletedUtc` DATETIME NULL DEFAULT NULL AFTER `LastLoginDateUtc`;
	
