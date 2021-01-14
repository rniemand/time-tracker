ALTER TABLE `Timers`
	ADD COLUMN `TaskId` INT(11) NOT NULL DEFAULT '0' AFTER `ProjectId`,
	ADD INDEX `TaskId` (`TaskId`);
	
CREATE TABLE `DailyTasks` (
	`TaskId` INT(11) NOT NULL AUTO_INCREMENT,
	`UserId` INT(11) NOT NULL DEFAULT '0',
	`ClientId` INT(11) NOT NULL DEFAULT '0',
	`Deleted` BIT(1) NOT NULL DEFAULT b'0',
	`DateCreatedUtc` DATETIME NOT NULL DEFAULT current_timestamp(),
	`DateModifiedUtc` DATETIME NULL DEFAULT NULL,
	`TaskName` VARCHAR(128) NOT NULL DEFAULT '' COLLATE 'utf8mb4_general_ci',
	PRIMARY KEY (`TaskId`) USING BTREE,
	INDEX `UserId` (`UserId`) USING BTREE,
	INDEX `ClientId` (`ClientId`) USING BTREE,
	INDEX `Deleted` (`Deleted`) USING BTREE
)
COLLATE='utf8mb4_general_ci'
ENGINE=InnoDB
;

